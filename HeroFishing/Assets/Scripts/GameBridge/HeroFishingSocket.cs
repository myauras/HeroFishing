using LitJson;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Socket {
    public class HeroFishingSocket : HeroFishingNetwork {


        static HeroFishingSocket Instance = null;
        public static HeroFishingNetwork GetInstance() {
            if (Instance == null) {
                Instance = new HeroFishingSocket();
                Instance.Init();
            }
            return Instance;
        }


        //public static readonly DateTime SERVER_START_TIME = new DateTime(1970, 1, 1, 0 ,0, 0, DateTimeKind.Utc);

        private TcpClient MatchmakerClient;
        private TcpClient gameClient;
        private UdpSocket udpClient;
        private ServerTimeSyncer timeSyncer;

        private Dictionary<Tuple<string, int>, Action<string>> commandCallback = new Dictionary<Tuple<string, int>, Action<string>>();

        //斷線事件
        public event Action disconnectCallback;
        //房間狀態更新事件
        public event Action roomUpdateCallback;

        public event Action<bool, string> waitinglayerCallback;

        //房主更新事件
        public event Action<bool> masterSwitchCallback;

        //離開房間事件
        public event Action leftRoomCallback;


        private string gameServerIP;
        private int gameServerPort;
        private string gameServerUdpToken;

        /// <summary>
        /// 快速開房等待中
        /// </summary>
        public bool IsInCreateQuickRoomWaiting { get; private set; }

        public override void Init() {
        }
        public override void Release() {
            if (Instance != null)
                Instance.Dispose();
            Instance = null;
        }
        public override void SetServerIP(string _ip, int _port) {
            if (MatchmakerClient != null)
                MatchmakerClient.Close();
            MatchmakerClient = new GameObject("MatchmakerSocket").AddComponent<TcpClient>();

#if Dev
            WriteLog.Log("Connect to server " + _ip + " " + _port);
            MatchmakerClient.Init(_ip, _port);
#else
            lobbyClient.Init(address, port);
#endif      
            MatchmakerClient.OnReceiveMsg += OnRecieveMessage;
        }

        public override void Dispose() {
            if (MatchmakerClient != null)
                MatchmakerClient.Close();
            if (gameClient != null)
                gameClient.Close();
            if (udpClient != null)
                udpClient.Close();
            if (timeSyncer != null)
                GameObject.Destroy(timeSyncer.gameObject);
        }

        private void OnRecieveMessage(string message) {
            if (udpClient != null)
                udpClient.ResetTimer();
            else if (!string.IsNullOrEmpty(gameServerIP) && !string.IsNullOrEmpty(gameServerUdpToken)) {
                udpClient = new GameObject("GameUdpSocket").AddComponent<UdpSocket>();
                udpClient.Init(gameServerIP, gameServerPort);
                try {
                    udpClient.StartConnect(gameServerUdpToken, (bool isConnect) => {
                        if (isConnect)
                            udpClient.OnReceiveMsg += OnConnectionCheck;
                    });
                    udpClient.RegistOnDisconnect(OnUDPDisconnect);
                } catch (Exception e) {
                    WriteLog.LogError("UDP error " + e.ToString());
                }
            }
            try {
                SocketCommand<BaseContent> data = JsonMapper.ToObject<SocketCommand<BaseContent>>(message);
                //LitJson.JsonData obj = LitJson.JsonMapper.ToObject(message);
                WriteLog.LogFormat($"<color=#9b791d>Recieve Command [{data.Command}] </color>");
                Tuple<string, int> commandID = new Tuple<string, int>(data.Command, data.PacketID);
                if (commandCallback.TryGetValue(commandID, out Action<string> _cb)) {
                    commandCallback.Remove(commandID);
                    _cb?.Invoke(message);
                } else {
                    //Server brocast message or just no callback?
                    switch (data.Command) {
                        case "START_GAME":
                            break;
                    }
                }
            } catch (Exception e) {
                WriteLog.LogError("Parse Recieve Message with Error : " + e.ToString());
                if (SceneManager.GetActiveScene().name != MyScene.BattleScene.ToString()) {
                    Release();
                }
            }
        }

        private void RegistCommandCallback(Tuple<string, int> commandID, Action<string> callback) {
            if (commandCallback.ContainsKey(commandID)) {
                WriteLog.LogError("Command remain here should not happen.");
                commandCallback.Remove(commandID);
            }
            commandCallback.Add(commandID, callback);
        }

        private void OnLobbyDisConnect() {
            WriteLog.Log("[MaJamSocket] OnLobbyDisConnect");
            disconnectCallback?.Invoke();
        }

        private void OnGameDisConnect() {
            WriteLog.Log("[MaJamSocket] OnGameDisConnect");
            disconnectCallback?.Invoke();
        }

        public override void Login(string token, Action<bool> callback) {
            WriteLog.Log("[MaJamSocket] Login");
            if (MatchmakerClient == null) {
                WriteLog.LogError("LobbyClient is null");
                callback?.Invoke(false);
                return;
            }
            commandCallback.Clear();
            MatchmakerClient.UnRegistOnDisconnect(OnLobbyDisConnect);
            MatchmakerClient.StartConnect((bool isConnect) => {
                if (!isConnect) {
                    callback?.Invoke(false);
                    return;
                }
                SocketCommand<Auth> command = new SocketCommand<Auth>(new Auth(token));

                int id = MatchmakerClient.Send(command);
                if (id < 0) {
                    callback?.Invoke(false);
                    return;
                }
                RegistCommandCallback(new Tuple<string, int>("ReAuth", id), (string msg) => {
                    SocketCommand<Auth_Receive> packet = LitJson.JsonMapper.ToObject<SocketCommand<Auth_Receive>>(msg);
                    callback?.Invoke(packet.Content.IsAuth);
                });
            });
            MatchmakerClient.RegistOnDisconnect(OnLobbyDisConnect);
        }

        public override void CreateRoom(string roomName, Action<bool, string> creatCallback) {
            WriteLog.Log("[MaJamSocket] CreateRoom");
        }
        public override void Disconnect() {
            WriteLog.Log("[MaJamSocket] DisConnect");
        }

        public override int GetPing() {
            if (timeSyncer)
                return Mathf.RoundToInt(timeSyncer.GetLantency() * 1000);
            return 0;
        }
        public override bool IsLogin() {
            return (MatchmakerClient != null && MatchmakerClient.IsConnected) || (gameClient != null && gameClient.IsConnected);
        }

        public void OnConnectionCheck(string message) {
            try {
                SocketCommand<UdpUpdatePacket> packet = LitJson.JsonMapper.ToObject<SocketCommand<UdpUpdatePacket>>(message);
                //WriteLog.Log("server time : " + packet.Content.ServerTime);
                timeSyncer.SycServerTime(packet.Content.ServerTime);
            } catch (Exception e) {
                WriteLog.LogError("Parse UDP Message with Error : " + e.ToString());
            }
        }

        public void OnUDPDisconnect() {
            udpClient.OnReceiveMsg -= OnConnectionCheck;
            //沒有timeout重連UDP
            if (udpClient != null && udpClient.CheckTimerInTime()) {
                udpClient.Close();
                udpClient = new GameObject("GameUdpSocket").AddComponent<UdpSocket>();
                udpClient.Init(gameServerIP, gameServerPort);
                udpClient.StartConnect(gameServerUdpToken, (bool udpConnect) => {
                    WriteLog.Log("Udp isConnect :" + udpConnect);
                    if (udpConnect)
                        udpClient.OnReceiveMsg += OnConnectionCheck;
                    else {
                        this.OnGameDisConnect();
                        this.Disconnect();
                    }
                });
                udpClient.RegistOnDisconnect(OnUDPDisconnect);
            } else {
                WriteLog.LogError("OnUDPDisconnect");

                this.OnGameDisConnect();
                this.Disconnect();
            }
        }

        public override void LeaveRoom() {
        }

        public override void RegistDisconnectCallback(Action callback) {
            WriteLog.Log("[MaJamSocket] RegistDisconnectCallback");
            disconnectCallback += callback;
        }
    }
}
