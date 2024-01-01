using Cysharp.Threading.Tasks;
using HeroFishing.Socket;
using Realms.Sync;
using Scoz.Func;
using Service.Realms;
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
        [SerializeField] Text VersionText;
        [SerializeField] AudioClip StartMusic;
        [SerializeField] Transform UIParent;

        public static StartSceneManager Instance;
        List<AsyncOperationHandle> HandleList = new List<AsyncOperationHandle>();
        /// <summary>
        /// 是否第一次執行遊戲，第一次執行遊戲後會自動進大廳，之後透過從大廳的設定中點回到主介面就不會又自動進大廳了
        /// </summary>
        public static bool FirstTimeLaunchGame { get; private set; } = true;

        private void Start() {
            ShowInfo();//顯示資訊
 
            Instance = this;
            //建立遊戲管理者
            GameManager.CreateNewInstance();
            //播放背景音樂主題曲，要在GameManager.CreateNewInstance()之後，因為AudioPlayer是在這之後才被初始化
            PlayMainMusic();
            //檢查網路
            PopupUI_Local.ShowLoading("Checking Internet");
            InternetChecker.SetOnConnectedAction(OnConnected);
            InternetChecker.StartCheckInternet();
        }

        /// <summary>
        /// 1. (玩家尚未登入) 顯示版本
        /// 2. (玩家登入) 顯示版本+玩家ID
        /// </summary>
        public void ShowInfo() {
            if (RealmManager.MyApp != null && RealmManager.MyApp.CurrentUser != null)
                VersionText.text = string.Format("Ver: {0} {1} ", Application.version, RealmManager.MyApp.CurrentUser.Id);
            else
                VersionText.text = string.Format("Ver: {0}", Application.version);
        }
        public void PlayMainMusic() {
            AudioPlayer.StopAllMusic_static();
            if (StartMusic != null)
                AudioPlayer.PlayAudioByAudioClip(MyAudioType.Music, StartMusic, true);
        }

        //確認有網路後才會執行後續
        void OnConnected() {
            PopupUI_Local.HideLoading();
            PopupUI_Local.ShowLoading("Init Data");
            ////初始化Realm
            RealmManager.NewApp();
            StartDownloadingAsset(); //開始載包
        }


        private void OnDestroy() {
            for (int i = 0; i < HandleList.Count; i++) {
                if (HandleList[i].IsValid())
                    Addressables.Release(HandleList[i]);
            }
            Instance = null;
        }


        /// <summary>
        /// 1. 開始下載資源包
        /// 2. 載完後顯示準備開始遊戲文字
        /// 3. 切至下一個場景
        /// </summary>
        public void StartDownloadingAsset() {
            GameManager.StartDownloadAddressable(() => {//下載完資源包後執行
            });
        }


    }
}