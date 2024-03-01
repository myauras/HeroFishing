using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;
using UnityEngine.AddressableAssets;

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
            foreach(var dbMap in dbmaps) {
                if (!dbMap.Enable.HasValue || !dbMap.Enable.Value) continue;
                var itemData = CreateMapItemData(dbMap.JsonMapID ?? 1, dbMap.Bet ?? 1, dbMap);
                _mapItemDatas.Add(itemData);
            }
            _mapScrollView.UpdateData(_mapItemDatas);
        }

        public void Confirm() {
            if (_localTest) {
                Debug.LogWarning("it is local test mode, so it is no db map at all");
                return;
            }


            if (_mapScrollView.SelectedMap == null) {
                Debug.LogError("db map is null");
                return;
            }

            SelectedDBMap = _mapScrollView.SelectedMap;
            LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Hero);
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