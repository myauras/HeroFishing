using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System;
using HeroFishing.Main;
using UnityEngine.Rendering.Universal;

namespace Scoz.Func {
    public enum DataLoad {
        GameDic,
        FirestoreData,
        AssetBundle,
    }
    public class GameManager : MonoBehaviour {
        public static GameManager Instance;
        public static bool IsInit { get; private set; } = false;
        public AssetReference PopupUIAsset;
        public AssetReference ResourcePreSetterAsset;
        public AssetReference PostPocessingAsset;
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


        DateTime LastServerTime;
        DateTime LastClientTime;
        public DateTime NowTime {
            get {
                TimeSpan span = DateTime.Now - LastClientTime;
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
                return (int)((DateTime.Now - NowTime).TotalHours);
            }
        }

        public static void UnloadUnusedAssets() {
            if (!CDChecker.DoneCD("UnloadUnusedAssets", GameSettingJsonData.GetFloat(GameSetting.UnloadUnusedAssetsCD)))
                return;
            Resources.UnloadUnusedAssets();
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

        public void SetTime(DateTime _serverTime) {
            LastServerTime = _serverTime;
            LastClientTime = DateTime.Now;
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
            //建立GameTimer
            gameObject.AddComponent<GameTimer>().Init();
            //建立GameStateManager
            gameObject.AddComponent<GameStateManager>().Init();
            //建立CameraManager
            gameObject.AddComponent<CamManager>().Init();
            //建立UniTaskManager
            gameObject.AddComponent<UniTaskManager>().Init();
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
            //建立Popup_Local
            if (SceneManager.GetActiveScene().name == MyScene.StartScene.ToString())
                PopupUI_Local.CreateNewInstance();
            //建立影片播放器
            MyVideoPlayer.CreateNewVideoPlayer();
            //建立Debugger
#if !Release
            Debugger.CreateNewInstance();
#endif
            //建立AudioPlayer    
            AudioPlayer.CreateNewAudioPlayer();
            //建立AddressableManage
            AddressableManage.CreateNewAddressableManage();
            //※設定本機資料要放最後(要在取得本機GameSetting後以及AudioPlayer.CreateNewAudioPlayer之後
            GamePlayer.Instance.LoadLocoData();
        }
        /// <summary>
        /// 依序執行以下
        /// 1. 下載Bundle包
        /// 2. 將Bundle包中的json資料存起來(JsonDataDic)
        /// 3. 建立PopupUI
        /// 4. Callback
        /// </summary>
        public static void StartDownloadAddressable(Action _action) {
            AddressableManage.Instance.StartLoadAsset(() =>//下載AssetBundle
            {
                OnAddressableDownloaded();
                GameDictionary.LoadJsonDataToDic(() => { //載入Bundle的json資料
                    MyText.RefreshActivityTextsAndFunctions();//更新介面的MyTex
                    Instance.CreateAddressableUIs(() => { //產生PopupUI
                        _action?.Invoke();
                    });
                    Instance.CreateResourcePreSetter();//載入ResourcePreSetter
                });
            });
        }
        static void OnAddressableDownloaded() {

        }
        public void CreateAddressableUIs(Action _ac) {
            //載入PopupUI(這個UI東西較多會載較久，所以在載好前會先設定StartUI文字讓玩家不要覺得是卡住)
            if (SceneManager.GetActiveScene().name == MyScene.StartScene.ToString()) {
                StartSceneUI.Instance?.SetMiddleText(StringJsonData.GetUIString("Login_WaitingForStartScene"));
                PopupUI_Local.ShowLoading(StringJsonData.GetUIString("Login_WaitingForStartScene"));
            }

            Addressables.LoadAssetAsync<GameObject>(Instance.PopupUIAsset).Completed += handle => {
                PopupUI_Local.HideLoading();
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


        public void CreateResourcePreSetter() {
            Addressables.LoadAssetAsync<GameObject>(Instance.ResourcePreSetterAsset).Completed += handle => {
                GameObject go = Instantiate(handle.Result);
                ResourcePreSetter preSetter = go.GetComponent<ResourcePreSetter>();
                preSetter.Init();
            };
        }

        /// <summary>
        /// 將自己的camera加入到目前場景上的MainCameraStack中
        /// </summary>
        public void AddCamStack(Camera _cam) {
            Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            var cameraData = mainCam.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(_cam);
        }

    }
}
