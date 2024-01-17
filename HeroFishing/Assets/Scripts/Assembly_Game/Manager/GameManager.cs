using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.Rendering.Universal;
using LitJson;
using Cysharp.Threading.Tasks;
using Service.Realms;
using HeroFishing.Main;
using HeroFishing.Socket;
using Unity.Entities;

namespace Scoz.Func {
    public enum DataLoad {
        GameDic,
        FirestoreData,
        AssetBundle,
    }
    public class GameManager : MonoBehaviour {
        public static GameManager Instance;
        public static bool IsInit { get; private set; } = false;
        static bool IsFinishedLoadAsset = false; //是否已經完成初始載包

        [HeaderAttribute("==============AddressableAssets==============")]
        [SerializeField] AssetReference PopupUIAsset;
        [SerializeField] AssetReference PostPocessingAsset;
        [SerializeField] AssetReference AddressableManageAsset;
        [SerializeField] AssetReference UICanvasAsset;
        [SerializeField] AssetReference UICamAsset;
        [SerializeField] AddressableManage MyAddressableManagerPrefab;
        [SerializeField] AssetReference ResourcePreSetterAsset;

        [Serializable] public class SceneUIAssetDicClass : SerializableDictionary<MyScene, AssetReference> { }
        [HeaderAttribute("==============場景對應入口UI==============")]
        [SerializeField] SceneUIAssetDicClass MySceneUIAssetDic;//字典對應UI字典

        [HeaderAttribute("==============遊戲設定==============")]
        public int TargetFPS = 60;
        public static EnvVersion CurVersion {//取得目前版本
            get {
#if Dev
                return EnvVersion.Dev;
#elif Test
            return EnvVersion.Test;
#elif Release
            return EnvVersion.Release;
#else
                return EnvVersion.Dev;
#endif
            }
        }

        DateTimeOffset LastServerTime;
        DateTimeOffset LastClientTime;
        public DateTimeOffset NowTime {
            get {
                TimeSpan span = DateTimeOffset.Now - LastClientTime;
                return LastServerTime.AddSeconds(span.TotalSeconds);
            }
        }
        /// <summary>
        /// 返回本機時間與UTC+0的時差
        /// </summary>
        public int LocoHourOffset {
            get {
                return (int)TimeZoneInfo.Local.BaseUtcOffset.TotalHours;
            }
        }
        /// <summary>
        /// 返回本機時間與Server的時差
        /// </summary>
        public int LocoHourOffsetToServer {
            get {
                return (int)((DateTimeOffset.Now - NowTime).TotalHours);
            }
        }

        public static void UnloadUnusedAssets() {
            if (!CDChecker.DoneCD("UnloadUnusedAssets", GameSettingJsonData.GetFloat(GameSetting.UnloadUnusedAssetsCD)))
                return;
            Resources.UnloadUnusedAssets();
        }

        private void Start() {
            Instance = this;
            Instance.Init();
        }


        public static GameManager CreateNewInstance() {

            if (Instance != null) {
                WriteLog.Log("GameManager之前已經被建立了");
            } else {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Common/GameManager");
                GameObject go = Instantiate(prefab);
                go.name = "GameManager";
                Instance = go.GetComponent<GameManager>();
                Instance.Init();
            }
            return Instance;
        }



        public void SetTime(DateTimeOffset _serverTime) {
            LastServerTime = _serverTime;
            LastClientTime = DateTimeOffset.Now;
            WriteLog.Log("Get Server Time: " + LastServerTime);
        }

