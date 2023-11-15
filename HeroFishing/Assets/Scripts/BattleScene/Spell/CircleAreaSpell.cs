using HeroFishing.Battle;
using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CircleAreaSpell : SpellBase {
    protected override int VariableCount => 4;
    private float _radius;
    private float _lifeTime;
    private float _delay;
    private float _collisionDelay;
    private float _collisionTime;

    public override SpellIndicator.IndicatorType SpellIndicatorType => SpellIndicator.IndicatorType.Circle;

    private HeroSpellJsonData _data;

    public CircleAreaSpell(HeroSpellJsonData data) {
        _data = data;

        var values = _data.SpellTypeValues;
        _radius = float.Parse(values[0]);
        _lifeTime = float.Parse(values[1]);
        _delay = float.Parse(values[2]);
        _collisionDelay = float.Parse(values[3]);
        _collisionTime = float.Parse(values[4]);
    }

    public override void Play(Vector3 position, Vector3 heroPosition, Vector3 direction) {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();
        var strIndex_SpellID = ECSStrManager.AddStr(_data.ID);
        entityManager.AddComponentData(entity, new SpellData {
            PlayerID = 1,
            StrIndex_SpellID = strIndex_SpellID,
            SpellPrefabID = _data.PrefabID,
            InitPosition = position,
            FirPosition = heroPosition,
            InitRotation = quaternion.LookRotationSafe(direction, math.up()),
            Speed = 0,
            Radius = _radius * 5,
            BulletScale = _radius,
            BulletDelay = _delay,
            CollisionDelay = _collisionDelay,
            CollisionTime = _collisionTime,
            LifeTime = _lifeTime,
            Waves = _data.Waves,
            DestoryOnCollision = _data.DestroyOnCollision,
            IgnoreFireModel = false,
            EnableBulletHit = false
        });
    }

    public override void IndicatorCallback(GameObject go) {
        var renderer = go.GetComponentInChildren<Renderer>();
        renderer.transform.localScale = Vector3.one * _radius * 5;
    }
}
