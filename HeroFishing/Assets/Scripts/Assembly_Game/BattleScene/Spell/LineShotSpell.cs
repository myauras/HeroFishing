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

    private float _delay;

    private HeroSpellJsonData _data;

    public LineShotSpell(HeroSpellJsonData data) {
        _data = data;

        var values = _data.SpellTypeValues;
        _indicatorLength = float.Parse(values[0]);
        _radius = float.Parse(values[1]);
        _speed = float.Parse(values[2]);
        _lifeTime = float.Parse(values[3]);
        _delay = float.Parse(values[4]);
    }

    public override void Play(SpellPlayData playData) {
        //base.Play(position, heroPosition, direction);
        //var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //var entity = entityManager.CreateEntity();
        //var strIndex_SpellID = ECSStrManager.AddStr(_data.ID);
        //var position = playData.heroPos + new Vector3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);//子彈高度固定調整

        //var spawnData = new SpellSpawnData {
        //    AttackID = playData.attackID,
        //    InitPosition = position,
        //    InitDirection = playData.direction,
        //    SpellPrefabID = _data.PrefabID,
        //    IgnoreFireModel = false,
        //    ProjectileDelay = _delay,
        //};

        //var bulletData = new SpellBulletData {
        //    HeroIndex = playData.heroIndex,
        //    StrIndex_SpellID = strIndex_SpellID,
        //    SpawnData = spawnData,
        //    Speed = _speed,
        //    Radius = _radius,
        //    LifeTime = _lifeTime,
        //    DestroyOnCollision = _data.DestroyOnCollision,
        //    IsSub = false
        //};

        //if (playData.lockAttack && playData.monsterIdx != -1) {
        //    entityManager.AddComponentData(entity, new LockMonsterData {
        //        MonsterIdx = playData.monsterIdx,
        //        BulletData = bulletData,
        //    });
        //}
        //else {
        //    entityManager.AddComponentData(entity, bulletData);
        //}
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
