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
        [SerializeField] TextMeshProUGUI NameText;
        [SerializeField] TextMeshProUGUI SPText;
        [SerializeField] TextMeshProUGUI DescriptionText;

        int CurHeroID;
        HeroSpellJsonData CurSpell;

        public void SetHero(int _heroID) {
            CurHeroID = _heroID;
            for (int i = 0; i < SpellItems.Length; i++) {
                var spellJson = HeroSpellJsonData.GetSpell(CurHeroID, (SpellName)(i + 1));
                if (spellJson == null) continue;

                SpellItems[i].Init(spellJson);
            }
            SwitchSpell(SpellName.spell1);
        }

        public void SwitchSpell(SpellName _spellName) {
            if (_spellName == SpellName.attack) return;
            CurSpell = HeroSpellJsonData.GetSpell(CurHeroID, _spellName);
            RefreshText();
        }


        public override void RefreshText() {
            if (CurSpell == null) return;
            NameText.text = CurSpell.Name;
            SPText.text = CurSpell.Cost.ToString();
            DescriptionText.text = CurSpell.Description;
        }



    }


}