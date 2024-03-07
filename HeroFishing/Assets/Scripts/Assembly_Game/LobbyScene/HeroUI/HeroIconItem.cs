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
        [SerializeField] GameObject Lock;
        [SerializeField] Button EnterBtn;
        [SerializeField] Text Name;

        public bool IsSelected { get; set; }
        public bool IsActive { get; set; }


        public HeroJsonData MyJsonHero { get; private set; }

        public void Init(HeroJsonData _data) {
            MyJsonHero = _data;
        }


        public void RefreshItem() {
            if (MyJsonHero == null) {
                HeroIconImg.sprite = null;
                Lock.SetActive(false);
                EnterBtn.interactable = false;
                return;
            }
            Name.text = MyJsonHero.Name;
            bool unlock = true;
            Lock.SetActive(!unlock);
            EnterBtn.interactable = unlock;
            AddressablesLoader.GetSpriteAtlas("HeroIcon", atlas => {
                if (IsSelected) {
                    HeroIconImg.sprite = atlas.GetSprite(MyJsonHero.Ref + "_On");
                } else {
                    if (unlock) HeroIconImg.sprite = atlas.GetSprite(MyJsonHero.Ref);
                    else HeroIconImg.sprite = atlas.GetSprite(MyJsonHero.Ref + "_Lock");
                }
            });
        }
        public void OnClick() {
            HeroUI.GetInstance<HeroUI>()?.SwitchHero(this);
        }
    }
}
