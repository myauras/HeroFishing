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

        string TmpDBMapID;//暫時紀錄要建立或加入的DBMapID
        Action<bool> OnConnToMatchgameCB;//連線Matchgame callback

        /// <summary>
        ///  1. 從DB取ip, port, 檢查目前Server狀態後傳入此function
        ///  2. 連線進Matchmaker後會驗證token, 沒問題會回傳成功並執行OnConnectEvent
        /// </summary>
        public async UniTask ConnToMatchmaker(string _dbMapID, Action<bool> _cb) {
            WriteLog.LogColor("Start ConnToMatchmaker", WriteLog.LogType.Connection);
            CurRetryTimes = 0;
            TmpDBMapID = _dbMapID;
            OnConnToMatchgameCB = _cb;
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
            // 連上MatchmakerServer
            WriteLog.LogColor("登入Matchmaker成功", WriteLog.LogType.Connection);
            CreateRoom();
        }

        /// <summary>
        /// 登入配對伺服器失敗
        /// </summary>
        async UniTask OnLoginToMatchmakerError() {
            // 連線失敗時嘗試重連
            CurRetryTimes++;
            if (CurRetryTimes >= MAX_RETRY_TIMES || !InternetChecker.InternetConnected) {
                WriteLog.LogColorFormat("嘗試連線{0}次都失敗，糟糕了", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                OnConnToMatchgameCB?.Invoke(false);
            } else {
                WriteLog.LogColorFormat("第{0}次連線失敗，{0}秒後嘗試重連: ", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                //連線失敗有可能是TOKEN過期 刷Token後再連
                var token = await RealmManager.GetValidAccessToken();
                Socket.LoginToMatchmaker(token);
            }
        }

        /// <summary>
        ///  1. 送Matchmaker(配對伺服器)來建立Matchgame(遊戲房)
        ///  2. 建立後會開始偵聽DB資料, 並等待Server把Matchgame設定好後會建立DBMatchgame
        ///  3. 偵聽到DBMatchgame表被建立後會跑ConnToMatchgame開始連線到Matchgame
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
                WriteLog.LogError("OnCreateRoom回傳的CREATEROOM_REPLY內容為null");
                OnConnToMatchgameCB?.Invoke(false);
                return;
            }
            // 建立房間成功
            WriteLog.LogColorFormat("建立房間成功: ", WriteLog.LogType.Connection, DebugUtils.ObjToStr(_reply));
            //設定玩家目前所在遊戲房間的資料
            AllocatedRoom.Instance.SetRoom(_reply.CreaterID, _reply.PlayerIDs, _reply.DBMapID, _reply.DBMatchgameID, _reply.IP, _reply.Port, _reply.PodName);
            UniTask.Void(async () => {
                var bsonDoc = await RealmManager.Query_GetDoc(DBGameCol.matchgame.ToString(), _reply.DBMatchgameID);
                if (bsonDoc != null) {
                    // var dbMatchgame = new DBMatchgame(bsonDoc);
                    GameConnector.Instance.ConnToMatchgame();
                } else {
                    WriteLog.LogError("DBMatchgame 文件找不到, 請確認server有確實建立該文件");
                }
            });
        }

        private void OnCreateRoomError(Exception _exception) {
            // 建立房間失敗
            if (_exception != null) {
                CurRetryTimes++;
                if (CurRetryTimes >= MAX_RETRY_TIMES || _exception.Message == "NOT_FOUR_PLAYER" || !InternetChecker.InternetConnected) {
                    OnConnToMatchgameCB?.Invoke(false);
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
        public void ConnectToMatchgameTestVer() {
            // 建立房間成功
            WriteLog.LogColor("個人測試模式連線Matchgame: ", WriteLog.LogType.Connection);
            var gameState = RealmManager.MyRealm.Find<DBGameSetting>(DBGameSettingDoc.GameState.ToString());
            //設定玩家目前所在遊戲房間的資料
            AllocatedRoom.Instance.SetRoom("System", new string[4], gameState.MatchgameTestverMapID, gameState.MatchgameTestverRoomName, gameState.MatchgameTestverIP, gameState.MatchgameTestverPort ?? 0, "");
            UniTask.Void(async () => {
                var bsonDoc = await RealmManager.Query_GetDoc(DBGameCol.matchgame.ToString(), _reply.DBMatchgameID);
                if (bsonDoc != null) {
                    // var dbMatchgame = new DBMatchgame(bsonDoc);
                    GameConnector.Instance.ConnToMatchgame();
                } else {
                    WriteLog.LogError("DBMatchgame 文件找不到, 請確認server有確實建立該文件");
                }
            });
        }

        /// <summary>
        /// 偵聽到DBMatchgame表被建立後會跳BattleScene並開始跑ConnToMatchgame開始連線到Matchgame
        /// </summary>
        public void ConnToMatchgame() {
            if (AllocatedRoom.Instance.InGame) return;
            AllocatedRoom.Instance.SetInGame(true);
            WriteLog.LogColor("DBMatchgame已建立好, 開始連線到Matchgame", WriteLog.LogType.Connection);
            UniTask.Void(async () => {
                var dbMatchgame = await GamePlayer.Instance.GetMatchGame();
                if (dbMatchgame == null) {
                    WriteLog.LogError("JoinMatchgame失敗，dbMatchgame is null");
                    OnConnToMatchgameCB?.Invoke(false);
                    return;
                }
                JoinMatchgame().Forget(); //開始連線到Matchgame                                          
                PopupUI.CallSceneTransition(MyScene.BattleScene);//跳轉到BattleScene
            });
        }
        /// <summary>
        /// 加入Matchmage
        /// </summary>
        async UniTask JoinMatchgame() {
            var realmToken = await RealmManager.GetValidAccessToken();
            if (string.IsNullOrEmpty(AllocatedRoom.Instance.IP) || AllocatedRoom.Instance.Port == 0) {
                WriteLog.LogError("JoinMatchgame失敗，AllocatedRoom的IP或Port為null");
                OnConnToMatchgameCB?.Invoke(false);
                return;
            }
            Socket.JoinMatchgame(realmToken, AllocatedRoom.Instance.IP, AllocatedRoom.Instance.Port);
        }

        void JoinGameSuccess() {
            OnConnToMatchgameCB?.Invoke(true);
            WriteLog.LogColor("JoinMatchgame success!", WriteLog.LogType.Connection);
        }

        void JoinGameFailed(Exception ex) {
            Debug.Log("JoinMatghgame failed: " + ex);
            OnConnToMatchgameCB?.Invoke(false);
        }
    }
}