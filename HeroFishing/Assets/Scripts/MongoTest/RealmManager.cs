using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using Realms.Sync;
using MongoDB.Bson;
using Realms.Exceptions;
using System;

public static class RealmManager 
{
    public static App MyApp { get; private set; }
    public static Realm MyRealm { get; private set; }

    /// <summary>
    /// 傳入的appID產生對應App，最初遊戲初始化要先New一個Ream App
    /// </summary>
    /// <param name="_appID">MongoDB App Services的AppID</param>
    public static void NewApp(string _appID) {
        MyApp = App.Create(_appID); // 創建 Realm App
    }
    /// <summary>
    /// 匿名註冊
    /// </summary>
    public static async void AnonymousSignUp() {
         await MyApp.LogInAsync(Credentials.Anonymous());
        OnSignin();
    }
    /// <summary>
    /// 信箱密碼註冊
    /// </summary>
    /// <param name="_email">信箱</param>
    /// <param name="_pw">密碼</param>
    public static async void EmailPWSignUp(string _email, string _pw) {
         await MyApp.LogInAsync(
            Credentials.EmailPassword(_email, _pw));
        OnSignin();
    }

    /// <summary>
    /// 玩家登入後執行
    /// </summary>
    public static void OnSignin() {
        SetConfiguration();
    }

    /// <summary>
    /// 設定FlexibleSyncConfg
    /// </summary>
    static async void SetConfiguration() {
        Debug.LogError("SetConfiguration: "+ MyApp.CurrentUser);

        var config = new FlexibleSyncConfiguration(MyApp.CurrentUser) {
            PopulateInitialSubscriptions = (realm) =>
            {
                var players = realm.All<Player>();
                realm.Subscriptions.Add(players);
            }
        };



        // The process will complete when all the user's items have been downloaded.
        MyRealm = await Realm.GetInstanceAsync(config);
        Debug.LogError("realm=" + MyRealm);
    }



    public static void GetData() {
        var playerId = new ObjectId("649152d10d942c2bf0ccafb5");
        var player = MyRealm.Find<Player>(playerId);

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

    public static async void CallFunc(string _funcName,params object[] _params) {
        Debug.LogError("CallFunc");
        try {
            var bsonValue = await MyApp.CurrentUser.Functions.CallAsync(_funcName, _params);
            Debug.LogError(bsonValue.ToString());
            var result = bsonValue.ToBsonDocument().ToDictionary();
            Debug.LogError("test");
        } catch(Exception _e) {
            Debug.LogError("呼叫Function錯誤: " + _funcName+"//// E: "+ _e);
        }
 
    }

}
