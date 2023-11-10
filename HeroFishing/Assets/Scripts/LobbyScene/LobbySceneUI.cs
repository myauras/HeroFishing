using UnityEngine;
using Scoz.Func;
using System;


namespace HeroFishing.Main {

    public class LobbySceneUI : BaseUI {

        public enum LobbyUIs {
            Lobby,//預設介面
            Map,//地圖介面
        }
        public LobbyUIs CurUI { get; private set; } = LobbyUIs.Lobby;
        public BaseUI LastPopupUI { get; private set; }//紀錄上次的彈出介面(切介面時要關閉上次的彈出介面)

        static bool FirstEnterLobby = true;//第一次進入大廳後會設定回false 用來判斷是否第一次進入大廳而做判斷


        //進遊戲就要初始化的UI放這裡(會增加場景切換時的讀取時間)
        [SerializeField] MapUI MyMapUI;


        public static LobbySceneUI Instance { get; private set; }


        ////進遊戲不先初始化，等到要用時才初始化的UI放這裡
        //[SerializeField] AssetReference BattleUIAsset;
        ////後產生的UI父層
        //[SerializeField] Transform BattleUIParent;
        ////後產生的UI實體
        //BattleUI MyBattleUI;

        public override void Init() {
            base.Init();
            MyMapUI.Init();
            SwitchUI(LobbyUIs.Lobby);
            Instance = this;
        }

        public void SwitchUI(LobbyUIs _ui, Action _cb = null) {

            if (LastPopupUI != null)
                LastPopupUI.SetActive(false);//關閉彈出介面
            //PlayerInfoUI.GetInstance<PlayerInfoUI>()?.SetActive(false);//所有介面預設都不會開啟資訊界面

            switch (_ui) {
                case LobbyUIs.Lobby://本來在其他介面時，可以傳入Lobby來關閉彈出介面並顯示回預設介面
                    MyMapUI.SetActive(false);
                    _cb?.Invoke();
                    LastPopupUI = null;
                    break;
                case LobbyUIs.Map:
                    MyMapUI.SetActive(true);
                    _cb?.Invoke();
                    LastPopupUI = MyMapUI;
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

        public void OnMapBtnClick() {
            SwitchUI(LobbyUIs.Map);
        }



    }
}