using Cysharp.Threading.Tasks;
using HeroFishing.Main;
using Newtonsoft.Json.Linq;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Socket {
    public partial class GameConnector : MonoBehaviour {
        public static GameConnector Instance = null;

        #region 共用參數設定

        const float RETRY_INTERVAL_SECS = 3.0f; //重連間隔時間
        const int MAX_RETRY_TIMES = 3; //最大重連次數
        const float CONNECT_TIMEOUT_SECS = 60.0f; //連線超時時間60秒
        int CurRetryTimes = 0; //目前重試次數

        #endregion


        public void Init() {
            Instance = this;
        }


        void OnDisConnect() {
            WriteLog.LogColor("OnDisConnect", WriteLog.LogType.Connection);
        }

        public void CheckLobbyServerStatus(Action<bool, bool> _cb) {
            WriteLog.LogColor("CheckLobbyServerStatus", WriteLog.LogType.Connection);
        }

    }
}