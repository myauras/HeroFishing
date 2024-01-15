using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;

namespace HeroFishing.Main {

    public class SkinItem : MonoBehaviour, IItem {
        [SerializeField] Image SkinIconImg;
        [SerializeField] Button EnterBtn;

        public bool IsActive { get; set; }


        public HeroSkinJsonData MyJsonSkin { get; private set; }

        public void Init(HeroSkinJsonData _data) {
            MyJsonSkin = _data;
            RefreshItem();
        }


        void RefreshItem() {
            AddressablesLoader.GetSpriteAtlas("HeroSkinIcon", atlas => {
                SkinIconImg.sprite = atlas.GetSprite(MyJsonSkin.ID);
            });
        }

        public void OnClick() {
            HeroUI.GetInstance<HeroUI>()?.SwitchHeroSkin(MyJsonSkin);
        }
    }
}
