using Cysharp.Threading.Tasks;
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

        List<AsyncOperationHandle> HandleList = new List<AsyncOperationHandle>();

        public static LobbySceneManager Instance { get; private set; }

        void Start() {
            //Instance = this;
            //if (GameManager.IsInit) {
            //    InitLobby();
            //} else {
            //    //建立遊戲管理者
            //    GameManager.CreateNewInstance();
            //    RealmManager.NewApp();
            //    if (RealmManager.MyApp.CurrentUser == null) {//玩家尚未登入
            //        WriteLog.LogError("玩家尚未登入Realm");
            //    } else {//已經登入，就開始載包

            //        PopupUI_Local.ShowLoading(StringJsonData.GetUIString("Loading"));
            //        UniTask.Void(async () => {
            //            await RealmManager.OnSignin();
            //            RealmManager.OnDataLoaded();
            //            PopupUI_Local.HideLoading();
            //            GameManager.StartDownloadAddressable(() => {
            //                InitLobby();
            //            });
            //        });

            //    }

            //}
        }




        /// <summary>
        /// 大廳初始化
        /// </summary>
        public void InitLobby() {
            //MyLoadingProgress = new LoadingProgress(LobbyUILoaded);
            SpawnAddressableAssets();
        }
        /// <summary>
        /// 大廳初始化完成時執行
        /// </summary>
        public void LobbyUILoaded() {


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
            ////初始化ui            
            //AddressablesLoader.GetPrefabByRef(LobbyUIAsset, (prefab, handle) => {
            //    WriteLog.LogFormat("載入LobbyUIAsset花費: {0}秒", (DateTime.Now - now).TotalSeconds);
            //    HandleList.Add(handle);
            //    GameObject go = Instantiate(prefab);
            //    go.transform.SetParent(MyCanvas.transform);
            //    go.transform.localPosition = prefab.transform.localPosition;
            //    go.transform.localScale = prefab.transform.localScale;
            //    RectTransform rect = go.GetComponent<RectTransform>();
            //    rect.offsetMin = Vector2.zero;//left、bottom
            //    rect.offsetMax = Vector2.zero;//right、top
            //    go.GetComponent<LobbySceneUI>().Init();
            //    MyLoadingProgress.FinishProgress("LobbyUI");//完成讀取ui

            //});
        }


    }
}