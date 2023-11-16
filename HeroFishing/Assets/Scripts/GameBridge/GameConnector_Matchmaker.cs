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
            HeroFishingSocket.GetInstance().NewMatchmakerTCPClient(gameSetting.MatchmakerIP, gameSetting.MatchmakerPort ?? 0);
            var token = await RealmManager.GetValidAccessToken();
            HeroFishingSocket.GetInstance().LoginToMatchmaker(token, OnLoginToMatchmaker);

        }

        /// <summary>
        /// 登入配對伺服器成功時執行
        /// </summary>
        async UniTask OnLoginToMatchmaker(bool _success) {

            // 連線失敗時嘗試重連
            if (!_success) {
                CurRetryTimes++;
                if (CurRetryTimes >= MAX_RETRY_TIMES || !InternetChecker.InternetConnected) {
                    WriteLog.LogColorFormat("嘗試連線{0}次都失敗，糟糕了", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                    OnConnToMatchgameCB?.Invoke(false);
                } else {
                    WriteLog.LogColorFormat("第{0}次連線失敗，{0}秒後嘗試重連", WriteLog.LogType.Connection, CurRetryTimes, RETRY_INTERVAL_SECS);
                    //連線失敗有可能是TOKEN過期 刷Token後再連
                    var token = await RealmManager.GetValidAccessToken();
                    HeroFishingSocket.GetInstance().LoginToMatchmaker(token, OnLoginToMatchmaker);
                }
                return;
            }
            // 連上MatchmakerServer
            WriteLog.LogColor("登入Matchmaker成功", WriteLog.LogType.Connection);
            CreateRoom();
        }



        /// <summary>
        ///  1. 送Matchmaker(配對伺服器)來建立Matchgame(遊戲房)
        ///  2. 建立後會開始偵聽DB資料, 並等待Server把Matchgame設定好後會建立DBMatchgame
        ///  3. 偵聽到DBMatchgame表被建立後會跑ConnToMatchgame開始連線到Matchgame
        /// </summary>
        void CreateRoom() {
            CurRetryTimes = 0;
            HeroFishingSocket.GetInstance().CreateMatchmakerRoom(TmpDBMapID, OnCreateRoom);
        }

        /// <summary>
        /// 收到建立房間結果回傳如果有錯誤就重連
        /// </summary>
        void OnCreateRoom(CREATEROOM_REPLY _reply, string _erroMsg) {

            // 建立房間失敗
            if (!string.IsNullOrEmpty(_erroMsg)) {
                CurRetryTimes++;
                if (CurRetryTimes >= MAX_RETRY_TIMES || _erroMsg == "NOT_FOUR_PLAYER" || !InternetChecker.InternetConnected) {
                    OnConnToMatchgameCB?.Invoke(false);
                } else {
                    WriteLog.LogColor("[GameConnector] 建立房間失敗 再試1次", WriteLog.LogType.Connection);
                    // 再試一次
                    DG.Tweening.DOVirtual.DelayedCall(RETRY_INTERVAL_SECS, () => {
                        HeroFishingSocket.GetInstance().CreateMatchmakerRoom(TmpDBMapID, OnCreateRoom);
                    });
                }
                return;
            }
            if (_reply == null) {
                WriteLog.LogError("OnCreateRoom回傳的CREATEROOM_REPLY內容為null");
                OnConnToMatchgameCB?.Invoke(false);
                return;
            }
            // 建立房間成功
            WriteLog.LogColor("建立房間成功", WriteLog.LogType.Connection);
            //設定玩家目前所在遊戲房間的資料並開始偵聽DBMatchgame(偵聽到DBMatchgame好後會自動去連Matchgame socket)
            AllocatedRoom.Instance.SetRoom(_reply.CreaterID, _reply.PlayerIDs, _reply.DBMapID, _reply.DBMatchgameID, _reply.IP, _reply.Port, _reply.PodName);
        }

        /// <summary>
        /// 偵聽到DBMatchgame表被建立後會跳BattleScene並開始跑ConnToMatchgame開始連線到Matchgame
        /// </summary>
        public void ConnToMatchgame() {
            WriteLog.LogColor("DBMatchgame已建立好, 開始連線到Matchgame", WriteLog.LogType.Connection);
            var dbMatchgame = GamePlayer.Instance.GetMatchGame();
            if (dbMatchgame == null) {
                WriteLog.LogError("JoinMatchgame失敗，dbMatchgame is null");
                OnConnToMatchgameCB?.Invoke(false);
                return;
            }
            JoinMatchgame().Forget(); //開始連線到Matchgame
            //跳轉到BattleScene
            PopupUI.CallSceneTransition(MyScene.BattleScene);
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
            HeroFishingSocket.GetInstance().JoinMatchgame(realmToken, AllocatedRoom.Instance.IP, AllocatedRoom.Instance.Port, success => {
                OnConnToMatchgameCB?.Invoke(success);
                if (success) {
                    WriteLog.LogColor("JoinMatchgame success!", WriteLog.LogType.Connection);
                } else WriteLog.LogError("JoinMatchgame failed");
            });
        }
    }
}