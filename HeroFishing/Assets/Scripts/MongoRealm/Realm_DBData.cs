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
        public static void GetDatas() {
            var player = MyRealm.Find<DBPlayer>(MyApp.CurrentUser.Id);

            // 在這裡使用 player 物件
            if (player != null) {
                // 找到了相應的文件
                Debug.LogError("Id=" + player.ID);
                // ...
            } else {
                Debug.LogError("找不到");
                // 沒有找到相應的文件
            }
        }
    }
}
