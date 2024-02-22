using Cysharp.Threading.Tasks;
using HeroFishing.Main;
using HeroFishing.Socket.Matchmaker;
using Newtonsoft.Json.Linq;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

namespace HeroFishing.Socket {
    public partial class GameConnector : MonoBehaviour {
        int CurRetryTimes = 0; //目前重試次數
        string TmpDBMapID;//暫時紀錄要建立或加入的DBMapID
        Action OnConnToMatchmakerFail;//連線Matchmaker失敗 callback
        Action OnCreateRoomFail;//建立Matchgame房間失敗 callback
        Action<CREATEROOM_TOCLIENT> OnMatchmakerCreated;//連線Matchmaker callback
        Action OnConnToMatchgame;//連線Matchgame callback
        Action OnJoinGameFail;//連線Matchgame失敗callback
        Action OnMatchgameDisconnected;//與Matchgame 斷線callback

        /// <summary>
        ///  1. 從DB取ip, port, 檢查目前Server狀態後傳入此function
        ///  2. 連線進Matchmaker後會驗證token, 沒問題會回傳成功並執行OnConnectEvent
        /// </summary>
        public async UniTask ConnToMatchmaker(string _dbMapID, Action _onConnToMatchmakerFail,Action _onCreateRoomFail, Action<CREATEROOM_TOCLIENT> _onMatchmakerCreated) {
            WriteLog.LogColor("Start ConnToMatchmaker", WriteLog.LogType.Connection);
            TmpDBMapID = _dbMapID;
            CurRetryTimes = 0;
            OnConnToMatchmakerFail = _onConnToMatchmakerFail;
            OnCreateRoomFail = _onCreateRoomFail;
            OnMatchmakerCreated = _onMatchmakerCreated;
            // 接Socket
            var gameSetting = GamePlayer.Instance.GetDBGameSettingDoc<DBGameSetting>(DBGameSettingDoc.GameState);
            Socket.CreateMatchmaker(gameSetting.MatchmakerIP, gameSetting.MatchmakerPort ?? 0);
            var token = await RealmManager.GetValidAccessToken();
            Socket.LoginToMatchmaker(token);

        }

        /// <summary>
        /// 登入配對伺服器成功時執行
        /// </summary>
        void OnLoginToMatchmaker() {
            WriteLog.LogColor("登入Matchmaker成功", WriteLog.LogType.Connection);
            CreateRoom();
        }

