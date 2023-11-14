using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;
using System;

namespace HeroFishing.Main {



    public class HeroUI : ItemSpawner_Remote<HeroIconItem> {

        [SerializeField] ScrollRect MyScrollRect;
        [SerializeField] TextMeshProUGUI TitleText;
        [SerializeField] SpellPanel MySpellPanel;
        [SerializeField] SkinPanel MySkinPanel;
        HeroJsonData.RoleCategory CurCategory = HeroJsonData.RoleCategory.LOL;
        HeroJsonData CurHero;

        public override void LoadItemAsset(Action _cb = null) {
            base.LoadItemAsset(_cb);
            MySpellPanel.Init();
            MySkinPanel.Init();
            MySkinPanel.LoadItemAsset(null);
        }

        public override void RefreshText() {
            base.RefreshText();
            TitleText.text = StringJsonData.GetUIString("HeroUITitle");
        }
        public void SwitchCategory(int _categoryIndex) {
            HeroJsonData.RoleCategory changeToCategory;
            if (!MyEnum.TryParseEnum(_categoryIndex, out changeToCategory)) {
                WriteLog.LogErrorFormat("錯誤的傳入值_categoryIndex: {0}", _categoryIndex);
                return;
            }
            CurCategory = changeToCategory;
            SpawnItems();
            SwitchHero(GetFirstHero());
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
                    HeroIconItem item = Spawn();
                    item.Init(heroJsons[i]);
                }
            }
            Filter();
        }
        HeroJsonData GetFirstHero() {
            for (int i = 0; i < ItemList.Count; i++) {
                if (ItemList[i].IsActive == false) continue;
                return ItemList[i].MyJsonHero;
            }
            return null;
        }
        void Filter() {
            for (int i = 0; i < ItemList.Count; i++) {
                if (ItemList[i].IsActive == false) continue;
                ItemList[i].gameObject.SetActive(CurCategory == ItemList[i].MyJsonHero.MyRoleCategory);
                ItemList[i].IsActive = CurCategory == ItemList[i].MyJsonHero.MyRoleCategory;
            }
            MyScrollRect.verticalNormalizedPosition = 1;//至頂
        }

        public void OnCloseUIClick() {
            LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Map);
        }
        public void SwitchHero(HeroJsonData _heroJsonData) {
            if (_heroJsonData == null) return;
            CurHero = _heroJsonData;
            MySpellPanel.SetHero(CurHero.ID);
            MySkinPanel.SetHero(CurHero.ID);
        }
    }


}