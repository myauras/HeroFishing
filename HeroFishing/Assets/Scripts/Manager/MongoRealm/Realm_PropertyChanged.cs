using Realms;
using Realms.Sync;
using Scoz.Func;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

namespace Service.Realms {

    public static partial class RealmManager {
        public static async Task RegisterRealmEvents() {
            RegisterConnectionStateChanges();
            await RegisterPropertyChangedNotifies();
        }

        /// <summary>
        /// 註冊Realm連線狀態通知
        /// </summary>
        public static void RegisterConnectionStateChanges() {
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
        public static async Task RegisterPropertyChangedNotifies() {
            //var playerQuery = MyRealm.All<DBPlayer>();
            //var players = playerQuery.ToArray();
            //Debug.Log(players.Length);
            //foreach (var p in players) {
            //    Debug.Log(p.ID + "   " + p.Ban);
            //}
            WriteLog.LogColor("註冊DB偵聽", WriteLog.LogType.Realm);
            var player = MyRealm.Find<DBPlayer>(MyApp.CurrentUser.Id);
            await MyRealm.WriteAsync(() => {
                player.DeviceUID = "Testset";
            });
            WriteLog.LogColor("寫入完成", WriteLog.LogType.Realm);
            if (player != null) {
                player.PropertyChanged += (sender, e) => {
                    var propertyName = e.PropertyName;
                    var propertyValue = player.GetType().GetProperty(propertyName).GetValue(player);
                    WriteLog.LogColorFormat("Changed field: {0}  Value: {1}", WriteLog.LogType.Realm, propertyName, propertyValue);
                    //if (e.PropertyName == "Ban") {
                    //    if (player.Ban) {
                    //        WriteLog.LogColor("Ban", WriteLog.LogType.Realm);
                    //    } else {
                    //        WriteLog.LogColor("UnBan", WriteLog.LogType.Realm);
                    //    }
                    //}
                };
            }

            //// 在這裡使用 player 物件
            //if (player != null) {
            //    // 找到了相應的文件
            //    Debug.LogError("Id=" + player.ID);
            //    Debug.LogError("Point=" + player.Point);
            //    // ...
            //} else {
            //    Debug.LogError("找不到");
            //    // 沒有找到相應的文件
            //}
        }
    }
}
