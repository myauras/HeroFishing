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
    private Hero _hero;
    private GameObject _buffGO;

    public override float Duration => _duration;

    private const string DROP_BUFF_EFFECT = "OtherEffect/Script_{0}";
    private const string DROP_GET_EFFCT = "OtherEffect/Script_Get{0}";

    public DropSpeedup(DropJsonData data, DropSpellJsonData spellData) : base(data, spellData) {
        _duration = spellData.EffectValue1;
        _speedupMultiplier = spellData.EffectValue2;
        _hero = BattleManager.Instance.GetHero(0);
    }

    public override bool PlayDrop() {
        _hero.AttackSpeedMultiplier = _speedupMultiplier;
        _dropUI.OnDropPlay(_data.ID, _duration);
        string path = string.Format(DROP_BUFF_EFFECT, _data.Ref);
        PoolManager.Instance.Pop(path, _hero.transform.position, Quaternion.identity, null, go => _buffGO = go);

        path = string.Format(DROP_GET_EFFCT, _data.Ref);
        PoolManager.Instance.Pop(path, _hero.transform.position, Quaternion.identity, null);

        Observable.Timer(TimeSpan.FromSeconds(_duration)).Subscribe(_ => {
            _hero.AttackSpeedMultiplier = 1;
            if (_buffGO != null) {
                PoolManager.Instance.Push(_buffGO);
                _buffGO = null;
            }
        });
        return true;
    }
}
