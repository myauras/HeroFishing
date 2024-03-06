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

namespace HeroFishing.Main {
    public class HeroUI : ItemSpawner_Remote<HeroIconItem> {

        [SerializeField] ScrollRect MyScrollRect;
        [SerializeField] SpellPanel MySpellPanel;
        [SerializeField] SkinScrollView MySkinScrollView;
        [SerializeField] HeroJsonData.RoleCategory CurCategory = HeroJsonData.RoleCategory.All;
        [SerializeField] GameObject[] CategoryTags;
        [SerializeField] Image HeroBG;
        HeroJsonData CurHero;
        HeroSkinJsonData CurHeroSkin;

        LoadingProgress MyLoadingProgress;

        public override void LoadItemAsset(Action _cb = null) {
            base.LoadItemAsset(_cb);
            MySpellPanel.Init();
            MyLoadingProgress = new LoadingProgress(() => { SwitchCategory(0); }); //子UI都都載入完成再執行SwitchCategory
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
            if (CurHero == null) return;
            var mapUI = MapUI.GetInstance<MapUI>();
            if (mapUI == null) return;
            PopupUI.ShowLoading(StringJsonData.GetUIString("Loading"));
            GamePlayer.Instance.RedisSync().Forget();
            AllocatedRoom.Instance.SetMyHero(CurHero.ID, CurHeroSkin.ID); //設定本地玩家自己使用的英雄ID
            //開始跑連線流程, 先連線Matchmaker後會轉連Matchgame並斷連Matchmaker
            GameConnector.Instance.ConnToMatchmaker(mapUI.SelectedDBMap.Id, OnConnFail, OnConnFail, OnCreateRoom).Forget();
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
            GameConnector.Instance.SetHero(CurHero.ID, CurHeroSkin.ID); //送Server玩家使用的英雄ID                
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
    }


}