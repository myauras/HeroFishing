using DG.Tweening;
using HeroFishing.Main;
using LitJson;
using NSubstitute;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
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
                WriteLog.LogColorFormat("Recieve Command: {0}   PackID: {1}", WriteLog.LogType.Connection, data.CMD, data.PackID);
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
            WriteLog.LogColor("OnLobbyDisConnect", WriteLog.LogType.Connection);
            disconnectCallback?.Invoke();
        }

        private void OnGameDisConnect() {
            WriteLog.LogColor("OnGameDisConnect", WriteLog.LogType.Connection);
            disconnectCallback?.Invoke();
        }

        public void Login(string _token, Func<string, bool, Task> _callback) {
            WriteLog.LogColor("Login", WriteLog.LogType.Connection);
            if (MatchmakerClient == null) {
                WriteLog.LogError("MatchmakerClient is null");
                _callback?.Invoke(null, false);
                return;
            }
            CMDCallback.Clear();
            MatchmakerClient.UnRegistOnDisconnect(OnLobbyDisConnect);
            MatchmakerClient.StartConnect((bool isConnect) => {
                if (!isConnect) {
                    _callback?.Invoke(_token, false);
                    return;
                }
                SocketCMD<AUTH> command = new SocketCMD<AUTH>(new AUTH(_token));

                int id = MatchmakerClient.Send(command);
                if (id < 0) {
                    _callback?.Invoke(null, false);
                    return;
                }
                RegistCommandCallback(new Tuple<string, int>(SocketContent.ReplyType.AUTH_REPLY.ToString(), id), (string msg) => {
                    SocketCMD<Auth_Reply> packet = LitJson.JsonMapper.ToObject<SocketCMD<Auth_Reply>>(msg);
                    _callback?.Invoke(_token, packet.Content.IsAuth);
                });
            });
            MatchmakerClient.RegistOnDisconnect(OnLobbyDisConnect);
        }

        public void CreateRoom(string _dbMapID, Action<bool, string> _cb) {
            WriteLog.LogColor("CreateRoom", WriteLog.LogType.Connection);
            CreateRoomCallback = _cb;
            CREATEROOM cmdContent = new CREATEROOM(_dbMapID, RealmManager.MyApp.CurrentUser.Id);//建立封包內容
            SocketCMD<CREATEROOM> cmd = new SocketCMD<CREATEROOM>(cmdContent);//建立封包
            int id = MatchmakerClient.Send(cmd);//送出封包
            if (id < 0) {
                _cb?.Invoke(false, "");
                return;
            }
            //註冊回呼
            WriteLog.LogColor("註冊回呼", WriteLog.LogType.Connection);
            RegistCommandCallback(new Tuple<string, int>(SocketContent.ReplyType.CREATEROOM_REPLY.ToString(), id), OnCreateRoom_Reply);
        }
        public void OnCreateRoom_Reply(string _msg) {
            WriteLog.LogColor("OnCreateRoom_Reply", WriteLog.LogType.Connection);
            var packet = LitJson.JsonMapper.ToObject<SocketCMD<CreateRoom_Reply>>(_msg);

            //有錯誤
            if (!string.IsNullOrEmpty(packet.ErrMsg)) {
                WriteLog.LogError("Create Room Fail : " + packet.ErrMsg);
                CreateRoomCallback?.Invoke(false, packet.ErrMsg);
                CreateRoomCallback = null;
                return;
            }

            AllocatedRoom.Instance.Init(packet.Content.DBMapID, packet.Content.CreaterID, packet.Content.GameServerIP, packet.Content.GameServerPort, packet.Content.GameServerName);//初始化房間資料
            CreateRoomCallback?.Invoke(true, string.Empty);
            CreateRoomCallback = null;
        }
        public void Disconnect() {
            WriteLog.Log("DisConnect");
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
            WriteLog.LogColor("RegistDisconnectCallback", WriteLog.LogType.Connection);
            disconnectCallback += callback;
        }
    }
}
