using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;
using System;
using HeroFishing.Socket;
using Cysharp.Threading.Tasks;
using LitJson;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UniRx;

namespace HeroFishing.Main {
    public class HeroUI : ItemSpawner_Remote<HeroIconItem> {

        [SerializeField] ScrollRect MyScrollRect;
        [SerializeField] SpellPanel MySpellPanel;
        [SerializeField] SkinScrollView MySkinScrollView;
        [SerializeField] HeroJsonData.RoleCategory CurCategory = HeroJsonData.RoleCategory.All;
        [SerializeField] GameObject[] CategoryTags;
        [SerializeField] Image HeroBG;
        public static HeroJsonData CurHero { get; private set; }
        public static HeroSkinJsonData CurHeroSkin { get; private set; }

        LoadingProgress MyLoadingProgress;

        public override void LoadItemAsset(Action _cb = null) {
            base.LoadItemAsset(_cb);
            MySpellPanel.Init();
            MyLoadingProgress = new LoadingProgress(() => {
                CurHero = HeroJsonData.GetData(1);
                CurHeroSkin = HeroSkinJsonData.GetData("1_1");
                SwitchCategory(0); 
            }); //子UI都都載入完成再執行SwitchCategory
            MyLoadingProgress.AddLoadingProgress("Hero", "Skin");
            MySkinScrollView.LoadItemAsset(() => { MyLoadingProgress.FinishProgress("Skin"); });
        }

        public override void OnLoadItemFinished() {
            base.OnLoadItemFinished();
            MyLoadingProgress.FinishProgress("Hero");
        }


        public override void RefreshText() {
            base.RefreshText();
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
            RefreshTags();
        }

        void RefreshTags() {
            for (int i = 0; i < CategoryTags.Length; i++) {
                CategoryTags[i].SetActive(i == (int)CurCategory);
            }
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
        HeroIconItem GetFirstHero() {
            for (int i = 0; i < ItemList.Count; i++) {
                if (ItemList[i].IsActive == false) continue;
                return ItemList[i];
            }
            return null;
        }
        void Filter() {
            for (int i = 0; i < ItemList.Count; i++) {
                if (ItemList[i].IsActive == false) continue;
                bool meetCatetory = (CurCategory == HeroJsonData.RoleCategory.All) || (CurCategory == ItemList[i].MyJsonHero.MyRoleCategory);
                ItemList[i].gameObject.SetActive(meetCatetory);
                ItemList[i].IsActive = meetCatetory;
            }
            MyScrollRect.verticalNormalizedPosition = 1;//至頂
        }

        public void OnCloseUIClick() {
            LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Map);
        }
        public void SwitchHero(HeroIconItem _item) {
            if (_item == null) return;
            SetItems(_item);
            CurHero = _item.MyJsonHero;
            //切換英雄後會自動選到第一個技能
            MySpellPanel.SetHero(CurHero.ID);
            //切換英雄後會自動選到第一個Skin
            MySkinScrollView.RefreshScrollView(CurHero.ID);
            var firstSkin = HeroSkinJsonData.GetSkinDic(CurHero.ID).First().Value;
            SwitchHeroSkin(firstSkin);
        }
        void SetItems(HeroIconItem _item) {
            for (int i = 0; i < ItemList.Count; i++) {
                ItemList[i].IsSelected = ItemList[i] == _item;
                ItemList[i].RefreshItem();
            }
        }
        public void SwitchHeroSkin(HeroSkinJsonData _heroSkinJsonData) {
            CurHeroSkin = _heroSkinJsonData;
            AddressablesLoader.GetSprite("HeroBG/" + CurHeroSkin.ID, (sprite, handle) => {
                HeroBG.sprite = sprite;
                //Addressables.Release(handle);
            });
        }
        public void OnBattleStartClick() {
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => {
                LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Hero);
            });
        }


    }


}