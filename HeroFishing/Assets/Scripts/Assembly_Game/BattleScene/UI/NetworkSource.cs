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
    // �ѦҸ�� https://stackoverflow.com/questions/60273473/how-do-i-know-the-methods-in-androidjavaobject-class-in-c-sharp-unity
    private bool _isInit;
    private const string PERMISSION_ACCESS_WIFI_STATE = "android.permission.ACCESS_WIFI_STATE";
    private AndroidJavaObject _wifiManager;

    // ���o�һݪ�Object: Wifi Manager �H�� Wifi Info
    public void Init() {
        try {
            // ���ounity player ���O
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // ���o��e��activity (android�����O)
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            // ���owifi�����A�ȡAwifi manager
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
    /// ���o�T���j��
    /// </summary>
    /// <returns>�T���j��</returns>
    public int GetSignalStrength() {
        if (!_isInit) return 0;
        // ���owifi������T�Awifi info�A���o�e�ݭn�P�N�A�����bManifest.xml�̭��K�[�A�Բӥi�d��Custom Main Manifest + add permission in manifest
        var wifiInfo = _wifiManager.Call<AndroidJavaObject>("getConnectionInfo");
        int rssi = wifiInfo.Call<int>("getRssi");
        return _wifiManager.CallStatic<int>("calculateSignalLevel", rssi, 4);
    }
}
