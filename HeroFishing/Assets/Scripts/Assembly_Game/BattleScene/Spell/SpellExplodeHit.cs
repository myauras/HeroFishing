using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpellExplodeHit : SpellHitBase {

    private HeroSpellJsonData _data;
    private float _delay;
    private float _radius;
    private float _lifeTime;
    private int _waves;

    public SpellExplodeHit(HeroSpellJsonData data) {
        _data = data;

        var values = _data.HitTypeValues;
        _delay = float.Parse(values[0]);
        _radius = float.Parse(values[1]);
        _lifeTime = float.Parse(values[2]);
        _waves = int.Parse(values[3]);
    }

    public override void OnHit(EntityCommandBuffer.ParallelWriter writer, SpellHitTag hitTag) {
        var entity = writer.CreateEntity(0);
        writer.AddComponent(entity.Index + 1, entity, new SpellAreaData() {
            PlayerID = hitTag.PlayerID,
            StrIndex_SpellID = hitTag.StrIndex_SpellID,
            SpawnData = new SpellSpawnData {
                AttackID = hitTag.AttackID,
                SpellPrefabID = _data.PrefabID,
                SubSpellPrefabID = _data.SubPrefabID,
                InitPosition = hitTag.BulletPosition,
                IgnoreFireModel = true,
                ProjectileDelay = _delay,
            },
            Radius = _radius,
            LifeTime = _lifeTime,            
            IgnoreMonster = hitTag.Monster,
            Waves = _waves,
        });
    }
}
