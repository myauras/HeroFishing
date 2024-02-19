using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
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
    private Monster[] _targetMonsters;

    public SpellChainHit(HeroSpellJsonData data) {
        _data = data;

        var values = _data.HitTypeValues;
        _triggerRange = float.Parse(values[0]);
        _radius = float.Parse(values[1]);
        _speed = float.Parse(values[2]);
        _lifeTime = float.Parse(values[3]);
        _angle = float.Parse(values[4]);
        _maxChainCount = int.Parse(values[5]);
        _targetMonsters = new Monster[_maxChainCount + 2];
    }

    public override void OnHit(SpellHitInfo hitInfo) {
        int count = Monster.GetMonstersInRangeWithAngle(hitInfo.HitPosition, _triggerRange, hitInfo.HitRotation * Vector3.forward, _angle, _targetMonsters, hitInfo.Monster);
        System.Array.Sort(_targetMonsters, (ma, mb) => {
            if (ma == null || mb == null) return 0;
            return (int)(Vector3.SqrMagnitude(ma.transform.position - hitInfo.HitPosition)
            - Vector3.SqrMagnitude(mb.transform.position - hitInfo.HitPosition));
        });
        for (int i = 0; i < count && i < _maxChainCount; i++) {
            var monster = _targetMonsters[i];
            var direction = (monster.transform.position - hitInfo.HitPosition).normalized;
            SpawnBulletInfo bulletInfo = new SpawnBulletInfo {
                PrefabID = _data.PrefabID,
                SubPrefabID = _data.SubPrefabID,
                InitPosition = hitInfo.HitPosition,
                InitDirection = direction,
                IgnoreFireModel = true,
                LifeTime = _lifeTime,
            };
            if (!BulletSpawner.Spawn(bulletInfo, out Bullet bullet)) continue;
            BulletCollisionInfo collisionInfo = new BulletCollisionInfo {
                HeroIndex = hitInfo.HeroIndex,
                AttackID = hitInfo.AttackID,
                SpellID = _data.ID,
                DestroyOnCollision = true,
                Speed = _speed,
                Radius = _radius,
                IsSub = true,
                TargetMonsterIdx = monster.MonsterIdx
            };
            BulletSpawner.AddCollisionComponent(collisionInfo, bullet);
        }
    }
    //public override void OnHit(EntityCommandBuffer.ParallelWriter writer, SpellHitTag hitTag) {
    //    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //    var monsterID = hitTag.Monster.MonsterID;
    //    var monsterData = MonsterJsonData.GetData(monsterID);
    //    var position = hitTag.HitPosition + new float3(0, monsterData.Radius, 0);
    //    Entity entity = entityManager.CreateEntity();
    //    writer.AddComponent(entity.Index, entity, new ChainHitData {
    //        HeroIndex = hitTag.HeroIndex,
    //        AttackID = hitTag.AttackID,
    //        StrIndex_SpellID = hitTag.StrIndex_SpellID,
    //        HitPosition = position,
    //        HitDirection = hitTag.HitDirection,
    //        MaxChainCount = _maxChainCount,
    //        OnHitMonster = hitTag.Monster,
    //        TriggerRange = _triggerRange,
    //        Angle = _angle,
    //        Radius = _radius,
    //        SpellPrefabID = _data.PrefabID,
    //        SubSpellPrefabID = _data.SubPrefabID,
    //        Speed = _speed,
    //        LifeTime = _lifeTime
    //    });
    //}
}