        public void Init() {
            if (IsInit) return;
            Instance = this;
            IsInit = true;
            DontDestroyOnLoad(gameObject);
            //設定FPS與垂直同步
#if Dev
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 100;
#else
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFPS;
#endif
            RealmManager.NewApp();//初始化Realm
            //產生一個新玩家
            new GamePlayer();
            //建立FirebaseManager
            //gameObject.AddComponent<FirebaseManager>();
            //建立CoroutineJob
            gameObject.AddComponent<CoroutineJob>();
            //建立InternetChecker
            gameObject.AddComponent<InternetChecker>().Init();
            //建立DeviceManager
            gameObject.AddComponent<DeviceManager>();
            //建立TypeEffector
            gameObject.AddComponent<TextTypeEffector>().Init();
            //建立GameStateManager
            gameObject.AddComponent<GameStateManager>().Init();
            //建立CameraManager
            gameObject.AddComponent<CamManager>().Init();
            //建立UniTaskManager
            gameObject.AddComponent<UniTaskManager>().Init();
            //建立GameConnector
            gameObject.AddComponent<GameConnector>().Init();
            // 初始化FlutterManager
            FlutterManager.Init();
            //Permission請求
#if UNITY_ANDROID
            gameObject.AddComponent<AndroidPermission>().RequestLaunchPermissions();
            //gameObject.AddComponent<AndroidPermission>().RequestNotificationPermissions();
#endif
            //初始化文字取代工具
            StringReplacer.Init();
            //建立左右兩邊Banner
            //SideBanner.CreateNewInstance();
            //建立遊戲資料字典
            GameDictionary.CreateNewInstance();//先初始化字典因為這樣會預先載入本機String表與GameSetting，之後在addressable載入後會取代本來String跟GameSetting
            MyText.RefreshActiveTexts();//刷新文字

            //建立影片播放器
            MyVideoPlayer.CreateNewVideoPlayer();
            //建立TestTool
#if !Release
            TestTool.CreateNewInstance();
#endif
            //建立AudioPlayer    
            AudioPlayer.CreateNewAudioPlayer();
            //建立PoolManager
            PoolManager.CreateNewInstance();

            //※設定本機資料要放最後(要在取得本機GameSetting後以及AudioPlayer.CreateNewAudioPlayer之後
            GamePlayer.Instance.LoadLocoData();

            // 建立AddressableManage並開始載包
            StartDownloadAddressable();
        }

        public void CreateResourcePreSetter() {
            Addressables.LoadAssetAsync<GameObject>(Instance.ResourcePreSetterAsset).Completed += handle => {
                GameObject go = Instantiate(handle.Result);
                ResourcePreSetter preSetter = go.GetComponent<ResourcePreSetter>();
                preSetter.Init();
            };
        }


        public void OnAuthFinished(AuthType _authType) {
            // 初始化AppsFlyer
            SetAppsFlyer();
        }

        public void SetAppsFlyer() {
            // 詢問IOS玩家是否要開啟透明追蹤(Appsflyer會用到)
#if APPSFLYER && UNITY_IOS && !UNITY_EDITOR
             AppsFlyerManager.Inst.IOSAskATTUserTrack();
#endif

            //完成分析相關的註冊事件
#if APPSFLYER
                        // 設定玩家UID
                        AppsFlyerManager.Inst.SetCustomerUserId(RealmManager.MyApp.CurrentUser.Id);
                        // AppsFlyer紀錄玩家登入
                        AppsFlyerManager.Inst.Login(RealmManager.MyApp.CurrentUser.Id);
#endif

        }


        /// <summary>
        /// 依序執行以下
        /// 1. 下載Bundle包
        /// 2. 載入遊戲包Dll
        /// 3. 載入SceneCanvas
        /// 4. 將Bundle包中的json資料存起來(JsonDataDic)
        /// 5. 刷新文字介面(會刷新為StringJson的文字) 與 建立PopupUI 與 載入ResourcePreSetter
        /// 6. 根據所在場景產生場景UI
        /// </summary>
        public void StartDownloadAddressable() {
            var addressableManager = Instantiate(MyAddressableManagerPrefab);
            addressableManager.Init();
            AddressableManage.Instance.StartLoadAsset(async () => {
                await LoadAssembly();//載入GameDll
                AddressablesLoader.GetPrefabByRef(UICamAsset, (sceneUIPrefab, handle) => {//載入UICam
                    var camGo = Instantiate(sceneUIPrefab);
                    camGo.GetComponent<UICam>().Init();
                    Addressables.Release(handle);
                    Instance.CreateResourcePreSetter();//載入ResourcePreSetter
                    GameDictionary.LoadJsonDataToDic(() => { //載入Bundle的json資料
                        MyText.RefreshActivityTextsAndFunctions();//更新介面的MyTex
                        Instance.CreateAddressableUIs(() => { //產生PopupUI
                            IsFinishedLoadAsset = true;
                            SpawnSceneUI();
                        });
                    });

                });
            });
        }

