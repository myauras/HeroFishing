using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using Realms.Sync;
using Scoz.Func;
using System;
using MongoDB.Bson;
using System.Linq;

namespace Service.Realms {

    public static partial class RealmManager {
        public static void RegisterPropertyChangedNotify() {
            //var playerQuery = MyRealm.All<DBPlayer>();
            //var players = playerQuery.ToArray();
            //Debug.Log(players.Length);
            //foreach (var p in players) {
            //    Debug.Log(p.ID + "   " + p.Ban);
            //}
            var player = MyRealm.Find<DBPlayer>(MyApp.CurrentUser.Id);
            if (player != null) {
                player.PropertyChanged += (sender, e) => {
                    if (e.PropertyName == "Ban") {
                        if (player.Ban) {
                            Debug.LogError("Ban!!!!");
                        } else {
                            Debug.Log("UnBan!!!!");
                        }
                    }
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
