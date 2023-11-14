using HeroFishing.Battle;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using UnityEngine;

public abstract class SpellBase {
    public SpellHitBase Hit;
    protected abstract int VariableCount { get; }

    public virtual void Play(Vector3 position, Vector3 direction) {

    }

    public virtual void ShowPreview() {
        
    }

    public void OnHit(EntityCommandBuffer.ParallelWriter writer, BulletHitTag hitTag) {
        if (Hit != null) { Hit.OnHit(writer, hitTag); }
    }
}
