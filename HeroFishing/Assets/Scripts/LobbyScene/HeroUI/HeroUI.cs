using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;

namespace HeroFishing.Main {



    public class HeroUI : ItemSpawner_Remote<HeroItem> {

        [SerializeField] ScrollRect MyScrollRect;
        [SerializeField] TextMeshProUGUI TitleText;
        public HeroJsonData.RoleCategory CurFilter { get; private set; } = HeroJsonData.RoleCategory.LOL;

        public override void RefreshText() {
            base.RefreshText();
            TitleText.text = StringJsonData.GetUIString("HeroUITitle");
        }

        public void SpawnItems() {
            if (!LoadItemFinished) {
                WriteLog.LogError("HeroItem尚未載入完成");
                return;
            }
            InActiveAllItem();
            var heroJsonDic = GameDictionary.GetIntKeyJsonDic<HeroJsonData>("Hero");
            if (heroJsonDic == null || heroJsonDic.Count == 0) return;
            var heroJsons = heroJsonDic.Values.ToList();
            for (int i = 0; i < heroJsons.Count; i++) {
                if (i < ItemList.Count) {
                    ItemList[i].Init(heroJsons[i]);
                    ItemList[i].IsActive = true;
                    ItemList[i].gameObject.SetActive(true);
                } else {
                    HeroItem item = Spawn();
                    item.Init(heroJsons[i]);
                }
            }
            MyScrollRect.verticalNormalizedPosition = 1;//至頂
        }

        public override void SetActive(bool _bool) {
            base.SetActive(_bool);
        }
        public void OnCloseUIClick() {
            LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Map);
        }
    }


}