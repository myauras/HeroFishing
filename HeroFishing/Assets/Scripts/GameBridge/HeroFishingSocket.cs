using DG.Tweening;
using HeroFishing.Main;
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
    public class HeroFishingSocket {

        public enum SendTarget {
            Master,
            Other,
            All,//include self.
            One
        }
        protected static HeroFishingSocket Instance = null;
        public static HeroFishingSocket GetInstance() {
            if (Instance == null) {
                Instance = new HeroFishingSocket();
                Instance.Init();
            }
            return Instance;
        }

        // 回呼
        public event Action<bool, string> CreateRoomCallback;

        //public static readonly DateTime SERVER_START_TIME = new DateTime(1970, 1, 1, 0 ,0, 0, DateTimeKind.Utc);


        TcpClient MatchmakerClient;
        TcpClient GameClient;
        UdpSocket UDPClient;
        ServerTimeSyncer TimeSyncer;

        Dictionary<Tuple<string, int>, Action<string>> CMDCallback = new Dictionary<Tuple<string, int>, Action<string>>();

        //斷線事件
        public event Action disconnectCallback;
        //房間狀態更新事件
        public event Action roomUpdateCallback;


        private string gameServerIP;
        private int gameServerPort;
        private string gameServerUdpToken;


        public void Init() {
        }
        public void Release() {
            if (Instance != null)
                Instance.Dispose();
            Instance = null;
        }
        public void SetServerIP(string _ip, int _port) {
            if (MatchmakerClient != null)
                MatchmakerClient.Close();
            MatchmakerClient = new GameObject("MatchmakerSocket").AddComponent<TcpClient>();

#if Dev
            WriteLog.LogColor("Connect to server " + _ip + " " + _port, WriteLog.LogType.Connection);
            MatchmakerClient.Init(_ip, _port);
#else
            MatchmakerClient.Init(_ip, _port);
#endif      
            MatchmakerClient.OnReceiveMsg += OnRecieveMessage;

        }

        public void Dispose() {
            if (MatchmakerClient != null)
                MatchmakerClient.Close();
            if (GameClient != null)
                GameClient.Close();
            if (UDPClient != null)
                UDPClient.Close();
            if (TimeSyncer != null)
                GameObject.Destroy(TimeSyncer.gameObject);
        }

        private void OnRecieveMessage(string message) {
            if (UDPClient != null)
                UDPClient.ResetTimer();
            else if (!string.IsNullOrEmpty(gameServerIP) && !string.IsNullOrEmpty(gameServerUdpToken)) {
                UDPClient = new GameObject("GameUdpSocket").AddComponent<UdpSocket>();
                UDPClient.Init(gameServerIP, gameServerPort);
                try {
                    UDPClient.StartConnect(gameServerUdpToken, (bool isConnect) => {
                        if (isConnect)
                            UDPClient.OnReceiveMsg += OnConnectionCheck;
                    });
                    UDPClient.RegistOnDisconnect(OnUDPDisconnect);
                } catch (Exception e) {
                    WriteLog.LogError("UDP error " + e.ToString());
                }
            }
            try {
                SocketCMD<SocketContent> data = JsonMapper.ToObject<SocketCMD<SocketContent>>(message);
                //LitJson.JsonData obj = LitJson.JsonMapper.ToObject(message);
                WriteLog.LogColorFormat("Recieve Command: {0}", WriteLog.LogType.Connection, data.CMD);
                Tuple<string, int> commandID = new Tuple<string, int>(data.CMD, data.PackID);
                if (CMDCallback.TryGetValue(commandID, out Action<string> _cb)) {
                    CMDCallback.Remove(commandID);
                    _cb?.Invoke(message);
                } else {
                    SocketContent.ReplyType cmdType;
                    if (!MyEnum.TryParseEnum(data.CMD, out cmdType)) {
                        WriteLog.LogErrorFormat("收到錯誤的命令類型: {0}", cmdType);
                        return;
                    }
                    switch (cmdType) {
                        case SocketContent.ReplyType.CREATEROOM_REPLY:
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
            if (CMDCallback.ContainsKey(commandID)) {
                WriteLog.LogError("Command remain here should not happen.");
                CMDCallback.Remove(commandID);
            }
            CMDCallback.Add(commandID, callback);
        }

        private void OnLobbyDisConnect() {
            WriteLog.LogColor("[HeroFishingSocket] OnLobbyDisConnect", WriteLog.LogType.Connection);
            disconnectCallback?.Invoke();
        }

        private void OnGameDisConnect() {
            WriteLog.LogColor("[HeroFishingSocket] OnGameDisConnect", WriteLog.LogType.Connection);
            disconnectCallback?.Invoke();
        }

        public void Login(string token, Action<bool> callback) {
            WriteLog.LogColor("[HeroFishingSocket] Login", WriteLog.LogType.Connection);
            if (MatchmakerClient == null) {
                WriteLog.LogError("MatchmakerClient is null");
                callback?.Invoke(false);
                return;
            }
            CMDCallback.Clear();
            MatchmakerClient.UnRegistOnDisconnect(OnLobbyDisConnect);
            MatchmakerClient.StartConnect((bool isConnect) => {
                if (!isConnect) {
                    callback?.Invoke(false);
                    return;
                }
                SocketCMD<AUTH> command = new SocketCMD<AUTH>(new AUTH(token));

                int id = MatchmakerClient.Send(command);
                if (id < 0) {
                    callback?.Invoke(false);
                    return;
                }
                RegistCommandCallback(new Tuple<string, int>(SocketContent.ReplyType.AUTH_REPLY.ToString(), id), (string msg) => {
                    SocketCMD<Auth_Reply> packet = LitJson.JsonMapper.ToObject<SocketCMD<Auth_Reply>>(msg);
                    callback?.Invoke(packet.Content.IsAuth);
                });
            });
            MatchmakerClient.RegistOnDisconnect(OnLobbyDisConnect);
        }

        public void CreateRoom(string _mapID, Action<bool, string> _cb) {
            WriteLog.LogColor("[HeroFishingSocket] CreateRoom", WriteLog.LogType.Connection);
            RegistCreateRoomCallback(_cb);
            CREATEROOM cmdContent = new CREATEROOM(_mapID, new string[] { "scoz" }, "scoz");//建立封包內容
            SocketCMD<CREATEROOM> cmd = new SocketCMD<CREATEROOM>(cmdContent);//建立封包
            int id = MatchmakerClient.Send(cmd);//送出封包
            if (id < 0) {
                _cb?.Invoke(false, "");
                return;
            }
            //註冊回呼
            RegistCommandCallback(new Tuple<string, int>(SocketContent.ReplyType.CREATEROOM_REPLY.ToString(), -1), OnCreateRoom_Reply);
        }
        public void RegistCreateRoomCallback(Action<bool, string> callback) {
            CreateRoomCallback = callback;
        }
        public void UnRegistCreateRoomCallback() {
            CreateRoomCallback = null;
        }
        public void OnCreateRoom_Reply(string _msg) {
            WriteLog.LogColor("[MaJamSocket] OnWaitingReCreateRoom", WriteLog.LogType.Connection);
            var packet = LitJson.JsonMapper.ToObject<SocketCMD<CreateRoom_Reply>>(_msg);

            //有錯誤
            if (!string.IsNullOrEmpty(packet.ErrMsg)) {
                WriteLog.LogError("Create Room Fail : " + packet.ErrMsg);
                CreateRoomCallback?.Invoke(false, packet.ErrMsg);
                UnRegistCreateRoomCallback();
                return;
            }


            GameRoomData.Instance.Init();//初始化房間資料
            CreateRoomCallback?.Invoke(true, string.Empty);
            UnRegistCreateRoomCallback();
        }
        public void Disconnect() {
            WriteLog.Log("[HeroFishingSocket] DisConnect");
        }

        public int GetPing() {
            if (TimeSyncer)
                return Mathf.RoundToInt(TimeSyncer.GetLantency() * 1000);
            return 0;
        }
        public bool IsLogin() {
            return (MatchmakerClient != null && MatchmakerClient.IsConnected) || (GameClient != null && GameClient.IsConnected);
        }

        public void OnConnectionCheck(string message) {
            try {
                SocketCMD<UdpUpdatePacket> packet = LitJson.JsonMapper.ToObject<SocketCMD<UdpUpdatePacket>>(message);
                //WriteLog.Log("server time : " + packet.Content.ServerTime);
                TimeSyncer.SycServerTime(packet.Content.ServerTime);
            } catch (Exception e) {
                WriteLog.LogError("Parse UDP Message with Error : " + e.ToString());
            }
        }

        public void OnUDPDisconnect() {
            UDPClient.OnReceiveMsg -= OnConnectionCheck;
            //沒有timeout重連UDP
            if (UDPClient != null && UDPClient.CheckTimerInTime()) {
                UDPClient.Close();
                UDPClient = new GameObject("GameUdpSocket").AddComponent<UdpSocket>();
                UDPClient.Init(gameServerIP, gameServerPort);
                UDPClient.StartConnect(gameServerUdpToken, (bool udpConnect) => {
                    WriteLog.Log("Udp isConnect :" + udpConnect);
                    if (udpConnect)
                        UDPClient.OnReceiveMsg += OnConnectionCheck;
                    else {
                        this.OnGameDisConnect();
                        this.Disconnect();
                    }
                });
                UDPClient.RegistOnDisconnect(OnUDPDisconnect);
            } else {
                WriteLog.LogError("OnUDPDisconnect");

                this.OnGameDisConnect();
                this.Disconnect();
            }
        }

        public void LeaveRoom() {
        }

        public void RegistDisconnectCallback(Action callback) {
            WriteLog.LogColor("[HeroFishingSocket] RegistDisconnectCallback", WriteLog.LogType.Connection);
            disconnectCallback += callback;
        }
    }
}
