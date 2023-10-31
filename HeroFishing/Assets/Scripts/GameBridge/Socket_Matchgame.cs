using HeroFishing.Main;
using HeroFishing.Socket.Matchgame;
using Scoz.Func;
using System;
using System.Collections.Generic;
using UnityEngine;

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
            TCP_MatchgameClient.OnReceiveMsg += OnRecieveTCPMsg;
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
                    TCP_MatchmakerClient.OnReceiveMsg -= OnRecieveTCPMsg;
                    TCP_MatchmakerClient.Close();
                    WriteLog.LogColor($"JoinMatchgame成功後 TCP_MatchmakerClient不需要了關閉 {TCP_MatchmakerClient}", WriteLog.LogType.Connection);
                }
                SocketCMD<AUTH> cmd = new SocketCMD<AUTH>(new AUTH(_realmToken));

                int id = TCP_MatchgameClient.Send(cmd);
                if (id < 0) {
                    _ac?.Invoke(false);
                    return;
                }
                RegistrMatchgameCommandCB(new Tuple<string, int>(SocketContent.Matchgame_Reply.AUTH_REPLY.ToString(), id), (string msg) => {
                    SocketCMD<AUTH_REPLY> packet = LitJson.JsonMapper.ToObject<SocketCMD<AUTH_REPLY>>(msg);
                    _ac?.Invoke(packet.Content.IsAuth);
                    if (packet.Content.IsAuth) {
                        try {
                            UDP_MatchgameConnToken = packet.Content.ConnToken;
                            WriteLog.LogColor($"Matchgame auth success! UDP_MatchgameConnToken: {UDP_MatchgameConnToken}", WriteLog.LogType.Connection);
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
    }
}
