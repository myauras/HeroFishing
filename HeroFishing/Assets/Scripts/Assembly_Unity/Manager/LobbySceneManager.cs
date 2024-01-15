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
        [HeaderAttribute("==============Addressable Assets==============")]
        public AssetReference GameManagerAsset;

        List<AsyncOperationHandle> HandleList = new List<AsyncOperationHandle>();

        public static LobbySceneManager Instance { get; private set; }

        void Start() {

            Instance = this;
            //建立遊戲管理者
            if (BaseManager.IsInit) {
                OnConnected();
            } else {
                BaseManager.CreateNewInstance();
                //檢查網路
                PopupUI_Local.ShowLoading("Checking Internet");
                InternetChecker_UnityAssembly.StartCheckInternet(OnConnected);
            }

        }

        //確認有網路後才會執行後續
        void OnConnected() {
            PopupUI_Local.ShowLoading("Init Data");
            BaseManager.Instance.StartDownloadAddressable(() => {
                SpawnAddressableAssets();
            });
        }

        private void OnDestroy() {
            Instance = null;
            for (int i = 0; i < HandleList.Count; i++) {
                if (HandleList[i].IsValid())
                    Addressables.Release(HandleList[i]);
            }
        }
        /// <summary>
        /// 載入UI
        /// </summary>
        void SpawnAddressableAssets() {

            DateTime now = DateTime.Now;
            //載入GameManager
            AddressablesLoader_UnityAssebly.GetPrefabByRef(GameManagerAsset, (prefab, handle) => {
                WriteLog_UnityAssembly.LogFormat("載入GameManagerAsset花費: {0}秒", (DateTime.Now - now).TotalSeconds);
                HandleList.Add(handle);
                GameObject go = Instantiate(prefab);
            });
        }


    }
}