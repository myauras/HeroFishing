﻿using HeroFishing.Main;
using Newtonsoft.Json.Linq;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Socket {
    public class GameConnector : MonoBehaviour {
        static GameConnector instance = null;
        public static GameConnector Instance {
            get {
                if (instance == null)
                    instance = new GameObject("GameConnector").AddComponent<GameConnector>();
                return instance;
            }
        }
        const float RETRY_INTERVAL_SECS = 3.0f; //重連間隔時間
        const int MAX_RETRY_TIMES = 3; //最大重連次數
        const float CONNECT_TIMEOUT_SECS = 60.0f; //連線超時時間60秒
        int CurRetryTimes = 0; //目前重試次數

        event Action<bool, bool> OnConnectEvent; //連線Matchmaker(配房伺服器)回傳
        Action<bool> CreateRoomCB;//建立Matchgame(遊戲房)回傳

        void Start() {
            DontDestroyOnLoad(this);
        }

        /// <summary>
        ///  1. 從DB取ip, port, token並檢查目前Server狀態後傳入此function
        ///  2. 連線進Matchmaker後會驗證token, 沒問題會回傳成功
        /// </summary>
        public async void Run(string _ip, int _port, Action<bool, bool> _cb) {
            WriteLog.LogColor("Run", WriteLog.LogType.Connection);
            OnConnectEvent = _cb;
            OnConnectEvent += OnConnectToMatchmakerServer;
            CurRetryTimes = 0;
            HeroFishingSocket.GetInstance().RegistDisconnectCallback(OnDisConnect);
            await ConnectToMatchmakerServer(_ip, _port);
        }
        async Task ConnectToMatchmakerServer(string _ip, int _port) {
            WriteLog.LogColor("ConnectToMatchmakerServer", WriteLog.LogType.Connection);
            HeroFishingSocket.GetInstance().SetServerIP(_ip, _port);
            var token = await RealmManager.GetValidAccessToken();
            HeroFishingSocket.GetInstance().Login(token, OnLoginToMatchmakerServer);
        }
        /// <summary>
        /// 登入配對伺服器成功時執行
        /// </summary>
        async Task OnLoginToMatchmakerServer(string _token, bool _isLogin) {
            WriteLog.LogColor("OnLoginToMatchmakerServer", WriteLog.LogType.Connection);

            // 連線失敗時嘗試重連
            if (!_isLogin) {
                CurRetryTimes++;
                if (CurRetryTimes >= MAX_RETRY_TIMES || !InternetChecker.InternetConnected) {
                    OnConnectEvent?.Invoke(false, false);
                    WriteLog.LogColorFormat("嘗試連線{0}次都失敗，糟糕了><", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                } else {
                    WriteLog.LogColorFormat("第{0}次連線失敗，{0}秒後嘗試重連", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                    //連線失敗有可能是TOKEN過期 刷Token後再連
                    var token = await RealmManager.GetValidAccessToken();
                    HeroFishingSocket.GetInstance().Login(_token, OnLoginToMatchmakerServer);
                }
                return;
            }
            // 連上MatchmakerServer
            OnConnectEvent?.Invoke(true, true);
        }

        void OnDisConnect() {
            WriteLog.LogColor("OnDisConnect", WriteLog.LogType.Connection);
        }

        public void CheckLobbyServerStatus(Action<bool, bool> _cb) {
            WriteLog.LogColor("CheckLobbyServerStatus", WriteLog.LogType.Connection);
        }


        /// <summary>
        ///  1. 傳入mapID送Matchmaker(配對伺服器)來建立Matchgame(遊戲房)
        ///  2. 沒問題會回傳Matchgame(遊戲房)的ip與port
        ///  3. 成功後會跟Matchmaker斷線並連到Matchgame(遊戲房) 並回傳連線成功
        /// </summary>
        public void CreateRoom(string _dbMapID, Action<bool> _cb) {
            CreateRoomCB = _cb;
            HeroFishingSocket.GetInstance().CreateRoom(_dbMapID, OnCreateRoom);
        }


        void OnCreateRoom(string _dbMapID, bool isCreate, string errorMsg) {


            // 建立房間失敗
            if (!isCreate) {
                CurRetryTimes++;
                if (CurRetryTimes >= MAX_RETRY_TIMES || errorMsg == "NOT_FOUR_PLAYER" || !InternetChecker.InternetConnected) {
                    CreateRoomCB?.Invoke(isCreate);
                    PopupUI.ShowClickCancel(StringJsonData.GetUIString("ErrorCreateGame"), () => {
                    });
                } else {
                    WriteLog.LogColor("[GameConnector] 建立房間失敗 再試1次", WriteLog.LogType.Connection);
                    // 再試一次
                    DG.Tweening.DOVirtual.DelayedCall(RETRY_INTERVAL_SECS, () => {
                        HeroFishingSocket.GetInstance().CreateRoom(_dbMapID, OnCreateRoom);
                    });
                }
                return;
            }

            // 建立房間成功
            CreateRoomCB?.Invoke(isCreate);
        }


        void OnConnectRoom(bool isConnect) {
            WriteLog.LogColorFormat("[GameConnector] OnConnectRoom isConnect={0}", WriteLog.LogType.Connection, isConnect);
        }

        public void JoinGame(string serverIp, int port, Action<bool> _cb) {
            WriteLog.Log($"[GameConnector] JoinGame serverIp={serverIp} port={port}");
        }

        public void TryJoinLastRoom(Action<bool> _cb) {
            WriteLog.Log($"[GameConnector] TryJoinLastRoom");

        }



        void OnConnectToMatchmakerServer(bool _success, bool _serverMaintain) {
            WriteLog.Log($"[GameConnector] OnConnectToMatchmakerServer isSuccess={_success} isServerMatain={_serverMaintain}");
        }

        /// <summary>
        /// 離開快速房
        /// </summary>
        public void LeaveQuickRoom() {
            WriteLog.Log("[GameConnector] LeaveQuickRoom");
        }
    }
}