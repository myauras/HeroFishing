using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropBase {
    public virtual float Duration { get; }
    public virtual bool IsAttack { get; set; }
    protected DropJsonData _data;
    public DropBase(DropJsonData data) {
        _data = data;
    }

    public virtual void SetAttackID(int attackID) { }
    public abstract void AddDrop(int heroIndex);
    public abstract bool PlayDrop(int heroIndex);
}

public abstract class DropSpellBase : DropBase {
    protected DropSpellJsonData _spellData;

    public DropSpellBase(DropJsonData data, DropSpellJsonData spellData) : base(data) {
        _spellData = spellData;
    }

    public override void AddDrop(int heroIndex) {

    }
}
