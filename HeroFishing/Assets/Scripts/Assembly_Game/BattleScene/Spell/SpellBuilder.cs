using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
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
            case HeroSpellJsonData.HitType.Explode:
                hit = new SpellExplodeHit(Data);
                break;
        }

        SpellMoveBase move = null;
        switch (Data.MyMoveType) {
            case HeroSpellJsonData.MoveType.Target:
                move = new SpellTargetMove(Data);
                break;
        }

        SpellShakeCamera shakeCam = null;
        if(Data.CameraShakeSettings != null) {
            shakeCam = new SpellShakeCamera(Data);
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
            case HeroSpellJsonData.SpellType.SectorArea:
                spell = new SectorAreaSpell(Data);
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
        spell.Move = move;
        spell.ShakeCamera = shakeCam;
        return spell;
    }
}
