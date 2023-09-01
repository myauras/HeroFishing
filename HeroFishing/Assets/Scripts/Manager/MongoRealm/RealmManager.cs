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

        public static void ClearApp() {
            if (MyRealm != null) {
                MyRealm.Dispose();
                MyRealm = null;
            }
            if (MyApp != null) MyApp = null;
        }
        /// <summary>
        /// 最初Realm初始化要先New一個Ream App
        /// </summary>
        public static App NewApp() {
            MyApp = App.Create(REALM_APPID_DIC[GameManager.CurVersion]); // 創建 Realm App
            DeviceManager.AddOnApplicationQuitAction(() => { ClearApp(); });
            return MyApp;
        }


    }
}
