using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace HeroFishing.Battle {
    public class BattleSceneManager : MonoBehaviour {
        public Canvas MyCanvas;
        [SerializeField] BattleManager MyBattleManager;
        [HeaderAttribute("==============Addressable Assets==============")]
        public AssetReference BattleUIAsset;
        public AssetReference SpellIndicatorAsset;


        [HeaderAttribute("==============設定==============")]
        public int HeroID;
        public string HeroSkin;
        [SerializeField]
        private bool _isSpellTest;
        public bool IsSpellTest => _isSpellTest;
        LoadingProgress MyLoadingProgress;//讀取進度，讀取完會跑FinishInitLobby()



        public static BattleSceneManager Instance { get; private set; }


        void Start() {
            Instance = this;

            if (GameManager.IsInit) {
                InitBattleScene();
            } else {
                //建立遊戲管理者
                GameManager.CreateNewInstance();

                //以下繞過正式流程
                //載資源包
                GameManager.StartDownloadAddressable(() => {
                    InitBattleScene();
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

        public void OnDestroy() {
            Instance = null;
            if (World.DefaultGameObjectInjectionWorld != null) {
                // 呼叫clear all system去清理所有entity
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var entity = entityManager.CreateEntity();
                entityManager.AddComponent<ClearAllTag>(entity);
            }
        }



        /// <summary>
        /// 戰場初始化
        /// </summary>
        public void InitBattleScene() {
            MyLoadingProgress = new LoadingProgress(BattleUILoaded);
            SpawnAddressableAssets();

            MyBattleManager.Init();
            UniTaskManager.StartTask("SetBattleCam", SetCam, 100);//等待後再設定Cam避免場景切換時CinemachineBrain的CinemachineVirtualCamera尚未啟用而抓到null
        }

        /// <summary>
        /// 將目前攝影機加到CameraManager中，之後方便使用CameraManager的方法
        /// </summary>
        void SetCam() {
            var camBrain = GameObject.FindGameObjectWithTag("SceneCam").GetComponent<Camera>().GetComponent<Cinemachine.CinemachineBrain>();
            if (camBrain == null) return;
            CamManager.SetCam(camBrain);
            var vCam = camBrain.ActiveVirtualCamera as Cinemachine.CinemachineVirtualCamera;
            CamManager.AddVirtualCam(CamManager.CamNames.Battle, vCam);
        }
        void SpawnAddressableAssets() {



            ////載入UI
            ///            MyLoadingProgress.AddLoadingProgress("LobbyUI");//新增讀取中項目
            //Addressables.LoadAssetAsync<GameObject>(LobbyUIAsset).Completed += handle => {
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

            AddressablesLoader.GetPrefabByRef(BattleUIAsset, (prefab, handle) => {
                AddressableManage.SetToChangeSceneRelease(handle);
                GameObject go = Instantiate(prefab, transform);
                go.GetComponent<Canvas>().worldCamera = Camera.main;
                go.GetComponent<BattleSceneUI>().Init();
            });

            //載入SpellIndicator
            MyLoadingProgress.AddLoadingProgress("SpellIndicator");//新增讀取中項目
            Addressables.LoadAssetAsync<GameObject>(SpellIndicatorAsset).Completed += handle => {
                GameObject go = Instantiate(handle.Result);
                go.transform.localPosition = handle.Result.transform.localPosition;
                go.transform.localScale = handle.Result.transform.localScale;
                go.GetComponent<SpellIndicator>().Init();
                MyLoadingProgress.FinishProgress("SpellIndicator");//完成讀取UI
                Addressables.Release(handle);
            };
        }
        /// <summary>
        /// 戰場初始化完成時執行
        /// </summary>
        public void BattleUILoaded() {
            PopupUI.FinishSceneTransitionProgress("BattleUILoaded");


        }
    }
}