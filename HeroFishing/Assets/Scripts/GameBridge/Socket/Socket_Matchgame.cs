using HeroFishing.Battle;
using HeroFishing.Main;
using HeroFishing.Socket.Matchgame;
using LitJson;
using Scoz.Func;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Socket {
    public partial class HeroFishingSocket {
        TcpClient TCP_MatchgameClient;
        UdpSocket UDP_MatchgameClient;
        ServerTimeSyncer TimeSyncer;
        string UDP_MatchgameConnToken;// 連Matchgame需要的Token，由AUTH_REPLY時取得

        Dictionary<Tuple<string, int>, Action<string>> CMDCallback = new Dictionary<Tuple<string, int>, Action<string>>();

        public int GetMatchgamePing() {
            if (TimeSyncer)
                return Mathf.RoundToInt(TimeSyncer.GetLantency() * 1000);
            return 0;
        }
        public void MatchgameDisconnect() {
            WriteLog.LogColor("MatchgameDisconnect", WriteLog.LogType.Connection);
        }
        void RegistrMatchgameCommandCB(Tuple<string, int> _cmdID, Action<string> _ac) {
            if (CMDCallback.ContainsKey(_cmdID)) {
                WriteLog.LogError("Command remain here should not happen.");
                CMDCallback.Remove(_cmdID);
            }
            CMDCallback.Add(_cmdID, _ac);
        }
        public void JoinMatchgame(string _realmToken, string _ip, int _port, Action<bool> _ac) {
            WriteLog.LogColor("JoinMatchgame", WriteLog.LogType.Connection);
            if (TCP_MatchgameClient != null) {
                WriteLog.LogColor($"JoinMatchgame時 TCP_MatchgameClient不為null, 關閉 {TCP_MatchgameClient}", WriteLog.LogType.Connection);
                TCP_MatchgameClient.Close();
            }
            TCP_MatchgameClient = new GameObject("TCP_MatchgameClient").AddComponent<TcpClient>();
            TCP_MatchgameClient.Init(_ip, _port);
            TCP_MatchgameClient.OnReceiveMsg += OnRecieveMatchgameTCPMsg;
            if (UDP_MatchgameClient != null) {
                UDP_MatchgameClient.Close();
                WriteLog.LogColor($"JoinMatchgame時 TCP_MatchgameClient不為null, 關閉 {TCP_MatchgameClient}", WriteLog.LogType.Connection);
            }
            UDP_MatchgameClient = new GameObject("TCP_MatchgameClient").AddComponent<UdpSocket>();
            UDP_MatchgameClient.Init(_ip, _port);


            TCP_MatchgameClient.StartConnect((bool connected) => {

                WriteLog.LogColor($"TCP_MatchgameClient connection: {connected}", WriteLog.LogType.Connection);
                if (!connected) {
                    _ac?.Invoke(false);
                    return;
                }
                if (TCP_MatchmakerClient != null) {
                    TCP_MatchmakerClient.OnReceiveMsg -= OnRecieveMatchmakerTCPMsg;
                    TCP_MatchmakerClient.Close();
                    WriteLog.LogColor($"JoinMatchgame成功後 TCP_MatchmakerClient不需要了關閉 {TCP_MatchmakerClient}", WriteLog.LogType.Connection);
                }
                SocketCMD<AUTH> cmd = new SocketCMD<AUTH>(new AUTH(_realmToken));

                int id = TCP_MatchgameClient.Send(cmd);
                if (id < 0) {
                    _ac?.Invoke(false);
                    return;
                }
                RegistrMatchgameCommandCB(new Tuple<string, int>(SocketContent.MatchgameCMDType.AUTH_REPLY.ToString(), id), (string msg) => {
                    SocketCMD<AUTH_REPLY> packet = LitJson.JsonMapper.ToObject<SocketCMD<AUTH_REPLY>>(msg);
                    _ac?.Invoke(packet.Content.IsAuth);
                    if (packet.Content.IsAuth) {
                        try {
                            //取得Matchgame Auth的回傳結果 UDP socket的ConnToken與遊戲房間的座位索引
                            WriteLog.LogColor($"Matchgame auth success! UDP_MatchgameConnToken: {UDP_MatchgameConnToken}", WriteLog.LogType.Connection);
                            UDP_MatchgameConnToken = packet.Content.ConnToken;
                            AllocatedRoom.Instance.SetPlayerIndex(packet.Content.Index);

                            //取得ConnToken後就能進行UDP socket連線
                            UDP_MatchgameClient.StartConnect(UDP_MatchgameConnToken, (bool connected) => {
                                WriteLog.LogColor($"UDP Is connected: {connected}", WriteLog.LogType.Connection);
                                if (connected)
                                    UDP_MatchgameClient.OnReceiveMsg += OnMatchgameUDPConnCheck;
                            });
                            UDP_MatchgameClient.RegistOnDisconnect(OnMatchgameUDPDisconnect);
                        } catch (Exception e) {
                            WriteLog.LogError($"UDP error: " + e);
                        }
                        if (TimeSyncer == null)
                            TimeSyncer = new GameObject("TimeSyncer").AddComponent<ServerTimeSyncer>();
                        TimeSyncer.StartCountTime();

                    }
                });
            });
            TCP_MatchgameClient.RegistOnDisconnect(OnMatchmakerDisconnect);


        }
        public void OnMatchgameUDPConnCheck(string _msg) {
            try {
                SocketCMD<UPDATE_UDP> packet = LitJson.JsonMapper.ToObject<SocketCMD<UPDATE_UDP>>(_msg);
                TimeSyncer.SycServerTime(packet.Content.ServerTime);
            } catch (Exception e) {
                WriteLog.LogError("Parse UDP Message with Error : " + e.ToString());
            }
        }
        public void OnMatchgameUDPDisconnect() {
            UDP_MatchgameClient.OnReceiveMsg -= OnMatchgameUDPConnCheck;
            //沒有timeout重連UDP
            if (UDP_MatchgameClient != null && UDP_MatchgameClient.CheckTimerInTime()) {
                UDP_MatchgameClient.Close();
                UDP_MatchgameClient = new GameObject("GameUdpSocket").AddComponent<UdpSocket>();
                var dbMatchgame = GamePlayer.Instance.GetMatchGame();
                if (dbMatchgame == null) { WriteLog.LogError("OnMatchgameUDPDisconnect時重連失敗，dbMatchgame is null"); return; }
                UDP_MatchgameClient.Init(dbMatchgame.IP, dbMatchgame.Port);
                UDP_MatchgameClient.StartConnect(UDP_MatchgameConnToken, (bool connected) => {
                    WriteLog.LogColor("OnMatchgameUDPDisconnect後重連結果 :" + connected, WriteLog.LogType.Connection);
                    if (connected)
                        UDP_MatchgameClient.OnReceiveMsg += OnMatchgameUDPConnCheck;
                    else {
                        this.MatchgameDisconnect();
                    }
                });
                UDP_MatchgameClient.RegistOnDisconnect(OnMatchgameUDPDisconnect);
            } else {
                WriteLog.LogError("OnUDPDisconnect");
                this.MatchgameDisconnect();
            }
        }

        public void TCPSend<T>(SocketCMD<T> cmd) where T : SocketContent {
            TCP_MatchgameClient.Send(cmd);
        }
        private void OnRecieveMatchgameTCPMsg(string _msg) {
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

                WriteLog.LogColorFormat("收到Server端資訊: {0}", WriteLog.LogType.Connection, _msg);
                SocketCMD<SocketContent> data = JsonMapper.ToObject<SocketCMD<SocketContent>>(_msg);
                Tuple<string, int> commandID = new Tuple<string, int>(data.CMD, data.PackID);
                if (CMDCallback.TryGetValue(commandID, out Action<string> _cb)) {
                    CMDCallback.Remove(commandID);
                    _cb?.Invoke(_msg);
                } else {
                    SocketContent.MatchgameCMDType cmdType;
                    if (!MyEnum.TryParseEnum(data.CMD, out cmdType)) {
                        WriteLog.LogErrorFormat("收到錯誤的命令類型: {0}", cmdType);
                        return;
                    }
                    switch (cmdType) {
                        case SocketContent.MatchgameCMDType.SPAWN:
                            var spawnPacket = LitJson.JsonMapper.ToObject<SocketCMD<SPAWN>>(_msg);
                            HandleSPAWN(spawnPacket);
                            break;
                        case SocketContent.MatchgameCMDType.ACTION_SETHERO_REPLY:
                            var setHeroPacket = LitJson.JsonMapper.ToObject<SocketCMD<ACTION_SETHERO_REPLY>>(_msg);
                            HandleSETHERO(setHeroPacket);
                            break;
                        case SocketContent.MatchgameCMDType.UPDATE_PLAYER_REPLY:
                            var updatePlaeyrPacket = LitJson.JsonMapper.ToObject<SocketCMD<UPDATE_PLAYER_REPLY>>(_msg);
                            HandleUpdatePlayer(updatePlaeyrPacket);
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

        void HandleSPAWN(SocketCMD<SPAWN> _packet) {
            if (SceneManager.GetActiveScene().name != MyScene.BattleScene.ToString()) return;
            if (BattleManager.Instance == null || BattleManager.Instance.MyMonsterScheduler == null) return;
            BattleManager.Instance.MyMonsterScheduler.EnqueueMonster(_packet.Content.MonsterIDs, _packet.Content.RouteID, _packet.Content.IsBoss);
        }

        void HandleSETHERO(SocketCMD<ACTION_SETHERO_REPLY> _packet) {
            if (SceneManager.GetActiveScene().name != MyScene.BattleScene.ToString()) return;
            if (BattleManager.Instance == null || BattleManager.Instance.MyMonsterScheduler == null) return;
        }
        void HandleUpdatePlayer(SocketCMD<UPDATE_PLAYER_REPLY> _packet) {
            if (SceneManager.GetActiveScene().name != MyScene.BattleScene.ToString()) return;
            if (BattleManager.Instance == null || BattleManager.Instance.MyMonsterScheduler == null) return;
        }

    }
}
