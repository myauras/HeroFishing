using LitJson;
using Scoz.Func;
using System;
using System.Reflection;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace HeroFishing.Main {
    public class BaseManager : MonoBehaviour {
        [SerializeField] AssetReference GameManagerAsset;
        public static BaseManager Instance { get; private set; }
        public static bool IsInit { get; private set; } = false;
        public static BaseManager CreateNewInstance() {

            //在每一個場景的開使都會先呼叫BaseManager的CreateNewInstance
            //如果還沒初始化過(Instance為null)就會跑正式流程: 建立BaseManager > 下載資源包 > 建立GameManager
            //如果已經初始化過(Instance不為null)就會跳果載包等流程直接透過反射來呼叫GameManager的SpawnSceneUI方法

            if (Instance != null) {
                CallGameManagerFunc("SpawnSceneUI");
            } else {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Common/BaseManager");
                GameObject go = Instantiate(prefab);
                go.name = "BaseManager";
                Instance = go.GetComponent<BaseManager>();
                Instance.Init();
            }
            return Instance;
        }

        /// <summary>
        /// 呼叫GameAssembly的GameManager的靜態方法
        /// </summary>
        static void CallGameManagerFunc(string _func) {
            Assembly gameAssembly = Assembly.Load("Game");
            Type gameManager = gameAssembly.GetType("Scoz.Func.GameManager");
            MethodInfo spawnFunc = gameManager.GetMethod(_func);
            spawnFunc.Invoke(null, null);
        }

        void Init() {
            if (IsInit) return;
            IsInit = true;
            DontDestroyOnLoad(gameObject);
            SpawnSceneObjs();//生成場景限定
            SetJsonMapper();//設定LiteJson的JsonMapper            
            gameObject.AddComponent<CoroutineJob_UnityAssembly>();//建立CoroutineJob
            //建立AddressableManage並生成GameManager
            AddressableManage_UnityAssembly.CreateNewAddressableManage();
            StartDownloadAddressablesAndSpawnGameManager();
        }
        /// <summary>
        /// 生成場景限定
        /// </summary>
        void SpawnSceneObjs() {
            var myScene = MyEnum_UnityAssembly.ParseEnum<MyScene>(SceneManager.GetActiveScene().name);
            switch (myScene) {
                case MyScene.StartScene:
                    //建立Popup_Local
                    PopupUI_Local.CreateNewInstance();
                    //建立InternetChecker
                    gameObject.AddComponent<InternetChecker_UnityAssembly>().Init();
                    break;
                case MyScene.LobbyScene:
                    break;
                case MyScene.BattleScene:
                    break;
            }
        }

        public void SetJsonMapper() {
            JsonMapper.RegisterImporter<int, long>((int value) => {
                return (long)value;
            });
        }

        /// <summary>
        /// 下載Buindle, 下載好後之後產生 GameManager, 之後都由GameAssembly的GameManager處理
        /// </summary>
        void StartDownloadAddressablesAndSpawnGameManager() {
            PopupUI_Local.ShowLoading(StringJsonData_UnityAssembly.GetUIString("DataLoading"));
            AddressableManage_UnityAssembly.Instance.StartLoadAsset(() => {
                AddressablesLoader_UnityAssebly.GetAssetRef<GameObject>(GameManagerAsset, gameManagerPrefab => {
                    Instantiate(gameManagerPrefab);
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