        /// <summary>
        /// 登入配對伺服器失敗
        /// </summary>
        async UniTask OnLoginToMatchmakerError() {
            // 連線失敗時嘗試重連
            CurRetryTimes++;
            if (CurRetryTimes > MAX_RETRY_TIMES || !InternetChecker.InternetConnected) {
                WriteLog.LogColorFormat("嘗試連線{0}次都失敗，糟糕了", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                OnConnToMatchmakerFail?.Invoke();
            } else {
                WriteLog.LogColorFormat("第{0}次連線失敗，{0}秒後嘗試重連: ", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                //連線失敗有可能是TOKEN過期 刷Token後再連
                var token = await RealmManager.GetValidAccessToken();
                Socket.LoginToMatchmaker(token);
            }
        }

        /// <summary>
        ///  1. 送Matchmaker(配對伺服器)來建立Matchgame(遊戲房)
        ///  2. 建立後會呼叫OnCreateRoom
        /// </summary>
        void CreateRoom() {
            CurRetryTimes = 0;
            Socket.CreateMatchmakerRoom(TmpDBMapID);
        }

        /// <summary>
        /// 收到建立房間結果回傳如果有錯誤就重連
        /// </summary>
        void OnCreateRoom(CREATEROOM_TOCLIENT _reply) {
            if (_reply == null) {
                OnCreateRoomFail?.Invoke();
                WriteLog.LogError("OnCreateRoom回傳的CREATEROOM_REPLY內容為null");
                return;
            }
            // 建立房間成功
            WriteLog.LogColorFormat("建立房間成功: ", WriteLog.LogType.Connection, DebugUtils.ObjToStr(_reply));
            OnMatchmakerCreated?.Invoke(_reply);
        }

        private void OnCreateRoomError(Exception _exception) {
            // 建立房間失敗
            if (_exception != null) {
                CurRetryTimes++;
                if (CurRetryTimes > MAX_RETRY_TIMES || !InternetChecker.InternetConnected) {
                    OnCreateRoomFail?.Invoke();
                } else {
                    WriteLog.LogColor("[GameConnector] 建立房間失敗 再試1次", WriteLog.LogType.Connection);
                    // 再試一次
                    DG.Tweening.DOVirtual.DelayedCall(RETRY_INTERVAL_SECS, () => {
                        Socket.CreateMatchmakerRoom(TmpDBMapID);
                    });
                }
                return;
            }
        }

        /// <summary>
        /// 個人測試模式(不使用Agones服務, 不會透過Matchmaker分配房間再把ip回傳給client, 而是直接讓client去連資料庫matchgame的ip)
        /// </summary>
        public void ConnectToMatchgameTestVer(int _id, string _heroSkinID, Action _onConnnectedAC, Action _onJoinGameFail, Action _onDisconnected) {
            AllocatedRoom.Instance.SetMyHero(_id, _heroSkinID); //設定本地玩家自己使用的英雄ID
            // 建立房間成功
            WriteLog.LogColor("個人測試模式連線Matchgame: ", WriteLog.LogType.Connection);
            var gameState = RealmManager.MyRealm.Find<DBGameSetting>(DBGameSettingDoc.GameState.ToString());
            //設定玩家目前所在遊戲房間的資料
            UniTask.Void(async () => {
                await AllocatedRoom.Instance.SetRoom_TestvVer("System", new string[4], gameState.MatchgameTestverMapID, gameState.MatchgameTestverRoomName, gameState.MatchgameTestverTcpIP, gameState.MatchgameTestverUdpIP, gameState.MatchgameTestverPort ?? 0, "");
                GameConnector.Instance.ConnToMatchgame(_onConnnectedAC, _onJoinGameFail, _onDisconnected);
            });
        }

        /// <summary>
        /// 確認DBMatchgame表被建立後會跳BattleScene並開始跑ConnToMatchgame開始連線到Matchgame
        /// </summary>
        public void ConnToMatchgame(Action _onConnected,Action _onJoinGameFail, Action _onDisconnected) {
            if (AllocatedRoom.Instance.InGame) return;
            OnConnToMatchgame = _onConnected;
            OnJoinGameFail = _onJoinGameFail;
            OnMatchgameDisconnected = _onDisconnected;

            AllocatedRoom.Instance.SetInGame(true);
            WriteLog.LogColor("DBMatchgame已建立好, 開始連線到Matchgame", WriteLog.LogType.Connection);
            UniTask.Void(async () => {
                var dbMatchgame = await GamePlayer.Instance.GetMatchGame();
                if (dbMatchgame == null) {
                    WriteLog.LogError("JoinMatchgame失敗，dbMatchgame is null");
                    OnJoinGameFail?.Invoke();
                    return;
                }
                JoinMatchgame(_onDisconnected).Forget(); //開始連線到Matchgame                                          
                PopupUI.CallSceneTransition(MyScene.BattleScene);//跳轉到BattleScene
            });
        }
        /// <summary>
        /// 加入Matchmage
        /// </summary>
        async UniTask JoinMatchgame(Action _onDisconnected) {
            var realmToken = await RealmManager.GetValidAccessToken();
            if (string.IsNullOrEmpty(AllocatedRoom.Instance.TcpIP) || string.IsNullOrEmpty(AllocatedRoom.Instance.UdpIP) || AllocatedRoom.Instance.Port == 0) {
                WriteLog.LogError("JoinMatchgame失敗，AllocatedRoom的IP或Port為null");
                OnJoinGameFail?.Invoke();
                return;
            }
            Socket.JoinMatchgame(_onDisconnected, realmToken, AllocatedRoom.Instance.TcpIP, AllocatedRoom.Instance.UdpIP, AllocatedRoom.Instance.Port);
        }

        void JoinGameSuccess() {
            OnConnToMatchgame?.Invoke();
            WriteLog.LogColor("JoinMatchgame success!", WriteLog.LogType.Connection);
        }

        void JoinGameFailed(Exception ex) {
            Debug.Log("JoinMatghgame failed: " + ex);
            OnJoinGameFail?.Invoke();
        }
    }
}