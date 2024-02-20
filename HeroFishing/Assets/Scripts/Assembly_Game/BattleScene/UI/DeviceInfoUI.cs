using HeroFishing.Main;
using HeroFishing.Socket;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeviceInfoUI : BaseUI {
    [Header("Battery")]
    [SerializeField]
    private Image _imgBattery;
    [SerializeField]
    private Image _imgBatteryCharge;
    [SerializeField]
    private Color _normalColor;
    [SerializeField]
    private Color _emptyColor;
    [Header("Network")]
    [SerializeField]
    private TextMeshProUGUI _txtPing;
    [SerializeField]
    private Image[] _imgWifi;

    private INetworkInfoSource _networkInfoSource;
    private bool _isInit;

    private const int UPDATE_WIFI_MS = 1000;
    private const int UPDATE_BATTERY_MS = 5000;

    private const string TXT_PING_CONTENT = "{0}ms";
    public override void RefreshText() {

    }

    public override void Init() {
        base.Init();
#if !UNITY_EDITOR && UNITY_ANDROID
        _networkInfoSource = new AndroidNetworkInfoSource();
#endif
        if (_networkInfoSource != null) {
            _networkInfoSource.Init();
            _isInit = true;
            Observable.Timer(TimeSpan.FromMilliseconds(UPDATE_WIFI_MS)).RepeatUntilDestroy(gameObject).Subscribe(_ => {
                int signalStrength = _networkInfoSource.GetSignalStrength();
                //Debug.Log(signalStrength);
                UpdateWifiImage(signalStrength);
                if (GameConnector.Connected) {
                    Ping ping = new Ping(AllocatedRoom.Instance.IP);
                    Observable.ReturnUnit().SkipWhile(_ => !ping.isDone).Timeout(TimeSpan.FromMilliseconds(UPDATE_WIFI_MS)).Subscribe(_ => {
                        Debug.Log($"ping {ping.time}");
                        _txtPing.text = string.Format(TXT_PING_CONTENT, ping);
                    }, ex => {
                        _txtPing.text = string.Format(TXT_PING_CONTENT, 999);
                    });
                }
            });
        }

        Observable.Timer(TimeSpan.FromMilliseconds(UPDATE_BATTERY_MS)).RepeatUntilDestroy(gameObject).Subscribe(_ => {
            UpdateBattery(SystemInfo.batteryStatus, SystemInfo.batteryLevel);
        });
    }

    private void UpdateWifiImage(int signalStrength) {
        for (int i = 0; i < _imgWifi.Length; i++) {
            int index = i;
            _imgWifi[index].gameObject.SetActive(signalStrength > index);
        }
    }

    private void UpdateBattery(BatteryStatus status, float level) {
        if (status == BatteryStatus.Unknown) return;
        _imgBattery.fillAmount = level;
        _imgBattery.color = status != BatteryStatus.Charging && level < 0.2f ? _emptyColor : _normalColor;
        _imgBatteryCharge.gameObject.SetActive(status == BatteryStatus.Charging);
    }
}
