using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using Realms.Sync;
using Scoz.Func;
using System.Threading.Tasks;
using System.Linq;
using System;
using static UnityEditor.Progress;
using Cysharp.Threading.Tasks;

namespace Service.Realms {
    public static partial class RealmManager {

        /// <summary>
        /// 匿名註冊
        /// </summary>
        public static async Task AnonymousSignup() {
            if (MyApp == null) { WriteLog.LogError("尚未建立Realm App"); return; }
            try {
                await MyApp.LogInAsync(Credentials.Anonymous());
            } catch (Exception _e) {
                WriteLog.LogError("在AnonymousSignup時MyApp.LogInAsync發生錯誤: " + _e);
            }
            await OnSignin();
        }
        /// <summary>
        /// 信箱密碼註冊
        /// </summary>
        /// <param name="_email">信箱</param>
        /// <param name="_pw">密碼</param>
        public static async void EmailPWSignup(string _email, string _pw) {
            if (MyApp == null) { WriteLog.LogError("尚未建立Realm App"); return; }
            await MyApp.LogInAsync(
               Credentials.EmailPassword(_email, _pw));
            await OnSignin();
        }
        /// <summary>
        /// 取得AccessToken
        /// </summary>
        public static async Task<string> GetValidAccessToken() {
            if (MyApp == null || MyApp.CurrentUser == null) { WriteLog.LogErrorFormat("尚未建立Realm App，無法取得AccessToken"); return null; }
            await MyApp.CurrentUser.RefreshCustomDataAsync();
            WriteLog.LogColor("AccessToken:"+MyApp.CurrentUser.AccessToken, WriteLog.LogType.Realm);
            return MyApp.CurrentUser.AccessToken;
        }

        /// <summary>
        /// 取得Provider
        /// </summary>
        public static async Task<string> GetProvider() {
            if (MyApp == null || MyApp.CurrentUser == null) { WriteLog.LogErrorFormat("尚未建立Realm App，無法取得AccessToken"); return null; }
            await MyApp.CurrentUser.RefreshCustomDataAsync();
            var provider = MyApp.CurrentUser.Provider;
            return provider.ToString();
        }

        /// <summary>
        /// 玩家登入後都會執行這裡(不管是剛註冊後還是已註冊的玩家登入)
        /// </summary>
        public static async Task OnSignin() {
            WriteLog.LogColorFormat("Realm帳號登入: {0}", WriteLog.LogType.Realm, MyApp.CurrentUser);
            UniTask.Void(async () => {
                try {
                    await GetServerTime();
                } catch (Exception _e) {
                    WriteLog.LogError(_e);
                }
            });
            await SetConfiguration();
        }

        /// <summary>
        /// 向AtlasFunction取Server時間
        /// </summary>
        static async Task GetServerTime() {
            var serverTimeData = await CallAtlasFunc(AtlasFunc.GetServerTime, null);
            if (serverTimeData.TryGetValue("serverTime", out object _obj)) {
                //WriteLog.LogColor(_obj.ToString(), WriteLog.LogType.Realm);
                try {
                    DateTimeOffset utcDateTimeOffset = DateTimeOffset.Parse(_obj.ToString());
                    //DateTimeOffset localDateTimeOffset = utcDateTimeOffset.ToOffset(TimeSpan.FromHours(8));
                    GameManager.Instance.SetTime(utcDateTimeOffset);
                } catch (Exception _e) {
                    WriteLog.LogError("GetServerTime發生錯誤: " + _e);
                }
            } else {
                WriteLog.LogError("GetServerTime發生錯誤: Atlas Function回傳格式錯誤");
            }
        }

        /// <summary>
        /// 設定FlexibleSyncConfg
        /// </summary>
        static async Task SetConfiguration() {
            WriteLog.LogColorFormat("開始註冊Realm設定檔...", WriteLog.LogType.Realm);
            var config = new FlexibleSyncConfiguration(MyApp.CurrentUser) {
                PopulateInitialSubscriptions = (realm) => {
                    //註冊玩家自己的player資料
                    var players = realm.All<DBPlayer>().Where(i => i.ID == MyApp.CurrentUser.Id);
                    realm.Subscriptions.Add(players, new SubscriptionOptions() { Name = "MyPlayer" });
                    //註冊GameSetting資料
                    var gameSettings = realm.All<DBGameSetting>();
                    realm.Subscriptions.Add(gameSettings, new SubscriptionOptions() { Name = "GameSetting" });
                }
            };

            try {
                MyRealm = await Realm.GetInstanceAsync(config);
                //await MyRealm.SyncSession.WaitForDownloadAsync();
            } catch (Exception _e) {
                WriteLog.LogError("Realm 使用config來GetInstanceAsync時發生錯誤: " + _e);
                WriteLog.LogError("Realm設定檔註冊失敗");
                return;
            }
            WriteLog.LogColorFormat("Realm設定檔註冊完成", WriteLog.LogType.Realm);
        }

        public static async Task Signout() {
            await MyApp.CurrentUser.LogOutAsync();
            WriteLog.LogColorFormat("登出Realm帳戶", WriteLog.LogType.Realm);
        }


    }
}