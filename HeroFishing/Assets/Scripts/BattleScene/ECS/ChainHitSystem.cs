using HeroFishing.Battle;
using Scoz.Func;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[BurstCompile]
public partial struct ChainHitSystem : ISystem {
    EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ChainHitData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var writer = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        //尋找目標怪物
        //float minDistance = float.MaxValue;
        foreach(var (hitData, entity) in SystemAPI.Query<RefRW<ChainHitData>>().WithEntityAccess()) {
            foreach (var monster in SystemAPI.Query<MonsterValue>().WithAbsent<AutoDestroyTag>()) {
                // 檢查是否有怪物，是否是最初擊中的怪物，以及怪物是否已經進場
                if (monster.MyEntity == Entity.Null || hitData.ValueRO.OnHitMonster.MyEntity == monster.MyEntity || !monster.InField) continue;

                // 檢查怪物角度是否小於等於設定值
                float3 monsterDir = monster.Pos - hitData.ValueRO.OnHitMonster.Pos;
                float angle = MathUtility.angle(monsterDir, hitData.ValueRO.HitDirection);
                if (angle > hitData.ValueRO.Angle / 2) continue;

                // 尋找距離最近的怪物
                //float distance = math.distancesq(hitData.ValueRO.OnHitMonster.Pos, monster.Pos);
                //if (distance < hitData.ValueRO.TriggerRange * hitData.ValueRO.TriggerRange && distance < minDistance) {
                //    hitData.ValueRW.NearestMonster = monster;
                //    //Debug.Log("hit monster: " + hitData.ValueRO.OnHitMonster.MyEntity.Index + "_" + hitData.ValueRO.OnHitMonster.MyEntity.Version);
                //    //Debug.Log("chain monster: " + hitData.ValueRO.NearestMonster.MyEntity.Index + "_" + hitData.ValueRO.NearestMonster.MyEntity.Version);
                //    minDistance = distance;
                //}

                var spellEntity = state.EntityManager.CreateEntity();
                var direction = monster.Pos - hitData.ValueRO.HitPosition;
                var position = hitData.ValueRO.HitPosition;
                writer.AddComponent(0, spellEntity, new SpellData {
                    PlayerID = 1,
                    InitPosition = position,
                    InitRotation = quaternion.LookRotationSafe(direction, math.up()),
                    DestoryOnCollision = true,
                    SpellPrefabID = hitData.ValueRO.SpellPrefabID,
                    SubSpellPrefabID = hitData.ValueRO.SubSpellPrefabID,
                    StrIndex_SpellID = hitData.ValueRO.StrIndex_SpellID,
                    Speed = hitData.ValueRO.Speed,
                    LifeTime = hitData.ValueRO.LifeTime,
                    Radius = hitData.ValueRO.Radius,
                    Waves = 0,
                    IgnoreFireModel = true,
                    EnableBulletHit = false,
                    TargetMonster = monster
                });
            }
            writer.DestroyEntity(1, entity);
        }

        //foreach (var (hitData, entity) in SystemAPI.Query<ChainHitData>().WithEntityAccess()) {

        //    if (hitData.NearestMonster.MyEntity != Entity.Null) {
        //        var spellEntity = state.EntityManager.CreateEntity();
        //        var direction = hitData.NearestMonster.Pos - hitData.HitPosition;
        //        var position = hitData.HitPosition;
        //        writer.AddComponent(0, spellEntity, new SpellData {
        //            PlayerID = 1,
        //            InitPosition = position,
        //            InitRotation = quaternion.LookRotationSafe(direction, math.up()),
        //            DestoryOnCollision = true,
        //            SpellPrefabID = hitData.SpellPrefabID,
        //            SubSpellPrefabID = hitData.SubSpellPrefabID,
        //            StrIndex_SpellID = hitData.StrIndex_SpellID,
        //            Speed = hitData.Speed,
        //            LifeTime = hitData.LifeTime,
        //            Radius = hitData.Radius,
        //            Waves = 0,
        //            IgnoreFireModel = true,
        //            EnableBulletHit = false,
        //            TargetMonster = hitData.NearestMonster
        //        });
        //    }
        //    writer.DestroyEntity(1, entity);
        //}
    }
}

//[BurstCompile]
//partial struct CheckMonsterJob : IJobEntity {
//    [ReadOnly] public MapGridData GridData;
//    [ReadOnly] public NativeArray<int2> OffsetGrids;
//    [ReadOnly] public float3 MonsterCollisionPosOffset;

//    public void Execute(ref ChainHitData chainHitData, in Entity entity) {
//        int2 gridIndex = new int2(
//            (int)(chainHitData.HitPosition.x / GridData.CellSize),
//            (int)(chainHitData.HitPosition.z / GridData.CellSize));

//        float minDistance = float.MaxValue;
//        foreach (var offset in OffsetGrids) {
//            int2 gridToCheck = gridIndex + offset;
//            MonsterValue monsterValue;
//            if (GridData.GridMap.TryGetFirstValue(gridToCheck, out monsterValue, out var it)) {
//                do {
//                    if (monsterValue.MyEntity == chainHitData.OnHitMonster.MyEntity) {
//                        Debug.Log("same monster");
//                        continue;
//                    }
//                    float distance = math.distance(monsterValue.Pos, chainHitData.HitPosition);
//                    var radius = monsterValue.Radius + chainHitData.Radius;
//                    //Debug.Log(distance + " " + monsterValue.Radius + " " + chainHitData.Radius);
//                    if (distance < chainHitData.TriggerRange) {
//                        if (distance < minDistance) {
//                            Debug.Log("monster found");
//                            chainHitData.NearestMonster = monsterValue;
//                            minDistance = distance;
//                        }
//                    }
//                } while (GridData.GridMap.TryGetNextValue(out monsterValue, ref it));
//            }
//        }
//    }
//}
