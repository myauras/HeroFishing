using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Numerics;
using UnityEngine.Profiling;
using HeroFishing.Main;
using UnityEngine.SceneManagement;
using System;

namespace Scoz.Func {
    public partial class TestTool : MonoBehaviour {
        public static bool IsInit;
        public static TestTool Instance;
        [SerializeField] Text EnvVersion;
        public Text Text_FPS;
        public float InfoRefreshInterval = 0.5f;
        public Text VersionText;

        int FrameCount = 0;
        float PassTimeByFrames = 0.0f;
        float LastFrameRate = 0.0f;

        MyTimer InfoRefreshTimer = null;
        public Canvas MyCanvas { get; private set; }


        public Camera GetCamera() {
            return GetComponent<Camera>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Start() {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MyCanvas = GetComponent<Canvas>();
            MyCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            MyCanvas.sortingLayerName = "UI";
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
            GetComponent<RectTransform>().sizeDelta = new UnityEngine.Vector2(Screen.width, Screen.height);
            VersionText.text = "Ver: " + Application.version;

        }


        void GetNumber(int _n, Action<int> _callback) {
            _callback?.Invoke(_n);
        }

        void OnDestroy() {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        void OnLevelFinishedLoading(Scene _scene, LoadSceneMode _mode) {
            MyCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            MyCanvas.sortingLayerName = "UI";

        }

        public void UpdateEnvVersionText() {
            //if (FirebaseManager.MyFirebaseApp != null)
            //    EnvVersion.text = "Env: " + FirebaseManager.MyFirebaseApp.Options.ProjectId;
        }
        void InitTestTool() {
            IsInit = true;
            DontDestroyOnLoad(gameObject);

        }
        public static TestTool CreateNewInstance() {
            if (Instance != null) {
                WriteLog.Log("TestTool之前已經被建立了");
            } else {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Common/TestTool");
                GameObject go = Instantiate(prefab);
                go.name = "TestTool";
                Instance = go.GetComponent<TestTool>();
                Instance.InitTestTool();
            }
            return Instance;
        }

        void FPSCalc() {
            if (Text_FPS == null || !Text_FPS.isActiveAndEnabled)
                return;
            if (PassTimeByFrames < InfoRefreshInterval) {
                PassTimeByFrames += Time.deltaTime;
                FrameCount++;
            } else {
                //This code will break if you set your m_refreshTime to 0, which makes no sense.
                LastFrameRate = (float)FrameCount / PassTimeByFrames;
                FrameCount = 0;
                PassTimeByFrames = 0.0f;
            }
            Text_FPS.text = string.Format("FPS:{0}", Mathf.Round(LastFrameRate).ToString());
        }
        void Update() {
            FPSCalc();
            KeyDetector();
            if (InfoRefreshTimer != null)
                InfoRefreshTimer.RunTimer();
        }

    }
}
