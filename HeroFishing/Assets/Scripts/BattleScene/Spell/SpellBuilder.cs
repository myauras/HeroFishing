using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpellBuilder {
    public HeroSpellJsonData Data;

    public SpellBuilder(HeroSpellJsonData data) {
        Data = data;
    }

    public SpellBase Build() {
        SpellHitBase hit = null;
        switch (Data.MyHitType) {
            case HeroSpellJsonData.HitType.Chain:
                hit = new SpellChainHit(Data);
                break;
        }

        SpellBase spell = null;
        switch (Data.MySpellType) {
            case HeroSpellJsonData.SpellType.LineShot:
                spell = new LineShotSpell(Data);
                break;
            case HeroSpellJsonData.SpellType.SpreadLineShot:
                return null;
            case HeroSpellJsonData.SpellType.CircleArea:
                spell = new CircleAreaSpell(Data);
                break;
                //case HeroSpellJsonData.SpellType.LineRange:
                //    return null;
                //case HeroSpellJsonData.SpellType.LineRangeInstant:
                //    return null;
        }

        if (spell == null) {
            WriteLog.LogError("spell is not fetch any type");
            return null;
        }
        spell.Hit = hit;
        return spell;
    }
}
