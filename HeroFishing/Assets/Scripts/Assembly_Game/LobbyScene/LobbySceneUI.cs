using UnityEngine;
using Scoz.Func;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Service.Realms;

namespace HeroFishing.Main {

    public class LobbySceneUI : BaseUI {

        public enum LobbyUIs {
            Lobby,//預設介面
            Map,//地圖介面
            Hero,//英雄介面
        }
        public Dictionary<LobbyUIs, BaseUI> UIs = new Dictionary<LobbyUIs, BaseUI>();
        public LobbyUIs CurUI { get; private set; } = LobbyUIs.Lobby;
        public BaseUI LastPopupUI { get; private set; }//紀錄上次的彈出介面(切介面時要關閉上次的彈出介面)

        static bool FirstEnterLobby = true;//第一次進入大廳後會設定回false 用來判斷是否第一次進入大廳而做判斷


        //進遊戲就要初始化的UI放這裡(會增加場景切換時的讀取時間)
        [SerializeField] MapUI MyMapUI;
        [SerializeField] HeroUI MyHeroUI;


        public static LobbySceneUI Instance { get; private set; }


        ////進遊戲不先初始化，等到要用時才初始化的UI放這裡
        //[SerializeField] AssetReference BattleUIAsset;
        ////後產生的UI父層
        //[SerializeField] Transform BattleUIParent;
        ////後產生的UI實體
        //BattleUI MyBattleUI;

        private void Start() {
            Init();
        }

        public override void Init() {
            base.Init();
            Instance = this;


            //檢查realmg登入檢查
            RealmLoginCheck(() => {
                //初始化UIs
                PopupUI.FinishSceneTransitionProgress("LobbyUILoaded");
                MyMapUI.Init();
                MyMapUI.LoadItemAsset();
                UIs.Add(LobbyUIs.Map, MyMapUI);
                MyHeroUI.Init();
                MyHeroUI.LoadItemAsset();
                UIs.Add(LobbyUIs.Hero, MyHeroUI);
                SwitchUI(LobbyUIs.Lobby);

            });
        }


        void RealmLoginCheck(Action _ac) {
            if (RealmManager.MyApp.CurrentUser == null) {//尚無Realm帳戶
                PopupUI.ShowLoading("玩家尚未登入Realm 要先登入Realm才能從Lobby開始遊戲");
                WriteLog.LogError("玩家尚未登入Realm 要先登入Realm才能從Lobby開始遊戲");
                return;
            } else {//已經有Realm帳戶，就登入Realm

                PopupUI.ShowLoading(StringJsonData.GetUIString("DataLoading"));
                UniTask.Void(async () => {
                    await RealmManager.OnSignin();
                    RealmManager.OnDataLoaded();
                    PopupUI.HideLoading();
                    _ac?.Invoke();
                });
            }
        }

        void CloseUIExcept(LobbyUIs _exceptUI) {
            foreach (var key in UIs.Keys) {
                UIs[key].SetActive(key == _exceptUI);
            }
        }

        public void SwitchUI(LobbyUIs _ui, Action _cb = null) {

            if (LastPopupUI != null)
                LastPopupUI.SetActive(false);//關閉彈出介面

            CloseUIExcept(_ui);//打開目標UI關閉其他UI

            switch (_ui) {
                case LobbyUIs.Lobby://本來在其他介面時，可以傳入Lobby來關閉彈出介面並顯示回預設介面
                    _cb?.Invoke();
                    LastPopupUI = null;
                    break;
                case LobbyUIs.Map:
                    MyMapUI.SpawnItems();
                    _cb?.Invoke();
                    LastPopupUI = MyMapUI;
                    break;
                case LobbyUIs.Hero:
                    MyHeroUI.SwitchCategory(0);
                    LastPopupUI = MyHeroUI;
                    break;

                    //case AdventureUIs.Battle:
                    //    MyCreateRoleUI.SetActive(false);
                    //    MyBattleUI?.SetActive(true);
                    //    //判斷是否已經載入過此UI，若還沒載過就跳讀取中並開始載入
                    //    if (MyBattleUI != null) {
                    //        MyBattleUI.SetBattle();
                    //        _cb?.Invoke();
                    //    } else {
                    //        LoadAssets(_ui, _cb);//讀取介面
                    //    }
                    //    LastPopupUI = MyBattleUI;

                    //    break;
            }
        }

        public override void RefreshText() {
        }

        //void LoadAssets(AdventureUIs _ui, Action _cb) {
        //    switch (_ui) {
        //        case AdventureUIs.Battle:
        //            PopupUI.ShowLoading(StringData.GetUIString("WaitForLoadingUI"));
        //            //初始化UI
        //            AddressablesLoader.GetPrefabByRef(BattleUIAsset, (prefab, handle) => {
        //                PopupUI.HideLoading();
        //                GameObject go = Instantiate(prefab);
        //                go.transform.SetParent(BattleUIParent);

        //                RectTransform rect = go.GetComponent<RectTransform>();
        //                go.transform.localPosition = prefab.transform.localPosition;
        //                go.transform.localScale = prefab.transform.localScale;
        //                rect.offsetMin = Vector2.zero;//Left、Bottom
        //                rect.offsetMax = Vector2.zero;//Right、Top

        //                MyBattleUI = go.GetComponent<BattleUI>();
        //                MyBattleUI.Init();
        //                MyBattleUI.SetBattle();
        //                _cb?.Invoke();
        //            }, () => { WriteLog.LogError("載入BattleUIAsset失敗"); });
        //            break;
        //    }
        //}

        public void GoBattle() {
            PopupUI.InitSceneTransitionProgress(1);
            PopupUI.CallSceneTransition(MyScene.BattleScene);
        }

        public void OnMapBtnClick() {
            SwitchUI(LobbyUIs.Map);
        }




    }
}