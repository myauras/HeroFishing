using UnityEngine;
using Service.Realms;
using System.Linq;
using HeroFishing.Socket;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using HeroFishing.Main;
using System.Security.Cryptography;

namespace Scoz.Func {
    public partial class TestTool : MonoBehaviour {

        [SerializeField] GameObject ToolGO;

        public static Animator MyAni;
        int key = 0;

        [ContextMenu("Snap Shot")]
        public void SnapShot() {
            string date = DateTime.Now.ToString("MMdd-HHmmss");
            string path = $"Assets/CaptureScreenshot_{date}.png";
            Debug.LogError("存圖片至路徑: " + path);
            ScreenCapture.CaptureScreenshot(path);
        }
        // Update is called once per frame
        void KeyDetector() {


            if (Input.GetKeyDown(KeyCode.Q)) {
                DropManager.Instance.AddDrop(0, 4);

                //int[] monsterIdxs = new int[1] { 1 };
                //key++;
                //GameConnector.Instance.Hit(key, monsterIdxs, "1_attack");
            } else if (Input.GetKeyDown(KeyCode.T)) {
                SnapShot();
                //Action connFunc = null;
                //AllocatedRoom.Instance.SetMyHero(1, "1_1"); //設定本地玩家自己使用的英雄ID
                //if (SceneManager.GetActiveScene().name != MyScene.BattleScene.ToString())
                //    PopupUI.CallSceneTransition(MyScene.BattleScene);//跳轉到BattleScene
                //PopupUI.ShowLoading(StringJsonData.GetUIString("Loading"));
                //connFunc = () => GameConnector.Instance.ConnectToMatchgameTestVer(() => {
                //    PopupUI.HideLoading();
                //    GameConnector.Instance.SetHero(1, "1_1"); //送Server玩家使用的英雄ID
                //}, () => {
                //    WriteLog.LogError("連線遊戲房失敗");
                //}, () => {
                //    if (AllocatedRoom.Instance.CurGameState == AllocatedRoom.GameState.Playing) {
                //        WriteLog.LogError("需要斷線重連");
                //        connFunc();
                //    }
                //});
                //connFunc();
            } else if (Input.GetKeyDown(KeyCode.W)) {
                GameConnector.Instance.DropSpell(4);
            } else if (Input.GetKeyDown(KeyCode.E)) {
                GameConnector.Instance.LvUpSpell(0);

            } else if (Input.GetKeyDown(KeyCode.R)) {
                GameConnector.Instance.DropSpell(4);

            } else if (Input.GetKeyDown(KeyCode.P)) {
                GameConnector.Instance.UpdateScene();
            } else if (Input.GetKeyDown(KeyCode.O)) {

            } else if (Input.GetKeyDown(KeyCode.I)) {

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
