using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneUI : BaseUI {
    [SerializeField]
    private SpellUI _spellUI;

    public override void Init() {
        base.Init();
        _spellUI.Init();
    }

    public override void RefreshText() {
        
    }
}
