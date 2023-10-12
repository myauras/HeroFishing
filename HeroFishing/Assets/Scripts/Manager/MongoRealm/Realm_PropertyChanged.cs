using HeroFishing.Main;
using Realms;
using Realms.Sync;
using Scoz.Func;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Service.Realms {

    public static partial class RealmManager {

        /// <summary>
        /// 玩家資料已經取到後執行
        /// </summary>
        public static void OnDataLoaded() {
            try {
                GamePlayer.Instance.InitDBPlayerDocs();//初始化玩家DB資料
                GamePlayer.Instance.InitDBGameSettingDcos();//初始化遊戲設定DB資料
                RegisterRealmEvents();//註冊Realm事件    
                GameManager.Instance.AddComponent<GameTimer>().Init();//建立GameTimer
            } catch (Exception _e) {
                WriteLog.LogError(_e);
            }
        }

        /// <summary>
        /// 註冊Realm事件
        /// </summary>
        static void RegisterRealmEvents() {
            RegisterConnectionStateChanges();
            RegisterPropertyChanges();
        }

        /// <summary>
        /// 註冊Realm連線狀態通知
        /// </summary>
        public static void RegisterConnectionStateChanges() {
            WriteLog.LogColor("註冊Realm連線狀態變化通知", WriteLog.LogType.Realm);
            try {
                var session = MyRealm.SyncSession;
                session.PropertyChanged += (sender, e) => {
                    if (e.PropertyName == nameof(Session.ConnectionState)) {
                        var session = (Session)sender;
                        var state = session.ConnectionState;
                        switch (state) {
                            case ConnectionState.Connecting://連線Realm中
                                WriteLog.LogColor("連線Realm中....", WriteLog.LogType.Realm);
                                break;
                            case ConnectionState.Connected://連上Realm
                                WriteLog.LogColor("已連上Realm", WriteLog.LogType.Realm);
                                break;
                            case ConnectionState.Disconnected://與Realm斷線
                                WriteLog.LogColor("與Realm斷線", WriteLog.LogType.Realm);
                                break;
                            default:
                                // 不應該出現其他狀態
                                WriteLog.LogColorFormat("RegisterConnectionStateChanges接收到未定義的連線狀態: {0}", WriteLog.LogType.Realm, state);
                                break;
                        }
                    }
                };
            } catch (Exception _e) {
                Console.WriteLine(_e.Message);
            }
        }
        /// <summary>
        /// 註冊Realm文件異動通知
        /// </summary>
        public static void RegisterPropertyChanges() {
            WriteLog.LogColor("註冊Realm文件異動通知", WriteLog.LogType.Realm);
            RegisterPropertyChanges_MyPlayer();
        }

        /// <summary>
        /// 玩家文件通知
        /// </summary>
        static void RegisterPropertyChanges_MyPlayer() {
            var player = GamePlayer.Instance.GetDBPlayerDoc<DBPlayer>(DBPlayerCol.player);
            if (player != null) {
                player.PropertyChanged += (sender, e) => {
                    var propertyName = e.PropertyName;
                    var propertyValue = player.GetType().GetProperty(propertyName).GetValue(player);
                    GameStateManager.Instance.InGameCheckCanPlayGame();
                    WriteLog.LogColorFormat("Changed field: {0}  Value: {1}", WriteLog.LogType.Realm, propertyName, propertyValue);
                };
            }
        }
    }
}
