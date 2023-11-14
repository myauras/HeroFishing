using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;

namespace HeroFishing.Main {



    public class MapUI : ItemSpawner_Remote<MapItem> {

        [SerializeField] ScrollRect MyScrollRect;
        [SerializeField] TextMeshProUGUI TitleText;

        public override void RefreshText() {
            base.RefreshText();
            TitleText.text = StringJsonData.GetUIString("MapUITitle");
        }

        public void SpawnItems() {
            if (!LoadItemFinished) {
                WriteLog.LogError("MapItem尚未載入完成");
                return;
            }
            InActiveAllItem();
            var query = RealmManager.MyRealm.All<DBMap>();
            if (query == null || query.Count() == 0) return;
            var dbMaps = query.Where(a => a.Enable == true).ToList();
            dbMaps = dbMaps.OrderByDescending(a => a.Priority).ToList();
            for (int i = 0; i < dbMaps.Count(); i++) {
                if (i < ItemList.Count) {
                    ItemList[i].Init(dbMaps[i]);
                    ItemList[i].IsActive = true;
                    ItemList[i].gameObject.SetActive(true);
                } else {
                    MapItem item = Spawn();
                    item.Init(dbMaps[i]);
                }
            }
            MyScrollRect.verticalNormalizedPosition = 1;//至頂
        }

        public override void SetActive(bool _bool) {
            base.SetActive(_bool);
        }
        public void OnCloseUIClick() {
            LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Lobby);
        }
    }


}