        /// <summary>
        /// 根據所在Scene產生UI
        /// </summary>
        public static void SpawnSceneUI() {
            if (!IsFinishedLoadAsset) return;
            AddressablesLoader.GetPrefabByRef(Instance.UICanvasAsset, (canvasPrefab, handle) => {//載入UICanvas
                GameObject canvasGO = Instantiate(canvasPrefab);
                canvasGO.GetComponent<UICanvas>().Init();
                var myScene = MyEnum.ParseEnum<MyScene>(SceneManager.GetActiveScene().name);
                AddressablesLoader.GetPrefabByRef(Instance.MySceneUIAssetDic[myScene], (prefab, handle) => {
                    GameObject go = Instantiate(prefab);
                    go.transform.SetParent(canvasGO.transform);
                    go.transform.localPosition = prefab.transform.localPosition;
                    go.transform.localScale = prefab.transform.localScale;
                    RectTransform rect = go.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0, 0);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.offsetMin = rect.offsetMax = Vector2.zero;
                });
            });
        }
        /// <summary>
        /// 載入GameDll
        /// </summary>
        static async UniTask LoadAssembly() {
            WriteLog.LogColorFormat("開始載入Game Assembly", WriteLog.LogType.Addressable);
            var result = await AddressablesLoader.GetResourceByFullPath_Async<TextAsset>("Assets/AddressableAssets/Dlls/Game.dll.bytes");
            TextAsset dll = result.Item1;
            var gameAssembly = System.Reflection.Assembly.Load(dll.bytes);
            WriteLog.LogColorFormat("載入Game Assembly完成", WriteLog.LogType.Addressable);
        }

        public void CreateAddressableUIs(Action _ac) {
            //載入PopupUI(這個UI東西較多會載較久，所以在載好前會先設定StartUI文字讓玩家不要覺得是卡住)
            if (SceneManager.GetActiveScene().name == MyScene.StartScene.ToString()) {
                StartSceneUI.Instance?.SetMiddleText(StringJsonData.GetUIString("Login_WaitingForStartScene"));
                PopupUI.ShowLoading(StringJsonData.GetUIString("Login_WaitingForStartScene"));
            }

            Addressables.LoadAssetAsync<GameObject>(Instance.PopupUIAsset).Completed += handle => {
                PopupUI.HideLoading();
                GameObject go = Instantiate(handle.Result);
                go.transform.localPosition = Vector2.zero;
                go.transform.localScale = Vector3.one;
                RectTransform rect = go.GetComponent<RectTransform>();
                rect.offsetMin = Vector2.zero;//Left、Bottom
                rect.offsetMax = Vector2.zero;//Right、Top
                PopupUI ui = go.GetComponent<PopupUI>();
                ui.Init();
                go.SetActive(true);
                _ac?.Invoke();
            };
            //載入PostProcessingManager
            Addressables.LoadAssetAsync<GameObject>(Instance.PostPocessingAsset).Completed += handle => {
                GameObject go = Instantiate(handle.Result);
                go.transform.localPosition = Vector2.zero;
                go.transform.localScale = Vector3.one;
                PostProcessingManager manager = go.GetComponent<PostProcessingManager>();
                manager.Init();
                go.SetActive(true);
            };
        }

        /// <summary>
        /// 將指定camera加入到目前場景上的MainCameraStack中
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
