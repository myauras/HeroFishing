using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Socket {
    public class GameConnector : MonoBehaviour {
        const float RETRY_INTERVAL = 3.0f;
        const int MAX_RETRY_TIME = 3;
        const float CONNECT_TIMEOUT = 60.0f;//1*60 1分鐘        
        static GameConnector instance = null;
        public static GameConnector Instance {
            get {
                if (instance == null)
                    instance = new GameObject("GameConnector").AddComponent<GameConnector>();
                return instance;
            }
        }
        bool isConnectGameRoom = false;
        int RetryTimes = 0;

        event Action<bool, bool> OnConnectEvent;

        // 設定預設值
        string gameServerIP = "192.168.0.121";
        int gameServerPort = 7654;
        Coroutine timeoutCheckCoroutine;
        WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
        bool isInMaJam = false;

        void Start() {
            DontDestroyOnLoad(this);
        }

        public void Init() {
            WriteLog.LogColor("[GameConnector] Init", WriteLog.LogType.Connection);
            //nothing just create object.
            isConnectGameRoom = false;
            isInMaJam = false;
            if (timeoutCheckCoroutine != null)
                StopCoroutine(timeoutCheckCoroutine);
            timeoutCheckCoroutine = StartCoroutine(CheckConnectTimeout());
        }

        IEnumerator CheckConnectTimeout() {
            WriteLog.LogColor("[GameConnector] CheckConnectTimeout", WriteLog.LogType.Connection);
            float timer = 0;
            while (!isInMaJam) {
                timer += Time.fixedDeltaTime;
                if (timer > CONNECT_TIMEOUT) {
                    //讀取過久
                    if (SceneManager.GetActiveScene().name == MyScene.LobbyScene.ToString()) {
                        WriteLog.LogWarningFormat($"<color=#9b0000>[GameConnector] CheckConnectTimeout!</color>");
                    } else {
                        PopupUI.InitSceneTransitionProgress();
                        PopupUI.CallSceneTransition(MyScene.LobbyScene);
                        WriteLog.LogWarningFormat($"<color=#9b0000>[GameConnector] CheckConnectTimeout! SceneName={SceneManager.GetActiveScene().name}, Back to Lobbey Scene</color>");
                    }
                    yield break;
                }
                yield return fixedUpdate;
            }
            WriteLog.Log("連線進遊戲房了");
        }

        public void SetIsInMaJam() {
            WriteLog.Log("[GameConnector] SetIsInMaJam");
            isInMaJam = true;
        }

        public void StopCheckTimeout() {
            WriteLog.Log("[GameConnector] StopCheckTimeout");
            if (timeoutCheckCoroutine != null)
                StopCoroutine(timeoutCheckCoroutine);
            timeoutCheckCoroutine = null;
        }

        /*
         * Run =>
         * 檢查目前Server狀態並取IP跟Port =>  [CheckLobbyServerStatus] 
         * 連線進Lobby Server後 產出LobbyClient=> MaJam.Network.MaJamNetwork.GetInstance().SetServerIP()
         * FireStore取玩家Auth Token => FirebaseManager.GetToken()
         * 執行Lobby Server的登入, 驗證Auth是否正確 => MaJam.Network.MaJamNetwork.GetInstance().Login()
         * Lobby Server驗證Auth成功後 執行登入LobbyServer成功 準備創房間 => OnLoginToLobbyServer
         * 創房間 => MaJam.Network.MaJamNetwork.GetInstance().CreateRoom()
         */
        /// <summary>
        /// 開始跑連線相關流程
        /// </summary>
        public void Run(Action<bool, bool> _cb) {
            WriteLog.LogColor("[GameConnector Run]", WriteLog.LogType.Connection);
            OnConnectEvent = _cb;
            OnConnectEvent += OnConnectToMatchmakerServer;
            isConnectGameRoom = false;
            RetryTimes = 0;
            HeroFishingSocket.GetInstance().RegistDisconnectCallback(OnDisConnect);

            ConnectToMatchmakerServer();
        }


        void OnDisConnect() {
            WriteLog.LogColor("[GameConnector] OnDisConnect", WriteLog.LogType.Connection);
        }

        public bool IsConnectGame() {
            WriteLog.LogColor("[GameConnector] IsConnectGame", WriteLog.LogType.Connection);
            return isConnectGameRoom;
        }

        public void CheckLobbyServerStatus(Action<bool, bool> _cb) {
            WriteLog.LogColor("[GameConnector] CheckLobbyServerStatus", WriteLog.LogType.Connection);
        }

        void ConnectToMatchmakerServer() {
            WriteLog.LogColor("[GameConnector] ConnectToMatchmakerServer", WriteLog.LogType.Connection);
            HeroFishingSocket.GetInstance().SetServerIP("35.185.130.204", 32680);
            HeroFishingSocket.GetInstance().Login("scoz", OnLoginToMatchmakerServer);
        }

        /// <summary>
        /// 登入配對伺服器成功時執行
        /// </summary>
        /// <param name="isLogin"></param>
        void OnLoginToMatchmakerServer(bool isLogin) {
            WriteLog.LogColor("[GameConnector] OnLoginToLobbyServer", WriteLog.LogType.Connection);

            //連線失敗時嘗試重連
            if (!isLogin) {
                RetryTimes++;
                if (RetryTimes >= MAX_RETRY_TIME || !InternetChecker.InternetConnected) {
                    OnConnectEvent?.Invoke(false, false);
                } else {
                    WriteLog.LogColorFormat("[GameConnector] 連線失敗，{0}秒後嘗試重連", WriteLog.LogType.Connection, RETRY_INTERVAL);
                    DG.Tweening.DOVirtual.DelayedCall(RETRY_INTERVAL, () => {
                        //連線失敗有可能TOKEN過期 重要後再連
                        HeroFishingSocket.GetInstance().Login("重新連線token", OnLoginToMatchmakerServer);
                    });
                }
                return;
            }
            RetryTimes = 0;
            //連線成功就跟Server要求建立房間
            OnConnectEvent?.Invoke(true, true);

        }

        void test() {
            HeroFishingSocket.GetInstance().CreateRoom("mapID", OnCreateRoom);
        }


        void OnCreateRoom(bool isCreate, string errorMsg) {
            WriteLog.LogColor("[GameConnector] OnCreateRoom", WriteLog.LogType.Connection);

            // 建立房間失敗
            if (!isCreate) {
                RetryTimes++;
                if (RetryTimes >= MAX_RETRY_TIME || errorMsg == "NOT_FOUR_PLAYER" || !InternetChecker.InternetConnected) {
                    OnConnectEvent?.Invoke(false, false);
                    PopupUI.ShowClickCancel(StringJsonData.GetUIString("ErrorCreateGame"), () => {
                    });
                } else {
                    // 再試一次
                    DG.Tweening.DOVirtual.DelayedCall(RETRY_INTERVAL, () => {
                        HeroFishingSocket.GetInstance().CreateRoom("dbMapID", OnCreateRoom);
                    });
                }
                return;
            }

            // 建立房間成功

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