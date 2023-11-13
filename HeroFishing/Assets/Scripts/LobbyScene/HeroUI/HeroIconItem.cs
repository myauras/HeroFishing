using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;

namespace HeroFishing.Main {

    public class HeroIconItem : MonoBehaviour, IItem {
        [SerializeField] Image HeroIconImg;
        [SerializeField] Button EnterBtn;

        public bool IsActive { get; set; }


        public HeroJsonData MyJsonHero { get; private set; }

        public void Init(HeroJsonData _data) {
            MyJsonHero = _data;
            RefreshItem();
        }


        void RefreshItem() {
            AddressablesLoader.GetSpriteAtlas("HeroIcon", atlas => {
                HeroIconImg.sprite = atlas.GetSprite(MyJsonHero.Ref);
            });
        }

        public void OnClick() {
            HeroUI.GetInstance<HeroUI>()?.SwitchHero(MyJsonHero);
        }
    }
}
