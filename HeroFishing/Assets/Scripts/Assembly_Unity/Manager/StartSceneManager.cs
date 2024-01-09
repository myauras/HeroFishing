using Cysharp.Threading.Tasks;
using Realms.Sync;
using Scoz.Func;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace HeroFishing.Main {
    public class StartSceneManager : MonoBehaviour {
        [SerializeField] Canvas MyCanvas;

        [HeaderAttribute("==============Addressable Assets==============")]
        public AssetReference StartUIAsset;

        public static StartSceneManager Instance;
        List<AsyncOperationHandle> HandleList = new List<AsyncOperationHandle>();

        private void Start() {

            Instance = this;
            //建立遊戲管理者
            //GameManager.CreateNewInstance();
            //檢查網路
            PopupUI_Local.ShowLoading("Checking Internet");
            InternetChecker.StartCheckInternet(OnConnected);
        }


        //確認有網路後才會執行後續
        void OnConnected() {
            PopupUI_Local.HideLoading();
            PopupUI_Local.ShowLoading("Init Data");
            BaseManager.CreateNewInstance();//建立BaseManager
            BaseManager.Instance.StartDownloadAddressable(() => {

            });
        }


        private void OnDestroy() {
            for (int i = 0; i < HandleList.Count; i++) {
                if (HandleList[i].IsValid())
                    Addressables.Release(HandleList[i]);
            }
            Instance = null;
        }


        /// <summary>
        /// 載入UI
        /// </summary>
        void SpawnAddressableAssets() {

            DateTime now = DateTime.Now;
            //初始化ui            
            AddressablesLoader.GetPrefabByRef(StartUIAsset, (prefab, handle) => {
                WriteLog.LogFormat("載入StartUIAsset花費: {0}秒", (DateTime.Now - now).TotalSeconds);
                HandleList.Add(handle);
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(MyCanvas.transform);
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localScale = prefab.transform.localScale;
                RectTransform rect = go.GetComponent<RectTransform>();
                rect.offsetMin = Vector2.zero;//left、bottom
                rect.offsetMax = Vector2.zero;//right、top

            });
        }

    }
}