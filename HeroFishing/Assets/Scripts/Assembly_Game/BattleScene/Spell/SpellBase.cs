using HeroFishing.Battle;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;
using static HeroFishing.Battle.SpellIndicator;

public struct SpellPlayData {
    public bool lockAttack;
    public int heroIndex;
    public int monsterIdx;
    public int attackID;
    public Vector3 attackPos;
    public Vector3 heroPos;
    public Vector3 direction;
}

public abstract class SpellBase {
    public SpellHitBase Hit;
    public SpellMoveBase Move;
    public SpellShakeCamera ShakeCamera;
    protected abstract int VariableCount { get; }

    public virtual void Play(SpellPlayData playData) {
    }

    public virtual IndicatorType SpellIndicatorType => IndicatorType.Line;

    public virtual int IndicatorCount => 1;

    public virtual void IndicatorCallback(GameObject go) { }

    public void OnHit(SpellHitInfo hitInfo) {
        if (Hit != null)
            Hit.OnHit(hitInfo);
    }
    //public void OnHit(EntityCommandBuffer.ParallelWriter writer, SpellHitTag hitTag) {
    //    if (Hit != null) { Hit.OnHit(writer, hitTag); }
    //}
}
