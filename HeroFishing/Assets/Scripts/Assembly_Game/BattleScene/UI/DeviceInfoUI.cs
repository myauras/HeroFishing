using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeviceInfoUI : BaseUI {
    [SerializeField]
    private Image[] _imgWifi;

    private INetworkInfoSource _networkInfoSource;
    private bool _isInit;
    public override void RefreshText() {

    }

    public override void Init() {
        base.Init();
        Debug.Log("????");
#if !UNITY_EDITOR && UNITY_ANDROID
        _networkInfoSource = new AndroidNetworkInfoSource();
#endif
        Debug.Log($"network info source {_networkInfoSource != null}" );
        if (_networkInfoSource != null) {
            _networkInfoSource.Init();
            _isInit = true;
            Observable.Timer(TimeSpan.FromMilliseconds(1000)).RepeatUntilDestroy(gameObject).Subscribe(_ => {
                int signalStrength = _networkInfoSource.GetSignalStrength();
                Debug.Log(signalStrength);
                UpdateWifiImage(signalStrength);
            });
        }
    }

    private void UpdateWifiImage(int signalStrength) {
        for (int i = 0; i < _imgWifi.Length; i++) {
            int index = i;
            _imgWifi[index].gameObject.SetActive(signalStrength > index);
        }
    }
}
