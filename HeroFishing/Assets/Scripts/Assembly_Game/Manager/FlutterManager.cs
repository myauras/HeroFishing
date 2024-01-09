using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlutterUnityIntegration;
using Scoz.Func;

namespace HeroFishing.Main {
    public class FlutterManager {
        public static void Init() {
            UnityMessageManager.Instance.OnMessage += OnMsg;
        }
        public static void SendMsg(string _msg) {
            _msg = "[UNITY] " + _msg;
            UnityMessageManager.Instance.SendMessageToFlutter(_msg);
        }
        public static void OnMsg(string _msg) {
            WriteLog.LogColorFormat("收到Flutter: {0}", WriteLog.LogType.Flutter, _msg);
        }

    }
}