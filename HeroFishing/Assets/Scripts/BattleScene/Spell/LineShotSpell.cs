using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LineShotSpell : SpellBase {
    protected override int VariableCount => 4;

    private float _indicatorLength;
    public float IndicatorLength => _indicatorLength;

    private float _radius;
    public float Radius => _radius;

    private float _speed;
    public float Speed => _speed;

    private float _lifeTime;
    public float LifeTime => _lifeTime;

    private HeroSpellJsonData _data;

    public LineShotSpell(HeroSpellJsonData data) {
        _data = data;

        var values = _data.SpellTypeValues;
        _indicatorLength = float.Parse(values[0]);
        _radius = float.Parse(values[1]);
        _speed = float.Parse(values[2]);
        _lifeTime = float.Parse(values[3]);
    }

    public override void Play(Vector3 position, Vector3 direction) {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();
        var strIndex_SpellID = ECSStrManager.AddStr(_data.ID);
        entityManager.AddComponentData(entity, new SpellData {
            PlayerID = 1,
            StrIndex_SpellID = strIndex_SpellID,
            SpellPrefabID = _data.PrefabID,
            InitPosition = position,
            InitRotation = quaternion.LookRotationSafe(direction, math.up()),
            Speed = Speed,
            Radius = Radius,
            LifeTime = LifeTime,
            Waves = _data.Waves,
            DestoryOnCollision = _data.DestroyOnCollision,
            IgnoreFireModel = false,
            EnableBulletHit = true
        });
    }
}
