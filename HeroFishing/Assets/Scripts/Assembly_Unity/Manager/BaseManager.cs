using DG.Tweening.Core.Easing;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

namespace HeroFishing.Main {
    public class BaseManager : MonoBehaviour {
        [SerializeField] AssetReference GameManagerAsset;
        public static BaseManager Instance { get; private set; }
        public static bool IsInit { get; private set; } = false;
        public static BaseManager CreateNewInstance() {

            if (Instance != null) {
                WriteLog.Log("BaseManager之前已經被建立了");
            } else {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Common/BaseManager");
                GameObject go = Instantiate(prefab);
                go.name = "BaseManager";
                Instance = go.GetComponent<BaseManager>();
                Instance.Init();
            }
            return Instance;
        }

        void Init() {
            if (IsInit) return;
            IsInit = true;

            //建立Popup_Local
            PopupUI_Local.CreateNewInstance();
        }


        /// <summary>
        /// 下載Buindle, 下載好後之後都由GameAssembly的GameManager處理
        /// </summary>
        public void StartDownloadAddressable(Action _action) {
            AddressableManage.Instance.StartLoadAsset(() => {
                AddressablesLoader.GetAssetRef<GameObject>(GameManagerAsset, go => {
                    WriteLog.LogError("下載好GameManagerAsset");
                });
            });
        }

        /// <summary>
        /// 將自己的camera加入到目前場景上的MainCameraStack中
        /// </summary>
        public void AddCamStack(Camera _cam) {
            if (_cam == null) return;
            Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            if (mainCam == null) return;
            var cameraData = mainCam.GetUniversalAdditionalCameraData();
            if (cameraData == null) return;
            cameraData.cameraStack.Add(_cam);
        }
    }
}