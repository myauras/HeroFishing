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

namespace HeroFishing.Main {
    public class HeroUI : ItemSpawner_Remote<HeroIconItem> {

        [SerializeField] ScrollRect MyScrollRect;
        [SerializeField] TextMeshProUGUI TitleText;
        [SerializeField] SpellPanel MySpellPanel;
        [SerializeField] SkinPanel MySkinPanel;
        HeroJsonData.RoleCategory CurCategory = HeroJsonData.RoleCategory.LOL;
        HeroJsonData CurHero;
        HeroSkinJsonData CurHeroSkin;

        LoadingProgress MyLoadingProgress;

        public override void LoadItemAsset(Action _cb = null) {
            base.LoadItemAsset(_cb);
            MySpellPanel.Init();
            MySkinPanel.Init();
            MyLoadingProgress = new LoadingProgress(() => { SwitchCategory(0); }); //子UI都都載入完成再執行SwitchCategory
            MyLoadingProgress.AddLoadingProgress("Hero", "Skin");
            MySkinPanel.LoadItemAsset(() => { MyLoadingProgress.FinishProgress("Skin"); });

        }

        public override void OnLoadItemFinished() {
            base.OnLoadItemFinished();
            MyLoadingProgress.FinishProgress("Hero");
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
            WriteLog.Log("CurCategory=" + CurCategory);
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
            //切換英雄後會自動選到第一個技能
            MySpellPanel.SetHero(CurHero.ID);
            //切換英雄後會自動選到第一個Skin
            MySkinPanel.SetHero(CurHero.ID);
            var firstSkin = HeroSkinJsonData.GetSkinDic(CurHero.ID).First().Value;
            SwitchHeroSkin(firstSkin);
        }
        public void SwitchHeroSkin(HeroSkinJsonData _heroSkinJsonData) {
            CurHeroSkin = _heroSkinJsonData;
        }
        public void OnBattleStartClick() {
            if (CurHero == null) return;
            var mapUI = MapUI.GetInstance<MapUI>();
            if (mapUI == null) return;
            AllocatedRoom.Init();
            AllocatedRoom.Instance.SetMyHero(CurHero.ID, CurHeroSkin.ID); //設定本地玩家自己使用的英雄ID
            //開始跑連線流程, 先連線Matchmaker後會轉連Matchgame並斷連Matchmaker
            PopupUI.ShowLoading(StringJsonData.GetUIString("Loading"));
            GameConnector.Instance.ConnToMatchmaker(mapUI.SelectedDBMap.Id, OnConnResult).Forget();
        }
        void OnConnResult(bool _success) {
            PopupUI.HideLoading();
            if (!_success) {
                WriteLog.LogError("連線失敗");
                return;
            }
            GameConnector.Instance.SetHero(CurHero.ID, CurHeroSkin.ID); //送Server玩家使用的英雄ID

        }
    }


}