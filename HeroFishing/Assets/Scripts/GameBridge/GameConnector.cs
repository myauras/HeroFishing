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
        private const float RETRY_INTERVAL = 3.0f;
        private const int MAX_RETRY_TIME = 10;
        private const float CONNECT_TIMEOUT = 60.0f;//1*60 1分鐘        
        private static GameConnector instance = null;
        public static GameConnector Instance {
            get {
                if (instance == null)
                    instance = new GameObject("GameConnector").AddComponent<GameConnector>();
                return instance;
            }
        }
        private bool isConnectGameRoom = false;
        private int reTryConnectTimes = 0;
        private string roomName = "";

        private event Action<bool, bool> OnConnectEvent;

        // 設定預設值
        private string gameServerIP = "192.168.0.121";
        private int gameServerPort = 7654;
        private Coroutine timeoutCheckCoroutine;
        private WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
        private bool isInMaJam = false;

        void Start() {
            DontDestroyOnLoad(this);
        }

        public void Init() {
            WriteLog.Log("[GameConnector] Init");
            //nothing just create object.
            isConnectGameRoom = false;
            isInMaJam = false;
            if (timeoutCheckCoroutine != null)
                StopCoroutine(timeoutCheckCoroutine);
            timeoutCheckCoroutine = StartCoroutine(CheckConnectTimeout());
        }

        private IEnumerator CheckConnectTimeout() {
            WriteLog.Log("[GameConnector] CheckConnectTimeout");
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
        /// <param name="_cb"></param>
        public void Run(Action<bool, bool> _cb) {
            WriteLog.Log("[GameConnector Run]");
            OnConnectEvent = _cb;
            OnConnectEvent += OnConnectFromLobby;
            isConnectGameRoom = false;
            reTryConnectTimes = 0;
            HeroFishingSocket.GetInstance().RegistDisconnectCallback(OnDisConnect);

            ConnectToLobbyServer();
        }


        private void OnDisConnect() {
            WriteLog.Log("[GameConnector] OnDisConnect");
        }

        public bool IsConnectGame() {
            WriteLog.Log("[GameConnector] IsConnectGame");
            return isConnectGameRoom;
        }

        public void CheckLobbyServerStatus(Action<bool, bool> _cb) {
            WriteLog.Log("[GameConnector] CheckLobbyServerStatus");
        }

        private void ConnectToLobbyServer() {
            WriteLog.Log("[GameConnector] ConnectToLobbyServer");
            HeroFishingSocket.GetInstance().SetServerIP("35.194.151.95", 32680);
            HeroFishingSocket.GetInstance().Login("scoz", OnLoginToLobbyServer);
        }

        /// <summary>
        /// 在
        /// </summary>
        /// <param name="isLogin"></param>
        private void OnLoginToLobbyServer(bool isLogin) {
            WriteLog.Log("[GameConnector] OnLoginToLobbyServer");

        }

        private void OnCreateRoom(bool isCreate, string errorMsg) {
            WriteLog.Log("[GameConnector] OnCreateRoom");

        }


        private void OnConnectRoom(bool isConnect) {
            WriteLog.Log($"[GameConnector] OnConnectRoom isConnect={isConnect}");
        }

        public void JoinGame(string serverIp, int port, Action<bool> _cb) {
            WriteLog.Log($"[GameConnector] JoinGame serverIp={serverIp} port={port}");
        }

        public void TryJoinLastRoom(Action<bool> _cb) {
            WriteLog.Log($"[GameConnector] TryJoinLastRoom");

        }



        private void OnConnectFromLobby(bool isSuccess, bool isServerMatain) {
            WriteLog.Log($"[GameConnector] OnConnectFromLobby isSuccess={isSuccess} isServerMatain={isServerMatain}");
        }

        /// <summary>
        /// 離開快速房
        /// </summary>
        public void LeaveQuickRoom() {
            WriteLog.Log("[GameConnector] LeaveQuickRoom");
        }
    }
}