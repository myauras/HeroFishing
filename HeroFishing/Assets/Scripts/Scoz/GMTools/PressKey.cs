using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using HeroFishing.Main;
using SimpleJSON;
using Service.Realms;
using Realms;
using System.Linq;
using HeroFishing.Socket;

namespace Scoz.Func {
    public partial class TestTool : MonoBehaviour {

        [SerializeField] GameObject ToolGO;

        public static Animator MyAni;
        // Update is called once per frame
        void KeyDetector() {


            if (Input.GetKeyDown(KeyCode.Q)) {
                int[] monsterIdxs = new int[3] { 4, 5, 6 };
                GameConnector.Instance.Hit("0_1", monsterIdxs, "1_spell2");
            } else if (Input.GetKeyDown(KeyCode.W)) {
            } else if (Input.GetKeyDown(KeyCode.E)) {


            } else if (Input.GetKeyDown(KeyCode.R)) {


            } else if (Input.GetKeyDown(KeyCode.P)) {
                ToolGO?.SetActive(!ToolGO.activeSelf);


            } else if (Input.GetKeyDown(KeyCode.O)) {

            } else if (Input.GetKeyDown(KeyCode.I)) {

                var dbMatchgames = RealmManager.MyRealm.All<DBMatchgame>();//DBMatchgame在PopulateInitialSubscriptions中只取有自己在內的遊戲房所以直接用All不用再篩選
                WriteLog.LogColor("文件數量:" + dbMatchgames.Count(), WriteLog.LogType.Realm);

                var dbMaps = RealmManager.MyRealm.All<DBMap>();
                WriteLog.LogColor("文件數量:" + dbMaps.Count(), WriteLog.LogType.Realm);

                var dbPlayers = RealmManager.MyRealm.All<DBPlayer>();//DBMatchgame在PopulateInitialSubscriptions中只取有自己在內的遊戲房所以直接用All不用再篩選
                WriteLog.LogColor("文件數量:" + dbPlayers.Count(), WriteLog.LogType.Realm);

                var dbSettings = RealmManager.MyRealm.All<DBGameSetting>();//DBMatchgame在PopulateInitialSubscriptions中只取有自己在內的遊戲房所以直接用All不用再篩選
                WriteLog.LogColor("文件數量:" + dbSettings.Count(), WriteLog.LogType.Realm);

                var dbPlayerState = RealmManager.MyRealm.All<DBPlayerState>();
                WriteLog.LogColor("文件數量:" + dbPlayerState.Count(), WriteLog.LogType.Realm);
            }
        }

        public void OnModifyHP(int _value) {
        }
        public void OnModifySanP(int _value) {
        }
        public void ClearLocoData() {
            PlayerPrefs.DeleteAll();
        }

    }
}
