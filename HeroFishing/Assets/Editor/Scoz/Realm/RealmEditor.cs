
using UnityEngine;
using UnityEditor;
using Service.Realms;
using System;
using System.Threading.Tasks;
using NSubstitute;

public class RealmEditor : MonoBehaviour {
    [MenuItem("Scoz/Realm/Signout")]
    public static void SignoutRealmAuth() {
        try {
            RealmManager.NewApp(); // 創建 Realm App
            if (RealmManager.MyApp == null) { Debug.Log("無法取得Realm App"); return; }
            if (RealmManager.MyApp.CurrentUser == null) { Debug.Log("玩家尚未登入"); return; }
            Task.Run(async () => {
                Debug.Log("開始登出Realm...");
                await RealmManager.MyApp.CurrentUser.LogOutAsync();
            });
            //等待X秒鐘後判斷是否登出了
            Task.Run(async () => {
                await Task.Delay(TimeSpan.FromSeconds(2));
                if (RealmManager.MyApp.CurrentUser == null) Debug.Log("登出Realm完成!");
                else Debug.LogError("登出失敗");
                RealmManager.ClearApp();
            });

        } catch (Exception _e) {
            Debug.LogError("SignoutRealmAuth 發生錯誤: " + _e);
        }

    }
}
