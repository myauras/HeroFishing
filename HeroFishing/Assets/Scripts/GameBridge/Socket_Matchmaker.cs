using HeroFishing.Main;
using HeroFishing.Socket.Matchmaker;
using Scoz.Func;
using Service.Realms;
using System;
using System.Threading.Tasks;
using UnityEngine;
namespace HeroFishing.Socket {
    public partial class HeroFishingSocket {
        public event Action<bool, string> CreateRoomCallback;

        TcpClient TCP_MatchmakerClient;
        public void NewMatchmakerTCPClient(string _ip, int _port) {
            if (TCP_MatchmakerClient != null)
                TCP_MatchmakerClient.Close();
            TCP_MatchmakerClient = new GameObject("MatchmakerSocket").AddComponent<TcpClient>();
            TCP_MatchmakerClient.Init(_ip, _port);
            TCP_MatchmakerClient.OnReceiveMsg += OnRecieveTCPMsg;
        }
        private void OnMatchmakerDisconnect() {
            WriteLog.LogColor("OnMatchmakerDisconnect", WriteLog.LogType.Connection);
        }
        public void LoginToMatchmaker(string _realmToken, Func<string, bool, Task> _callback) {
            WriteLog.LogColor("LoginToMatchmaker", WriteLog.LogType.Connection);
            if (TCP_MatchmakerClient == null) {
                WriteLog.LogError("TCP_MatchmakerClient is null");
                _callback?.Invoke(null, false);
                return;
            }
            CMDCallback.Clear();
            TCP_MatchmakerClient.UnRegistOnDisconnect(OnMatchmakerDisconnect);
            TCP_MatchmakerClient.StartConnect((bool isConnect) => {
                if (!isConnect) {
                    _callback?.Invoke(_realmToken, false);
                    return;
                }
                SocketCMD<AUTH> command = new SocketCMD<AUTH>(new AUTH(_realmToken));

                int id = TCP_MatchmakerClient.Send(command);
                if (id < 0) {
                    _callback?.Invoke(null, false);
                    return;
                }
                RegistrMatchgameCommandCB(new Tuple<string, int>(SocketContent.ReplyType.AUTH_REPLY.ToString(), id), (string msg) => {
                    SocketCMD<AUTH_REPLY> packet = LitJson.JsonMapper.ToObject<SocketCMD<AUTH_REPLY>>(msg);
                    _callback?.Invoke(_realmToken, packet.Content.IsAuth);
                });
            });
            TCP_MatchmakerClient.RegistOnDisconnect(OnMatchmakerDisconnect);
        }

        public void CreateMatchmakerRoom(string _dbMapID, Action<bool, string> _cb) {
            WriteLog.LogColor("CreateMatchmakerRoom", WriteLog.LogType.Connection);
            CreateRoomCallback = _cb;
            CREATEROOM cmdContent = new CREATEROOM(_dbMapID, RealmManager.MyApp.CurrentUser.Id);//建立封包內容
            SocketCMD<CREATEROOM> cmd = new SocketCMD<CREATEROOM>(cmdContent);//建立封包
            int id = TCP_MatchmakerClient.Send(cmd);//送出封包
            if (id < 0) {
                _cb?.Invoke(false, "");
                return;
            }
            //註冊回呼
            WriteLog.LogColor("註冊回呼", WriteLog.LogType.Connection);
            RegistrMatchgameCommandCB(new Tuple<string, int>(SocketContent.ReplyType.CREATEROOM_REPLY.ToString(), id), OnCreateMatchmakerRoom_Reply);
        }
        public void OnCreateMatchmakerRoom_Reply(string _msg) {
            WriteLog.LogColor("OnCreateMatchmakerRoom_Reply", WriteLog.LogType.Connection);
            var packet = LitJson.JsonMapper.ToObject<SocketCMD<CREATEROOM_REPLY>>(_msg);

            //有錯誤
            if (!string.IsNullOrEmpty(packet.ErrMsg)) {
                WriteLog.LogError("Create MatchmakerRoom Fail : " + packet.ErrMsg);
                CreateRoomCallback?.Invoke(false, packet.ErrMsg);
                CreateRoomCallback = null;
                return;
            }

            //玩家目前所在遊戲房間的資料
            AllocatedRoom.Instance.InitRoom(packet.Content.CreaterID, packet.Content.PlayerIDs, packet.Content.DBMapID, packet.Content.DBMatchgameID, packet.Content.IP, packet.Content.Port, packet.Content.PodName);
            CreateRoomCallback?.Invoke(true, string.Empty);
            CreateRoomCallback = null;
        }


    }
}
