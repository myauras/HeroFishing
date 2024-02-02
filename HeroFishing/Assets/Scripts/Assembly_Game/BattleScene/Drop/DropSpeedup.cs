using HeroFishing.Battle;
using HeroFishing.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DropSpeedup : DropSpellBase {
    private float _duration;
    private float _speedupMultiplier;

    public DropSpeedup(DropJsonData data, DropSpellJsonData spellData) : base(data, spellData) {
        _duration = spellData.EffectValue1;
        _speedupMultiplier = spellData.EffectValue2;
    }

    public override bool PlayDrop() {
        Debug.Log("play speed up");
        BattleManager.Instance.GetHero(0).AttackSpeedMultiplier = _speedupMultiplier;
        _dropUI.OnDropPlay(_data.ID, _duration);
        Observable.Timer(TimeSpan.FromSeconds(_duration)).Subscribe(_ => {
            BattleManager.Instance.GetHero(0).AttackSpeedMultiplier = 1;
        });
        return true;
    }
}
