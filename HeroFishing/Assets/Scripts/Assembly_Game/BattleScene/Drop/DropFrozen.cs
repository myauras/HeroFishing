using HeroFishing.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DropFrozen : DropSpellBase {
    private float _duration;
    public DropFrozen(DropJsonData data, DropSpellJsonData spellData) : base(data, spellData) {
        _duration = spellData.EffectValue1;
    }

    public override bool PlayDrop() {
        WorldStateManager.Instance.Freeze(true);
        _dropUI.OnDropPlay(_data.ID, _duration);
        Observable.Timer(TimeSpan.FromSeconds(_duration)).Subscribe(_ => {
            WorldStateManager.Instance.Freeze(false);
        });
        return true;
    }
}
