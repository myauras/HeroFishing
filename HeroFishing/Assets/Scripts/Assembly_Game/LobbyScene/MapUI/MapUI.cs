using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;
using UnityEngine.AddressableAssets;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using HeroFishing.Socket;
using UnityEngine.SceneManagement;

namespace HeroFishing.Main {



    public class MapUI : BaseUI {
        [System.Serializable]
        public class MapColorDicClass : SerializableDictionary<int, Color> { }
        [SerializeField]
        private MapScrollView _mapScrollView;
        [SerializeField]
        private bool _localTest;
        [SerializeField]
        private MapColorDicClass _glowColorDic;
        [SerializeField]
        private MapColorDicClass _txtColorDic;
        [SerializeField]
        private Button _confirmButton;
        [SerializeField] GlassController MyGlass;

        private List<MapItemData> _mapItemDatas;
        private int[] _bets = new int[] { 1, 5, 10, 30, 50, 200, 200, 300 };

        public DBMap SelectedDBMap { get; private set; }

        public override void RefreshText() {
            //TitleText.text = StringJsonData.GetUIString("MapUITitle");
        }

        public override void Init() {
            base.Init();
            _mapItemDatas = new List<MapItemData>();
            RefreshScrollView();
            var glassController = Instantiate(MyGlass, transform);
            glassController.Init();
        }

        public void ResetScrollViewPos() {
            _mapScrollView.ResetPos();
        }

        public void RefreshScrollView() {
            _mapItemDatas.Clear();
            if (_localTest) {
                for (int i = 0; i < 8; i++) {
                    _mapItemDatas.Add(CreateMapItemDataLocal(i));
                }
                _mapScrollView.UpdateData(_mapItemDatas);
                return;
            }

            var query = RealmManager.MyRealm.All<DBMap>();
            if (query == null || query.Count() == 0) {
                Debug.LogError("query is empty");
                return;
            }

            var dbmaps = query.OrderByDescending(item => item.Priority);
            foreach (var dbMap in dbmaps) {
                if (!dbMap.Enable.HasValue || !dbMap.Enable.Value) continue;
                var itemData = CreateMapItemData(dbMap.JsonMapID ?? 1, dbMap.Bet ?? 1, dbMap);
                _mapItemDatas.Add(itemData);
            }
            _mapScrollView.UpdateData(_mapItemDatas);
        }

        public void SelectHeroBtn() {
            LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Hero);
        }


        public void Confirm() {
            GlassController.Instance.Play();
            if (_localTest) {
                Debug.LogWarning("it is local test mode, so it is no db map at all");
                return;
            }


            if (_mapScrollView.SelectedMap == null) {
                Debug.LogError("db map is null");
                return;
            }
            SelectedDBMap = _mapScrollView.SelectedMap;


            if (HeroUI.CurHero == null) return;
            var mapUI = MapUI.GetInstance<MapUI>();
            if (mapUI == null) return;
            PopupUI.ShowLoading(StringJsonData.GetUIString("Loading"));
            GamePlayer.Instance.RedisSync().Forget();
            AllocatedRoom.Instance.SetMyHero(HeroUI.CurHero.ID, HeroUI.CurHeroSkin.ID); //設定本地玩家自己使用的英雄ID
            //開始跑連線流程, 先連線Matchmaker後會轉連Matchgame並斷連Matchmaker
            GameConnector.Instance.ConnToMatchmaker(mapUI.SelectedDBMap.Id, OnConnFail, OnConnFail, OnCreateRoom).Forget();

        }

        private MapItemData CreateMapItemDataLocal(int index) {
            int bet = _bets[index];
            return CreateMapItemData(index + 1, bet);
        }

        private MapItemData CreateMapItemData(int id, int bet, DBMap dbMap = null) {
            var mapData = MapJsonData.GetData(id);
            var itemData = new MapItemData() {
                Id = id,
                Name = mapData.Name,
                Bet = bet,
                IsGradient = bet == _bets[_bets.Length - 1],
                txtColor = _txtColorDic[bet],
                dbMap = dbMap,
                Position = mapData.ForegroundPos,
            };
            itemData.glowColor = itemData.IsGradient ? Color.black : _glowColorDic[bet];
            return itemData;
        }


        void OnConnFail() {
            PopupUI.HideLoading();
            WriteLog.LogError("配房失敗");
        }

        void OnCreateRoom(Socket.Matchmaker.CREATEROOM_TOCLIENT _content) {
            UniTask.Void(async () => {
                PopupUI.CallSceneTransition(MyScene.BattleScene);//跳轉到BattleScene
                //設定玩家目前所在遊戲房間的資料
                await AllocatedRoom.Instance.SetRoom(_content.CreaterID, _content.PlayerIDs, _content.DBMapID, _content.DBMatchgameID, _content.IP, _content.Port, _content.PodName);
                GameConnector.Instance.ConnToMatchgame(OnConnToMatchgame, OnJoinGagmeFail, OnMatchgameDisconnected);
            });
        }

        void OnConnToMatchgame() {
            PopupUI.HideLoading();
            GameConnector.Instance.SetHero(HeroUI.CurHero.ID, HeroUI.CurHeroSkin.ID); //送Server玩家使用的英雄ID                
        }
        void OnJoinGagmeFail() {
            WriteLog.LogError("連線遊戲房失敗");
        }
        void OnMatchgameDisconnected() {
            //在戰鬥場景, 且仍在遊玩中就進行斷線重連
            if (AllocatedRoom.Instance.CurGameState == AllocatedRoom.GameState.Playing && //在遊玩中
                SceneManager.GetActiveScene().name == MyScene.BattleScene.ToString()) {//在戰鬥場景
                GameConnector.Instance.ConnToMatchgame(OnConnToMatchgame, OnJoinGagmeFail, OnMatchgameDisconnected);
            }
        }

        public void SpawnItems() {
            //if (!LoadItemFinished) {
            //    WriteLog.LogError("MapItem尚未載入完成");
            //    return;
            //}
            //InActiveAllItem();
            //if (_localTest) {
            //    for (int i = 0; i < 3; i++) {
            //        var id = i + 1;
            //        var bet = _bets[i];
            //        if(i < ItemList.Count) {
            //            ItemList[i].InitLocally(id, bet);
            //            ItemList[i].IsActive = true;
            //            ItemList[i].gameObject.SetActive(true);
            //        }
            //        else {
            //            MapItem item = Spawn();
            //            item.InitLocally(id, bet);
            //        }
            //    }
            //    return;
            //}

            //var query = RealmManager.MyRealm.All<DBMap>();
            //if (query == null || query.Count() == 0) return;
            //var dbMaps = query.Where(a => a.Enable == true).ToList();
            //dbMaps = dbMaps.OrderByDescending(a => a.Priority).ToList();
            //for (int i = 0; i < dbMaps.Count(); i++) {
            //    if (i < ItemList.Count) {
            //        ItemList[i].Init(dbMaps[i]);
            //        ItemList[i].IsActive = true;
            //        ItemList[i].gameObject.SetActive(true);
            //    } else {
            //        MapItem item = Spawn();
            //        item.Init(dbMaps[i]);
            //    }
            //}
            //MyScrollRect.verticalNormalizedPosition = 1;//至頂
        }

        //public override void SetActive(bool _bool) {
        //    base.SetActive(_bool);
        //}
        //public void OnCloseUIClick() {
        //    LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Lobby);
        //}

        //public void SelectMap(DBMap _dbMap) {
        //    //if (_dbMap == null) return;
        //    //SelectedDBMap = _dbMap;
        //    //LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Hero);
        //}
    }


}