using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class SpellHitBase
{
    public int VariableCount { get; }

    public abstract void OnHit(EntityCommandBuffer.ParallelWriter writer, SpellHitTag hitTag);
}
