using HeroFishing.Battle;
using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SectorAreaSpell : SpellBase {
    protected override int VariableCount => 4;

    private float _radius;
    private float _scale;
    private float _lifeTime;
    private float _angle;

    public override SpellIndicator.IndicatorType SpellIndicatorType => SpellIndicator.IndicatorType.Cone;

    private HeroSpellJsonData _data;

    public SectorAreaSpell(HeroSpellJsonData data) {
        _data = data;

        var values = _data.SpellTypeValues;

        _radius = float.Parse(values[0]);
        _scale = float.Parse(values[1]);
        _lifeTime = float.Parse(values[2]);
        _angle = float.Parse(values[3]);
    }

    public override void Play(SpellPlayData playData) {
        base.Play(playData);
        var spawnBulletInfo = new SpawnBulletInfo {
            PrefabID = _data.PrefabID,
            ProjectileScale = _scale,
            InitPosition = playData.attackPos,
            InitDirection = playData.direction,
            IgnoreFireModel = false,
            LifeTime = _lifeTime,
            IsDrop = playData.IsDrop,
        };
        if (!BulletSpawner.Spawn(spawnBulletInfo, out Bullet bullet)) return;
        var collisionInfo = new AreaCollisionInfo {
            HeroIndex = playData.heroIndex,
            AttackID = playData.attackID,
            SpellID = _data.ID,
            Radius = _radius,
            Waves = _data.Waves,
            Position = playData.attackPos,
            Direction = playData.direction,
            Angle = _angle,
            Duration = _lifeTime,
        };
        BulletSpawner.AddCollisionComponent(collisionInfo, bullet);
        //base.Play(position, heroPosition, direction);
        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var entity = entityManager.CreateEntity();
        //var strIndex_SpellID = ECSStrManager.AddStr(_data.ID);
        //var spawnData = new SpellSpawnData {
        //    AttackID = playData.attackID,
        //    SpellPrefabID = _data.PrefabID,
        //    ProjectileScale = _scale,
        //    InitPosition = playData.attackPos,
        //    InitDirection = playData.direction,
        //    IgnoreFireModel = false
        //};

        //entityManager.AddComponentData(entity, new SpellAreaData {
        //    HeroIndex = playData.heroIndex,
        //    StrIndex_SpellID = strIndex_SpellID,
        //    SpawnData = spawnData,
        //    Radius = _radius,
        //    Waves = _data.Waves,
        //    LifeTime = _lifeTime,
        //    CollisionAngle = _angle,
        //});
    }

    public override void IndicatorCallback(GameObject go) {
        var renderer = go.GetComponentInChildren<Renderer>();
        renderer.transform.localScale = Vector3.one * _radius * 2;
        var mat = renderer.material;
        mat.SetFloat("_Angle", _angle);
    }
}
