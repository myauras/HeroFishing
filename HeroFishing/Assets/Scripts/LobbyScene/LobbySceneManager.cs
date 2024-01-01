using Cysharp.Threading.Tasks;
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
    public class LobbySceneManager : MonoBehaviour {
        public Canvas MyCanvas;
        [HeaderAttribute("==============Addressable Assets==============")]
        public AssetReference LobbyUIAsset;

        [HeaderAttribute("==============設定==============")]
        LoadingProgress MyLoadingProgress;//讀取進度，讀取完會跑FinishInitLobby()
        List<AsyncOperationHandle> HandleList = new List<AsyncOperationHandle>();

        public static LobbySceneManager Instance { get; private set; }

        void Start() {
            Instance = this;
            if (GameManager.IsInit) {
                InitLobby();
            } else {
                //建立遊戲管理者
                GameManager.CreateNewInstance();
                RealmManager.NewApp();
                if (RealmManager.MyApp.CurrentUser == null) {//玩家尚未登入
                    WriteLog.LogError("玩家尚未登入Realm");
                } else {//已經登入，就開始載包

                    PopupUI_Local.ShowLoading(StringJsonData.GetUIString("Loading"));
                    UniTask.Void(async () => {
                        await RealmManager.OnSignin();
                        RealmManager.OnDataLoaded();
                        PopupUI_Local.HideLoading();
                        SetAppsFlyer();
                        GameManager.StartDownloadAddressable(() => {
                            InitLobby();
                        });
                    });

                }

            }
        }

        /// <summary>
        /// 登入狀態確認
        /// 1. (還沒登入)打開UI介面，讓玩家選擇登入方式
        /// 2. (已登入且第一次開遊戲)開始取同步Realm上的資料，都取完後就開始載Addressable資源包，載完後進入大廳場景(在編輯器模式中，為了測試不會直接進大廳)
        /// 3. (已登入且是從大廳退回主介面)打開UI介面，讓玩家選擇回大廳,登出還是移除帳戶(Apple限定)
        /// </summary>
        void AuthChek() {
            PopupUI_Local.HideLoading();


            //※之後也要新增玩家註冊完但是初始化玩家資料失敗的流程(不太會發生 但要考慮這個可能性)
            //※之後也要新增玩家註冊完但是初始化玩家資料失敗的流程(不太會發生 但要考慮這個可能性)
            //※之後也要新增玩家註冊完但是初始化玩家資料失敗的流程(不太會發生 但要考慮這個可能性)
            //※之後也要新增玩家註冊完但是初始化玩家資料失敗的流程(不太會發生 但要考慮這個可能性)

            if (RealmManager.MyApp.CurrentUser == null) {//玩家尚未登入
                WriteLog.LogColor("玩家尚未登入Realm", WriteLog.LogType.Realm);

            } else {//已經登入，就開始載包並進入遊戲

                //是否第一次執行遊戲，第一次執行遊戲後會自動進大廳，之後透過從大廳的設定中點回到主介面就不會又自動進大廳了
                if (FirstTimeLaunchGame) {
                    PopupUI_Local.ShowLoading("Loading");
                    UniTask.Void(async () => {
                        await RealmManager.OnSignin();
                        RealmManager.OnDataLoaded();
                        PopupUI_Local.HideLoading();
                        StartSceneManager.Instance.ShowInfo();//顯示下方文字

                        //如果是Dev版本不直接轉場景(Dev版以外會直接進Lobby)
#if Dev
                        MyStartSceneUI.ShowUI(StartSceneUI.Condition.BackFromLobby_ShowLogoutBtn);
#else
                        StartDownloadingAssetAndGoNextScene();//開始載資源包並開始遊戲
#endif

                    });
                } else {//如果是從大廳點設定回到主介面跑這裡，顯示登出按鈕與返回大廳按鈕
                    MyStartSceneUI.ShowUI(StartSceneUI.Condition.BackFromLobby_ShowLogoutBtn);
                }

            }
        }

        public void SetAppsFlyer() {
#if APPSFLYER
                        // 設定玩家UID
                        AppsFlyerManager.Inst.SetCustomerUserId(RealmManager.MyApp.CurrentUser.Id);
                        // AppsFlyer紀錄玩家登入
                        AppsFlyerManager.Inst.Login(RealmManager.MyApp.CurrentUser.Id);
#endif
        }


        /// <summary>
        /// 大廳初始化
        /// </summary>
        public void InitLobby() {
            MyLoadingProgress = new LoadingProgress(LobbyUILoaded);
            SpawnAddressableAssets();
        }
        /// <summary>
        /// 大廳初始化完成時執行
        /// </summary>
        public void LobbyUILoaded() {
            PopupUI.FinishSceneTransitionProgress("LobbyUILoaded");


        }
        private void OnDestroy() {
            Instance = null;
            for (int i = 0; i < HandleList.Count; i++) {
                if (HandleList[i].IsValid())
                    Addressables.Release(HandleList[i]);
            }
        }
        void SpawnAddressableAssets() {
            MyLoadingProgress.AddLoadingProgress("LobbyUI");//新增讀取中項目

            DateTime now = DateTime.Now;
            //初始化ui            
            AddressablesLoader.GetPrefabByRef(LobbyUIAsset, (prefab, handle) => {
                WriteLog.LogFormat("載入LobbyUIAsset花費: {0}秒", (DateTime.Now - now).TotalSeconds);
                HandleList.Add(handle);
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(MyCanvas.transform);
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localScale = prefab.transform.localScale;
                RectTransform rect = go.GetComponent<RectTransform>();
                rect.offsetMin = Vector2.zero;//left、bottom
                rect.offsetMax = Vector2.zero;//right、top
                go.GetComponent<LobbySceneUI>().Init();
                MyLoadingProgress.FinishProgress("LobbyUI");//完成讀取ui

            });
        }


    }
}