using HeroFishing.Main;
using Realms;
using Realms.Sync;
using Scoz.Func;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Service.Realms {

    public static partial class RealmManager {
        /// <summary>
        /// 訂閱Matchgame(遊戲房)資料庫
        /// </summary>
        public static void Subscribe_Matchgame() {

            //註冊Matchgame資料
            var dbPlayer = GamePlayer.Instance.GetDBPlayerDoc<DBPlayerState>(DBPlayerCol.player);
            if (dbPlayer == null) return;
            WriteLog.LogColorFormat("訂閱Matchgame資料 ID:{0}", WriteLog.LogType.Realm, dbPlayer.InMatchgameID);
            var dbMatchgames = MyRealm.All<DBMatchgame>().Where(i => i.ID == dbPlayer.InMatchgameID);
            MyRealm.Subscriptions.Add(dbMatchgames, new SubscriptionOptions() { Name = "MyMatchgame" });
        }
        public static void Unsubscribe_Matchgame() {
            MyRealm.Subscriptions.Remove("MyMatchgame");
        }
    }
}
