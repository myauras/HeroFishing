using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;

namespace HeroFishing.Main {
    public class SpellPanel : BaseUI {

        [SerializeField] SpellIconItem[] SpellItems = new SpellIconItem[3];
        [SerializeField] Text HeroNameText;
        [SerializeField] Text HeroTitleText;
        [SerializeField] Text SpellNameText;
        [SerializeField] Text SpellDescriptionText;
        [SerializeField] Image RoleTypeImg;

        HeroJsonData CurHero;
        HeroSpellJsonData CurSpell;

        public void SetHero(int _heroID) {
            CurHero = HeroJsonData.GetData(_heroID);
            for (int i = 0; i < SpellItems.Length; i++) {
                var spellJson = HeroSpellJsonData.GetSpell(_heroID, (SpellName)(i + 1));
                if (spellJson == null) continue;

                SpellItems[i].Init(spellJson);
            }
            AddressablesLoader.GetSpriteAtlas("HeroUI", atlas => {
                RoleTypeImg.sprite = atlas.GetSprite(CurHero.MyRoleType.ToString());
            });
            SwitchSpell(SpellName.spell1);
        }

        public void SwitchSpell(SpellName _spellName) {
            if (_spellName == SpellName.attack) return;
            CurSpell = HeroSpellJsonData.GetSpell(CurHero.ID, _spellName);
            for (int i = 0; i < SpellItems.Length; i++) {
                SpellItems[i].Switch((int)_spellName == (i + 1));
            }
            RefreshText();
        }


        public override void RefreshText() {
            if (CurSpell == null) return;
            HeroNameText.text = CurHero.Name;
            HeroTitleText.text = CurHero.Title;
            SpellNameText.text = CurSpell.Name;
            SpellDescriptionText.text = CurSpell.Description;
        }



    }


}