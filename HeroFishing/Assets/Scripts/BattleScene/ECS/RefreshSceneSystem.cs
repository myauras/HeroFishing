using HeroFishing.Battle;
using HeroFishing.Main;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[UpdateBefore(typeof(MonsterBehaviourSystem))]
[BurstCompile]
public partial struct RefreshSceneSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<RefreshSceneTag>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    public void OnUpdate(ref SystemState state) {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (spawnData, entity) in SystemAPI.Query<SpawnData>().WithAll<RefreshSceneTag>().WithEntityAccess()) {
            bool found = false;
            foreach (var monsterData in spawnData.Monsters) {
                // update
                foreach (var (monsterValue, monsterInstance, monsterEntity) in SystemAPI.Query<RefRW<MonsterValue>, MonsterInstance>().WithAbsent<AutoDestroyTag>().WithEntityAccess()) {
                    if (monsterValue.ValueRO.MonsterID == monsterData.ID && monsterValue.ValueRO.MonsterIdx == monsterData.Idx) {
                        //Debug.Log("refresh update");
                        found = true;
                        var deltaTime = GameTime.Current - spawnData.SpawnTime;
                        var routeData = RouteJsonData.GetData(spawnData.RouteID);
                        var speed = MonsterJsonData.GetData(monsterData.ID).Speed;
                        var rotation = Quaternion.AngleAxis(spawnData.PlayerIndex * 90, Vector3.up);
                        var dir = rotation * (routeData.TargetPos - routeData.SpawnPos).normalized;
                        var deltaPos = speed * deltaTime * dir;
                        if (Vector3.SqrMagnitude(deltaPos) > Vector3.SqrMagnitude(routeData.TargetPos - routeData.SpawnPos)) break;
                        monsterValue.ValueRW.Pos = rotation * routeData.SpawnPos + deltaPos;
                        commandBuffer.AddComponent<AlreadyUpdateTag>(monsterEntity);
                        monsterInstance.MyMonster.FaceDir(Quaternion.LookRotation(dir));
                        monsterInstance.Dir = dir;
                        break;
                    }
                }
            }
            // add
            if (!found) {
                //Debug.Log("refresh add");
                commandBuffer.RemoveComponent<RefreshSceneTag>(entity);
                commandBuffer.AddComponent<SpawnTag>(entity);
            }
            else {
                //Debug.Log("refresh destroy");
                commandBuffer.DestroyEntity(entity);
            }
        }

        state.Dependency.Complete();
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();

        EntityCommandBuffer commandBuffer2 = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        // delete
        var deleteQuery = SystemAPI.QueryBuilder().WithAll<MonsterValue>().WithAbsent<AutoDestroyTag, AlreadyUpdateTag>().Build();
        //Debug.Log("refresh delete " + deleteQuery.CalculateEntityCount());
        commandBuffer2.AddComponent(deleteQuery, new AutoDestroyTag { LifeTime = 1 });

        var aliveQuery = SystemAPI.QueryBuilder().WithAll<MonsterValue, AlreadyUpdateTag>().Build();
        //Debug.Log("refresh clear " + aliveQuery.CalculateEntityCount());
        commandBuffer2.RemoveComponent<AlreadyUpdateTag>(aliveQuery, EntityQueryCaptureMode.AtRecord);

        state.Dependency.Complete();
        commandBuffer2.Playback(state.EntityManager);
        commandBuffer2.Dispose();
    }
}
