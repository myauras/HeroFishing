using HeroFishing.Socket;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace HeroFishing.Main {
    public class StartSceneManager : MonoBehaviour {
        public Canvas MyCanvas;
        public StartSceneUI MyStartSceneUI;
        [SerializeField] Text VersionText;
        [SerializeField] AudioClip StartMusic;

        public static StartSceneManager Instance;
        List<AsyncOperationHandle> HandleList = new List<AsyncOperationHandle>();
        /// <summary>
        /// 是否第一次執行遊戲，第一次執行遊戲後會自動進大廳，之後透過從大廳的設定中點回到主介面就不會又自動進大廳了
        /// </summary>
        public static bool FirstTimeLaunchGame { get; private set; } = true;

        private void Start() {
            ShowInfo();//顯示資訊
            MyStartSceneUI.Init();
            Instance = this;
            //建立遊戲管理者
            GameManager.CreateNewInstance();
            //播放背景音樂主題曲，要在GameManager.CreateNewInstance()之後，因為AudioPlayer是在這之後才被初始化
            PlayMainMusic();
            //檢查網路
            PopupUI_Local.ShowLoading("Checking Internet");
            InternetChecker.SetOnConnectedAction(OnConnected);
            InternetChecker.StartCheckInternet();
        }
        public void SocketConnectTest() {
            GameConnector.Instance.Init();
            GameConnector.Instance.Run(OnConnectGame);
        }
        public void RealmInitTest() {
            RealmManager.NewApp();
            //RealmManager.AnonymousSignUp();
        }
        public void TestAtlasFunction() {
            RealmManager.CallAtlasFunc(RealmManager.AtlasFunc.Signup);
        }
        private void OnConnectGame(bool isSuccess, bool isMaintain) {
            WriteLog.Log("isSuccess=" + isSuccess);
        }
        /// <summary>
        /// 1. (玩家尚未登入) 顯示版本
        /// 2. (玩家登入) 顯示版本+玩家ID
        /// </summary>
        public void ShowInfo() {
            if (RealmManager.MyApp != null && RealmManager.MyApp.CurrentUser != null)
                VersionText.text = string.Format("Ver: {0} {1} " + Application.version, RealmManager.MyApp.CurrentUser.Id);
            else
                VersionText.text = string.Format("Ver: {0}" + Application.version);
        }
        public void PlayMainMusic() {
            AudioPlayer.StopAllMusic_static();
            if (StartMusic != null)
                AudioPlayer.PlayAudioByAudioClip(MyAudioType.Music, StartMusic, true);
        }

        //確認有網路後才會執行後續
        void OnConnected() {
            PopupUI_Local.ShowLoading("Init Data");

            ////初始化Realm
            RealmManager.NewApp();
            AuthChek();//登入狀態確認
        }
        /// <summary>
        /// 登入狀態確認
        /// 1. (還沒登入)打開UI介面，讓玩家選擇登入方式
        /// 2. (已登入且第一次開遊戲)開始取同步Realm上的資料，都取完後就開始載Addressable資源包，載完後進入大廳場景(在編輯器模式中，為了測試不會直接進大廳)
        /// 3. (已登入且是從大廳退回主介面)打開UI介面，讓玩家選擇回大廳,登出還是移除帳戶(Apple限定)
        /// </summary>
        void AuthChek() {
            PopupUI_Local.HideLoading();
            // 詢問IOS玩家是否要開啟透明追蹤(Appsflyer會用到)
#if APPSFLYER && UNITY_IOS && !UNITY_EDITOR
                         AppsFlyerManager.Inst.IOSAskATTUserTrack();
#endif


            if (RealmManager.MyApp.CurrentUser == null) {//玩家尚未登入
                WriteLog.LogColor("玩家尚未登入Realm", WriteLog.LogType.Realm);
                MyStartSceneUI.ShowUI(StartSceneUI.Condition.NotLogin);
            } else {//已經登入，就同步Realm上的資料

                //                //是否第一次執行遊戲，第一次執行遊戲後會自動進大廳，之後透過從大廳的設定中點回到主介面就不會又自動進大廳了
                //                if (FirstTimeLaunchGame) {
                //                    FirebaseManager.LoadDatas(() => {
                //                        WriteLog.LogFormat("玩家 {0} 已經登入", FirebaseManager.MyUser.UserId);
                //                        StartSceneManager.Instance.SetVersionText();//顯示下方文字
                //#if APPSFLYER
                //                                    // 設定玩家UID
                //                                    AppsFlyerManager.Inst.SetCustomerUserId(FirebaseManager.MyUser.UserId);
                //                                    // AppsFlyer紀錄玩家登入
                //                                    AppsFlyerManager.Inst.Login(FirebaseManager.MyUser.UserId);
                //#endif

                //#if !UNITY_EDITOR && FIREBASE_ANALYTICS
                //                                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                //                                // 設定Firebase UserId
                //                                FirebaseAnalytics.SetUserId(FirebaseManager.MyUser.UserId);
                //                                // 記錄登入事件
                //                                Parameter[] loginParameters = {
                //                                      new Parameter(FirebaseAnalytics.ParameterMethod, FirebaseManager.MyUser.UserId)
                //                                };
                //                                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin, loginParameters);
                //#endif

                //                        //如果是編輯器不直接轉場景(正式機才會直接進Lobby)
                //#if UNITY_EDITOR
                //                        MyStartSceneUI.ShowUI(StartSceneUI.Condietion.BackFromLobby_ShowLogoutBtn);
                //#else
                //                                    StartDownloadingAssetAndGoNextScene();
                //#endif
                //                    });


                //                } else {//如果是從大廳點設定回到主介面跑這裡，顯示登出按鈕與返回大廳按鈕
                //                    MyStartSceneUI.ShowUI(StartSceneUI.Condietion.BackFromLobby_ShowLogoutBtn);
                //                }



            }
        }


        private void OnDestroy() {
            for (int i = 0; i < HandleList.Count; i++) {
                if (HandleList[i].IsValid())
                    Addressables.Release(HandleList[i]);
            }
            Instance = null;
        }


        /// <summary>
        /// 1. 開始下載資源包
        /// 2. 載完後顯示準備開始遊戲文字
        /// 3. 切至下一個場景
        /// </summary>
        public void StartDownloadingAssetAndGoNextScene() {
            StartSceneUI.Instance?.SetMiddleText(StringJsonData.GetUIString("Login_DownloadAsset"));
            GameManager.StartDownloadAddressable(() => {//下載完資源包後執行


                //繞過正式流程
                FirstTimeLaunchGame = false;
                PopupUI.InitSceneTransitionProgress(0);
                PopupUI.CallSceneTransition(MyScene.LobbyScene);
                return;

                /// 根據是否能進行遊戲來執行各種狀態
                /// 1. 判斷玩家版本，若版本低於最低遊戲版本則會跳強制更新
                /// 2. 判斷玩家版本，若版本低於目前遊戲版本則會跳更新建議
                /// 3. 判斷Maintain是否為true，若為true則不在MaintainExemptPlayerUIDs中的玩家都會跳維護中
                /// 4. 判斷該玩家是否被Ban，不是才能進遊戲
                GameStateManager.Instance.StartCheckCanPlayGame(() => {
                    FirstTimeLaunchGame = false;
                    PopupUI.InitSceneTransitionProgress(0, "LobbyUILoaded");
                    PopupUI.CallSceneTransition(MyScene.LobbyScene);
                });

            });
        }


    }
}