using Cysharp.Threading.Tasks;
using HeroFishing.Main;
using HeroFishing.Socket.Matchgame;
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


        bool IsConnectomg = false;
        Action OnConnToMatchgame;//連線Matchgame callback
        Action OnJoinGameFail;//連線Matchgame失敗callback
        Action OnMatchgameDisconnected;//與Matchgame 斷線callback

        /// <summary>
        /// 確認DBMatchgame表被建立後會跳BattleScene並開始跑ConnToMatchgame開始連線到Matchgame
        /// </summary>
        public void ConnToMatchgame(Action _onConnected, Action _onJoinGameFail, Action _onDisconnected) {
            if (AllocatedRoom.Instance.CurGameState != AllocatedRoom.GameState.NotInGame) return;
            AllocatedRoom.Instance.SetGameState(AllocatedRoom.GameState.InGame);
            if (IsConnectomg) return;
            OnConnToMatchgame = _onConnected;
            OnJoinGameFail = _onJoinGameFail;
            OnMatchgameDisconnected = _onDisconnected;

            WriteLog.LogColor("DBMatchgame已建立好, 開始連線到Matchgame", WriteLog.LogType.Connection);
            UniTask.Void(async () => {
                var dbMatchgame = await GamePlayer.Instance.GetMatchGame();
                if (dbMatchgame == null) {
                    WriteLog.LogError("JoinMatchgame失敗，dbMatchgame is null");
                    OnJoinGameFail?.Invoke();
                    return;
                }
                IsConnectomg = true;
                JoinMatchgame().Forget(); //開始連線到Matchgame                                          
            });
        }
        /// <summary>
        /// 個人測試模式(不使用Agones服務, 不會透過Matchmaker分配房間再把ip回傳給client, 而是直接讓client去連資料庫matchgame的ip)
        /// </summary>
        public void ConnectToMatchgameTestVer(Action _onConnnectedAC, Action _onJoinGameFail, Action _onDisconnected) {
            // 建立房間成功
            WriteLog.LogColor("個人測試模式連線Matchgame: ", WriteLog.LogType.Connection);
            var gameState = RealmManager.MyRealm.Find<DBGameSetting>(DBGameSettingDoc.GameState.ToString());
            //設定玩家目前所在遊戲房間的資料
            UniTask.Void(async () => {
                await AllocatedRoom.Instance.SetRoom_TestvVer("System", new string[4], gameState.MatchgameTestverMapID, gameState.MatchgameTestverRoomName, gameState.MatchgameTestverTcpIP, gameState.MatchgameTestverUdpIP, gameState.MatchgameTestverPort ?? 0, "");
                ConnToMatchgame(_onConnnectedAC, _onJoinGameFail, _onDisconnected);
            });
        }
        /// <summary>
        /// 加入Matchmage
        /// </summary>
        async UniTask JoinMatchgame() {
            var realmToken = await RealmManager.GetValidAccessToken();
            if (string.IsNullOrEmpty(AllocatedRoom.Instance.TcpIP) || string.IsNullOrEmpty(AllocatedRoom.Instance.UdpIP) || AllocatedRoom.Instance.Port == 0) {
                WriteLog.LogError("JoinMatchgame失敗，AllocatedRoom的IP或Port為null");
                OnJoinGameFail?.Invoke();
                return;
            }
            Socket.JoinMatchgame(GameDisconnected, realmToken, AllocatedRoom.Instance.TcpIP, AllocatedRoom.Instance.UdpIP, AllocatedRoom.Instance.Port);
        }

        void JoinGameSuccess() {
            IsConnectomg = false;
            OnConnToMatchgame?.Invoke();
            WriteLog.LogColor("JoinMatchgame success!", WriteLog.LogType.Connection);
        }

        void JoinGameFailed(Exception ex) {
            IsConnectomg = false;
            Debug.Log("JoinMatghgame failed: " + ex);
            OnJoinGameFail?.Invoke();
        }

        void GameDisconnected() {
            IsConnectomg = false;
            OnMatchgameDisconnected?.Invoke();
        }

        /// <summary>
        /// 設定使用英雄ID
        /// </summary>
        public void SetHero(int _heroID, string _heroSkinID) {
            SocketCMD<SETHERO> cmd = new SocketCMD<SETHERO>(new SETHERO(_heroID, _heroSkinID));
            Socket.TCPSend(cmd);
        }
        /// <summary>
        /// 離開遊戲房
        /// </summary>
        public void LeaveRoom() {
            SocketCMD<LEAVE> cmd = new SocketCMD<LEAVE>(new LEAVE());
            Socket.TCPSend(cmd);
        }
        /// <summary>
        /// 攻擊
        /// </summary>
        public void Attack(int _attackID, string _spellJsonID, int _monsterIdx, bool _attackLock, Vector3 _attackPos, Vector3 _attackDir) {
            SocketCMD<ATTACK> cmd = new SocketCMD<ATTACK>(new ATTACK(_attackID, _spellJsonID, _monsterIdx, _attackLock, _attackPos, _attackDir));
            Socket.TCPSend(cmd);
        }
        /// <summary>
        /// 擊中
        /// </summary>
        public void Hit(int _attackID, int[] _monsterIdxs, string _spellJsonID) {
            SocketCMD<HIT> cmd = new SocketCMD<HIT>(new HIT(_attackID, _monsterIdxs, _spellJsonID));
            Socket.TCPSend(cmd);
        }
        /// <summary>
        /// 掉落施法
        /// </summary>
        public void DropSpell(int _dropSpellJsonID) {
            SocketCMD<DROPSPELL> cmd = new SocketCMD<DROPSPELL>(new DROPSPELL(_dropSpellJsonID));
            Socket.TCPSend(cmd);
        }

        public void DropSpell(int _attackID, int _dropSpellJsonID) {
            SocketCMD<DROPSPELL> cmd = new SocketCMD<DROPSPELL>(new DROPSPELL(_attackID, _dropSpellJsonID));
            Socket.TCPSend(cmd);
        }
        /// <summary>
        /// 場景狀態更新
        /// </summary>
        public void UpdateScene() {
            SocketCMD<UPDATESCENE> cmd = new SocketCMD<UPDATESCENE>(new UPDATESCENE());
            Socket.TCPSend(cmd);
        }

        public void Auto(bool isAuto) {
            SocketCMD<AUTO> cmd = new SocketCMD<AUTO>(new AUTO(isAuto));
            Socket.TCPSend(cmd);
        }
        public void LvUpSpell(int _idx) {
            SocketCMD<LVUPSPELL> cmd = new SocketCMD<LVUPSPELL>(new LVUPSPELL(_idx));
            Socket.TCPSend(cmd);
        }
        public void AddBot() {
            SocketCMD<ADDBOT> cmd = new SocketCMD<ADDBOT>(new ADDBOT());
            Socket.TCPSend(cmd);
        }
    }
}