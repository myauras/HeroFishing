using HeroFishing.Battle;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(MonsterBehaviourSystem))]
[BurstCompile]
public partial struct LockMonsterSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<LockMonsterData>();
        state.RequireForUpdate<MonsterValue>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var singleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var cmdBuffer = singleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (lockMonsterData, lockEntity) in SystemAPI.Query<LockMonsterData>().WithEntityAccess()) {
            foreach (var monsterValue in SystemAPI.Query<RefRO<MonsterValue>>()) {
                if(monsterValue.ValueRO.MonsterIdx == lockMonsterData.MonsterIdx) {
                    var bulletEntity = cmdBuffer.CreateEntity();
                    var bulletData = lockMonsterData.BulletData;
                    bulletData.TargetMonster = monsterValue.ValueRO;
                    cmdBuffer.AddComponent(bulletEntity, bulletData);
                    break;
                }
            }
            cmdBuffer.DestroyEntity(lockEntity);
        }
    }
}
