
using UnityEngine;
using UnityEditor;
using Service.Realms;
using System;
using System.Threading.Tasks;
using NSubstitute;
using Cysharp.Threading.Tasks;

public class RealmEditor : MonoBehaviour {
    [MenuItem("Scoz/Realm/Signout")]
    public static void SignoutRealmAuth() {
        RealmManager.NewApp(); // 創建 Realm App
        if (RealmManager.MyApp == null) { Debug.Log("無法取得Realm App"); return; }
        if (RealmManager.MyApp.CurrentUser == null) { Debug.Log("玩家尚未登入"); return; }
        UniTask.Void(async () => {
            try {
                Debug.Log("開始登出Realm...");
                await RealmManager.MyApp.CurrentUser.LogOutAsync();
                if (RealmManager.MyApp.CurrentUser == null) Debug.Log("登出Realm完成!");
                else Debug.LogError("登出失敗");
                RealmManager.ClearApp();
            } catch (Exception _e) {
                Debug.LogError("登出失敗:" + _e);
            }

        });

    }
}
