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
    public partial class HeroFishingSocket {

        protected static HeroFishingSocket Instance = null;
        public static HeroFishingSocket GetInstance() {
            if (Instance == null) {
                Instance = new HeroFishingSocket();
                Instance.Init();
            }
            return Instance;
        }


        public void Init() {
        }
        public void Release() {
            if (Instance != null)
                Instance.Dispose();
            Instance = null;
        }
        public void Dispose() {
            if (TCP_MatchmakerClient != null)
                TCP_MatchmakerClient.Close();
            if (TCP_MatchgameClient != null)
                TCP_MatchgameClient.Close();
            if (UDP_MatchgameClient != null)
                UDP_MatchgameClient.Close();
            if (TimeSyncer != null)
                GameObject.Destroy(TimeSyncer.gameObject);
        }


        private void OnRecieveTCPMsg(string _msg) {
            //if (UDP_MatchgameClient != null)
            //    UDP_MatchgameClient.ResetTimer();
            //var matchgame = GamePlayer.Instance.GetMatchGame();
            //if (matchgame != null) {
            //    UDP_MatchgameClient = new GameObject("UDP_MatchgameClient").AddComponent<UdpSocket>();
            //    UDP_MatchgameClient.Init(matchgame.IP, matchgame.Port);
            //    try {
            //        UDP_MatchgameClient.StartConnect(UDP_MatchgameConnToken, (bool isConnect) => {
            //            if (isConnect)
            //                UDP_MatchgameClient.OnReceiveMsg += OnMatchgameUDPConnCheck;
            //        });
            //        UDP_MatchgameClient.RegistOnDisconnect(OnMatchgameUDPDisconnect);
            //    } catch (Exception e) {
            //        WriteLog.LogError("UDP error " + e.ToString());
            //    }
            //}
            try {
                SocketCMD<SocketContent> data = JsonMapper.ToObject<SocketCMD<SocketContent>>(_msg);
                WriteLog.LogColorFormat("Recieve Command: {0}   PackID: {1}", WriteLog.LogType.Connection, data.CMD, data.PackID);
                Tuple<string, int> commandID = new Tuple<string, int>(data.CMD, data.PackID);
                if (CMDCallback.TryGetValue(commandID, out Action<string> _cb)) {
                    CMDCallback.Remove(commandID);
                    _cb?.Invoke(_msg);
                } else {
                    SocketContent.Matchmaker_Reply cmdType;
                    if (!MyEnum.TryParseEnum(data.CMD, out cmdType)) {
                        WriteLog.LogErrorFormat("收到錯誤的命令類型: {0}", cmdType);
                        return;
                    }
                    switch (cmdType) {
                        case SocketContent.Matchmaker_Reply.CREATEROOM_REPLY:
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






    }
}
