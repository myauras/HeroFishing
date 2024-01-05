using UnityEngine;
using Service.Realms;
using System.Linq;
using HeroFishing.Socket;

namespace Scoz.Func {
    public partial class TestTool : MonoBehaviour {

        [SerializeField] GameObject ToolGO;

        public static Animator MyAni;
        int key = 0;
        // Update is called once per frame
        void KeyDetector() {


            if (Input.GetKeyDown(KeyCode.Q)) {
                int[] monsterIdxs = new int[1] { 1 };
                key++;
                GameConnector.Instance.Hit(key, monsterIdxs, "1_attack");
            } else if (Input.GetKeyDown(KeyCode.W)) {
                GameConnector.Instance.Attack(1, "1_attack", 2);
            } else if (Input.GetKeyDown(KeyCode.E)) {
                GameConnector.Instance.Hit(1, new int[1] { 0 }, "1_attack");

            } else if (Input.GetKeyDown(KeyCode.R)) {
                GameConnector.Instance.DropSpell(4);

            } else if (Input.GetKeyDown(KeyCode.P)) {
                GameConnector.Instance.UpdateScene();
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
            } else if (Input.GetKeyDown(KeyCode.L)) {
                GameConnector.Instance.UpdateScene();
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
