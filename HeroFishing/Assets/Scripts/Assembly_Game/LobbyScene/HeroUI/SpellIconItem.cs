using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;

namespace HeroFishing.Main {

    public class SpellIconItem : MonoBehaviour, IItem {
        [SerializeField] Image SpellIconImg;
        [SerializeField] Button EnterBtn;

        public bool IsActive { get; set; }


        public HeroSpellJsonData MyJsonSpell { get; private set; }

        public void Init(HeroSpellJsonData _data) {
            MyJsonSpell = _data;
            RefreshItem();
        }


        void RefreshItem() {
            AddressablesLoader.GetSpriteAtlas("SpellIcon", atlas => {
                SpellIconImg.sprite = atlas.GetSprite(MyJsonSpell.Ref);
            });
        }

        public void OnClick() {
            SpellPanel.GetInstance<SpellPanel>()?.SwitchSpell(MyJsonSpell.SpellName);
        }
    }
}
