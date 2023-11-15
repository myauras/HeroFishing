using Cysharp.Threading.Tasks;
using HeroFishing.Main;
using HeroFishing.Socket.Matchmaker;
using LitJson;
using Scoz.Func;
using Service.Realms;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Socket {
    public partial class HeroFishingSocket {
        public event Action<CREATEROOM_REPLY, string> CreateRoomCallback;

        TcpClient TCP_MatchmakerClient;
        public void NewMatchmakerTCPClient(string _ip, int _port) {
            if (TCP_MatchmakerClient != null)
                TCP_MatchmakerClient.Close();
            TCP_MatchmakerClient = new GameObject("MatchmakerSocket").AddComponent<TcpClient>();
            TCP_MatchmakerClient.Init(_ip, _port);
            TCP_MatchmakerClient.OnReceiveMsg += OnRecieveMatchmakerTCPMsg;
        }
        private void OnMatchmakerDisconnect() {
            WriteLog.LogColor("OnMatchmakerDisconnect", WriteLog.LogType.Connection);
        }
        public void LoginToMatchmaker(string _realmToken, Func<bool, UniTask> _callback) {
            WriteLog.LogColor("LoginToMatchmaker", WriteLog.LogType.Connection);
            if (TCP_MatchmakerClient == null) {
                WriteLog.LogError("TCP_MatchmakerClient is null");
                _callback?.Invoke(false);
                return;
            }
            CMDCallback.Clear();
            TCP_MatchmakerClient.UnRegistOnDisconnect(OnMatchmakerDisconnect);
            TCP_MatchmakerClient.StartConnect((bool isConnect) => {
                if (!isConnect) {
                    _callback?.Invoke(false);
                    return;
                }
                SocketCMD<AUTH> command = new SocketCMD<AUTH>(new AUTH(_realmToken));

                int id = TCP_MatchmakerClient.Send(command);
                if (id < 0) {
                    _callback?.Invoke(false);
                    return;
                }
                RegistrMatchgameCommandCB(new Tuple<string, int>(SocketContent.MatchmakerCMDType.AUTH_REPLY.ToString(), id), (string msg) => {
                    SocketCMD<AUTH_REPLY> packet = LitJson.JsonMapper.ToObject<SocketCMD<AUTH_REPLY>>(msg);
                    _callback?.Invoke(packet.Content.IsAuth);
                });
            });
            TCP_MatchmakerClient.RegistOnDisconnect(OnMatchmakerDisconnect);
        }

        public void CreateMatchmakerRoom(string _dbMapID, Action<CREATEROOM_REPLY, string> _cb) {
            WriteLog.LogColor("CreateMatchmakerRoom", WriteLog.LogType.Connection);
            CreateRoomCallback = _cb;
            CREATEROOM cmdContent = new CREATEROOM(_dbMapID, RealmManager.MyApp.CurrentUser.Id);//建立封包內容
            SocketCMD<CREATEROOM> cmd = new SocketCMD<CREATEROOM>(cmdContent);//建立封包
            int id = TCP_MatchmakerClient.Send(cmd);//送出封包
            if (id < 0) {
                _cb?.Invoke(null, "packID小於0");
                return;
            }
            //註冊回呼
            WriteLog.LogColor("註冊回呼", WriteLog.LogType.Connection);
            RegistrMatchgameCommandCB(new Tuple<string, int>(SocketContent.MatchmakerCMDType.CREATEROOM_REPLY.ToString(), id), OnCreateMatchmakerRoom_Reply);
        }
        public void OnCreateMatchmakerRoom_Reply(string _msg) {
            WriteLog.LogColor("OnCreateMatchmakerRoom_Reply", WriteLog.LogType.Connection);
            var packet = LitJson.JsonMapper.ToObject<SocketCMD<CREATEROOM_REPLY>>(_msg);

            //有錯誤
            if (!string.IsNullOrEmpty(packet.ErrMsg)) {
                WriteLog.LogError("Create MatchmakerRoom Fail : " + packet.ErrMsg);
                CreateRoomCallback?.Invoke(null, packet.ErrMsg);
                CreateRoomCallback = null;
                return;
            }
            CreateRoomCallback?.Invoke(packet.Content, null);
            CreateRoomCallback = null;
        }

        private void OnRecieveMatchmakerTCPMsg(string _msg) {
            try {
                SocketCMD<SocketContent> data = JsonMapper.ToObject<SocketCMD<SocketContent>>(_msg);
                WriteLog.LogColorFormat("Recieve Command: {0}   PackID: {1}", WriteLog.LogType.Connection, data.CMD, data.PackID);
                Tuple<string, int> commandID = new Tuple<string, int>(data.CMD, data.PackID);
                if (CMDCallback.TryGetValue(commandID, out Action<string> _cb)) {
                    CMDCallback.Remove(commandID);
                    _cb?.Invoke(_msg);
                } else {
                    SocketContent.MatchmakerCMDType cmdType;
                    if (!MyEnum.TryParseEnum(data.CMD, out cmdType)) {
                        WriteLog.LogErrorFormat("收到錯誤的命令類型: {0}", cmdType);
                        return;
                    }
                    switch (cmdType) {
                        case SocketContent.MatchmakerCMDType.AUTH_REPLY:
                            WriteLog.LogColor("AUTH_REPLY", WriteLog.LogType.Connection);
                            break;
                        case SocketContent.MatchmakerCMDType.CREATEROOM_REPLY:
                            WriteLog.LogColor("CREATEROOM_REPLY", WriteLog.LogType.Connection);
                            break;
                    }
                }
            } catch (Exception e) {
                WriteLog.LogError("Parse收到的封包時出錯 : " + e.ToString());
                if (SceneManager.GetActiveScene().name != MyScene.BattleScene.ToString()) {
                    WriteLog.LogErrorFormat("不在{0}就釋放資源: ", MyScene.BattleScene, e.ToString());
                    Release();
                }
            }
        }


    }
}
