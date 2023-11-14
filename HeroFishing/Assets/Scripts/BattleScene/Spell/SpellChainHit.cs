using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpellChainHit : SpellHitBase {
    private HeroSpellJsonData _data;

    private float _triggerRange;
    public float TriggerRange => _triggerRange;

    private float _radius;
    public float Radius => _radius;

    private float _speed;
    public float Speed => _speed;

    private float _lifeTime;
    public float LifeTime => _lifeTime;

    public SpellChainHit(HeroSpellJsonData data) {
        _data = data;

        var values = _data.HitTypeValues;
        _triggerRange = float.Parse(values[0]);
        _radius = float.Parse(values[1]);
        _speed = float.Parse(values[2]);
        _lifeTime = float.Parse(values[3]);
    }

    public override void OnHit(EntityCommandBuffer.ParallelWriter writer, BulletHitTag hitTag) {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = entityManager.CreateEntity();
        writer.AddComponent(entity.Index, entity, new ChainHitData {
            StrIndex_SpellID = hitTag.StrIndex_SpellID,
            HitPosition = hitTag.HitPosition,
            OnHitMonster = hitTag.Monster,
            TriggerRange = _triggerRange,
            Radius = _radius,
            SpellPrefabID = _data.PrefabID,
            Speed = _speed,
            LifeTime = _lifeTime
        });
    }
}
