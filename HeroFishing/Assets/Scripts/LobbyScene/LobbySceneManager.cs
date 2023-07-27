using Scoz.Func;
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

                //以下繞過正式流程
                //載資源包
                GameManager.StartDownloadAddressable(() => {
                    InitLobby();
                });

                //FirebaseManager.Init(success => {
                //    if (success) {
                //        if (FirebaseManager.MyUser == null)//還沒在StartScene註冊帳戶就直接從其他Scene登入會報錯誤(通常還沒註冊帳戶就不會有玩家資料直接進遊戲會有問題)
                //            WriteLog.LogError("尚未註冊Firebase帳戶");

                //        //讀取Firestore資料
                //        FirebaseManager.LoadDatas(() => {
                //            //載資源包
                //            GameManager.StartDownloadAddressable(() => {
                //                InitLobby();
                //            });
                //        });
                //    }
                //});
            }
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
            //MyLoadingProgress.AddLoadingProgress("LobbyUI");//新增讀取中項目

            //DateTime now = DateTime.Now;
            ////初始化UI
            //Addressables.LoadAssetAsync<GameObject>(LobbyUIAsset).Completed += handle => {
            //    WriteLog.LogFormat("載入LobbyUI花費: {0}秒", (DateTime.Now - now).TotalSeconds);
            //    HandleList.Add(handle);
            //    GameObject go = Instantiate(handle.Result);
            //    go.transform.SetParent(MyCanvas.transform);
            //    go.transform.localPosition = handle.Result.transform.localPosition;
            //    go.transform.localScale = handle.Result.transform.localScale;
            //    RectTransform rect = go.GetComponent<RectTransform>();
            //    rect.offsetMin = Vector2.zero;//Left、Bottom
            //    rect.offsetMax = Vector2.zero;//Right、Top
            //    go.GetComponent<LobbyUI>().Init();
            //    MyLoadingProgress.FinishProgress("LobbyUI");//完成讀取UI
            //};
        }

        public void OnClick() {
            PopupUI.CallSceneTransition(MyScene.BattleScene);
        }


    }
}