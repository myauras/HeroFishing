using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System;
using Scoz.Func;

public interface INetworkInfoSource {
    void Init();
    int GetSignalStrength();
}

public class AndroidNetworkInfoSource : INetworkInfoSource {
    // 參考資料 https://stackoverflow.com/questions/60273473/how-do-i-know-the-methods-in-androidjavaobject-class-in-c-sharp-unity
    private bool _isInit;
    private const string PERMISSION_ACCESS_WIFI_STATE = "android.permission.ACCESS_WIFI_STATE";
    private AndroidJavaObject _wifiManager;

    // 取得所需的Object: Wifi Manager 以及 Wifi Info
    public void Init() {
        try {
            // 取得unity player 類別
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // 取得當前的activity (android的類別)
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            // 取得wifi相關服務，wifi manager
            _wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");

            if (!AndroidPermission.CheckPermission(PERMISSION_ACCESS_WIFI_STATE)) {
                Debug.LogError("no wifi access permission");
                return;
            }

            _isInit = true;
        }
        catch (Exception ex) {
            Debug.LogError("init wifi info access failed " + ex);
        }
    }

    /// <summary>
    /// 取得訊號強度
    /// </summary>
    /// <returns>訊號強度</returns>
    public int GetSignalStrength() {
        if (!_isInit) return 0;
        // 取得wifi相關資訊，wifi info，取得前需要同意，必須在Manifest.xml裡面添加，詳細可查詢Custom Main Manifest + add permission in manifest
        var wifiInfo = _wifiManager.Call<AndroidJavaObject>("getConnectionInfo");
        int rssi = wifiInfo.Call<int>("getRssi");
        return _wifiManager.CallStatic<int>("calculateSignalLevel", rssi, 4);
    }
}
