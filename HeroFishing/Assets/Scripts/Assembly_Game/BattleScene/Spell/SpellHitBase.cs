using HeroFishing.Battle;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SpellHitInfo {
    public int HeroIndex;
    public int AttackID;
    public Monster Monster;
    public Vector3 HitPosition;
    public Quaternion HitRotation;
}

public abstract class SpellHitBase
{
    public int VariableCount { get; }

    public abstract void OnHit(SpellHitInfo hitInfo);
    //public abstract void OnHit(EntityCommandBuffer.ParallelWriter writer, SpellHitTag hitTag);
}
