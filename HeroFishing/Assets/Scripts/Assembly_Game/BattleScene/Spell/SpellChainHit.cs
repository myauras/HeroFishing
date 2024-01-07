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

    private float _radius;

    private float _speed;

    private float _lifeTime;

    private float _angle;

    private int _maxChainCount;

    public SpellChainHit(HeroSpellJsonData data) {
        _data = data;

        var values = _data.HitTypeValues;
        _triggerRange = float.Parse(values[0]);
        _radius = float.Parse(values[1]);
        _speed = float.Parse(values[2]);
        _lifeTime = float.Parse(values[3]);
        _angle = float.Parse(values[4]);
        _maxChainCount = int.Parse(values[5]);
    }

    public override void OnHit(EntityCommandBuffer.ParallelWriter writer, SpellHitTag hitTag) {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var monsterID = hitTag.Monster.MonsterID;
        var monsterData = MonsterJsonData.GetData(monsterID);
        var position = hitTag.HitPosition + new float3(0, monsterData.Radius, 0);
        Entity entity = entityManager.CreateEntity();
        writer.AddComponent(entity.Index, entity, new ChainHitData {
            PlayerID = hitTag.PlayerID,
            AttackID = hitTag.AttackID,
            StrIndex_SpellID = hitTag.StrIndex_SpellID,
            HitPosition = position,
            HitDirection = hitTag.HitDirection,
            MaxChainCount = _maxChainCount,
            OnHitMonster = hitTag.Monster,
            TriggerRange = _triggerRange,
            Angle = _angle,
            Radius = _radius,
            SpellPrefabID = _data.PrefabID,
            SubSpellPrefabID = _data.SubPrefabID,
            Speed = _speed,
            LifeTime = _lifeTime
        });
    }
}
