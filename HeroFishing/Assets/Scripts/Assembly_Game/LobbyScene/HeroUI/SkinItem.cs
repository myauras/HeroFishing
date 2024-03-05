using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;
using FancyScrollView;
using JoshH.UI;
using UnityEngine.AddressableAssets;

namespace HeroFishing.Main {

    public class SkinItem : FancyCell<SkinItemData, Context> {
        [SerializeField] Image SkinIconImg;
        [SerializeField] Button _btnSelect;
        public bool IsActive { get; set; }

        public SkinItemData MySkinItem { get; private set; }
        private void Start() {
            _btnSelect.onClick.AddListener(() => Context.OnClick?.Invoke(Index));
        }
        public override void Initialize() {
            base.Initialize();
        }

        public override  void UpdateContent(SkinItemData _item) {
            MySkinItem = _item;
            RefreshItem();
        }

        public override void UpdatePosition(float position) {
            SetHightlight(position);
        }
        private void SetHightlight(float position) {
            WriteLog.LogError("position=" + position);
        }

        void RefreshItem() {
            AddressablesLoader.GetSpriteAtlas("HeroSkinIcon", atlas => {
                SkinIconImg.sprite = atlas.GetSprite(MySkinItem.MyJsonSkin.ID);
            });
        }

        public void OnClick() {
            HeroUI.GetInstance<HeroUI>()?.SwitchHeroSkin(MySkinItem.MyJsonSkin);
        }
    }
}
