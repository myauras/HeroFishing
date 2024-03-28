using HeroFishing.Main;
using HeroFishing.Socket;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DropFrozen : DropSpellBase {
    private float _duration;
    private static CompositeDisposable _disposables;

    public override float Duration => _duration;
    public DropFrozen(DropJsonData data, DropSpellJsonData spellData) : base(data, spellData) {
        _duration = spellData.EffectValue1;
        if (_disposables == null) _disposables = new CompositeDisposable();
    }

    public override bool PlayDrop(int heroIndex) {
        if (!WorldStateManager.Instance.IsFrozen)
            WorldStateManager.Instance.Freeze(true);

        _disposables.Clear();
        Observable.Timer(TimeSpan.FromSeconds(_duration)).Subscribe(_ => {
            WorldStateManager.Instance.Freeze(false);
            //GameConnector.Instance.UpdateScene();
        }).AddTo(_disposables);
        return true;
    }
}
