using HeroFishing.Battle;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;
using static HeroFishing.Battle.SpellIndicator;

public abstract class SpellBase {
    public SpellHitBase Hit;
    public SpellMoveBase Move;
    protected abstract int VariableCount { get; }

    public virtual void Play(Vector3 position, Vector3 heroPosition, Vector3 direction) {
    }

    public virtual IndicatorType SpellIndicatorType => IndicatorType.Line;

    public virtual int IndicatorCount => 1;

    public virtual void IndicatorCallback(GameObject go) { }

    public void OnHit(EntityCommandBuffer.ParallelWriter writer, SpellHitTag hitTag) {
        if (Hit != null) { Hit.OnHit(writer, hitTag); }
    }
}
