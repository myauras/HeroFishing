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

    private float _radius;

    private float _speed;

    private float _lifeTime;

    private HeroSpellJsonData _data;

    public LineShotSpell(HeroSpellJsonData data) {
        _data = data;

        var values = _data.SpellTypeValues;
        _indicatorLength = float.Parse(values[0]);
        _radius = float.Parse(values[1]);
        _speed = float.Parse(values[2]);
        _lifeTime = float.Parse(values[3]);
    }

    public override void Play(Vector3 position, Vector3 heroPosition, Vector3 direction) {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();
        var strIndex_SpellID = ECSStrManager.AddStr(_data.ID);
        position += new Vector3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);//子彈高度固定調整
        var spawnData = new SpellSpawnData {
            InitPosition = position,
            InitDirection = direction,
            SpellPrefabID = _data.PrefabID,
            IgnoreFireModel = false
        };

        entityManager.AddComponentData(entity, new SpellBulletData {
            PlayerID = 1,
            StrIndex_SpellID = strIndex_SpellID,
            SpawnData = spawnData,
            Speed = _speed,
            Radius = _radius,
            LifeTime = _lifeTime,
            DestroyOnCollision = _data.DestroyOnCollision,
            EnableBulletHit = true
        });
    }

    public override void IndicatorCallback(GameObject go) {
        var renderer = go.GetComponentInChildren<Renderer>();
        var mat = renderer.material;
        mat.SetTextureOffset("_MainTex", new Vector2(0, -_indicatorLength));
        var scale = renderer.transform.localScale;
        renderer.transform.localScale = new Vector3(_radius, scale.y, scale.z);
        go.transform.localRotation = Quaternion.identity;
    }
}
