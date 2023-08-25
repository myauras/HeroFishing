using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using Realms.Sync;
using Scoz.Func;
using System;
using MongoDB.Bson;

namespace Service.Realms {

    public static partial class RealmManager {
        //環境版本對應Realm App ID
        static Dictionary<EnvVersion, string> REALM_APPID_DIC = new Dictionary<EnvVersion, string>() {
            { EnvVersion.Dev, "aurafortest-bikmm"},
            { EnvVersion.Test, "aurafortest-bikmm"},
            { EnvVersion.Release, "aurafortest-bikmm"},
        };
        public static App MyApp { get; private set; }
        public static Realm MyRealm { get; private set; }

        /// <summary>
        /// 最初Realm初始化要先New一個Ream App
        /// </summary>
        public static App NewApp() {
            MyApp = App.Create(REALM_APPID_DIC[GameManager.CurVersion]); // 創建 Realm App
            return MyApp;
        }


        public static void GetData() {
            var playerId = new ObjectId("649152d10d942c2bf0ccafb5");
            var player = MyRealm.Find<DBPlayer>(playerId);

            // 在這裡使用 player 物件
            if (player != null) {
                // 找到了相應的文件
                Debug.LogError("Name=" + player.Name);
                Debug.LogError("Id=" + player.ID);
                // ...
            } else {
                Debug.LogError("找不到");
                // 沒有找到相應的文件
            }
        }

        public static async void CallFunc(string _funcName, params object[] _params) {
            Debug.LogError("CallFunc");
            try {
                var bsonValue = await MyApp.CurrentUser.Functions.CallAsync(_funcName, _params);
                Debug.LogError(bsonValue.ToString());
                var result = bsonValue.ToBsonDocument().ToDictionary();
                Debug.LogError("test");
            } catch (Exception _e) {
                Debug.LogError("呼叫Function錯誤: " + _funcName + "//// E: " + _e);
            }

        }

    }
}
