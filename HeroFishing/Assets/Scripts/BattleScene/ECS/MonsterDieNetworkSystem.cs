using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct MonsterDieNetworkSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MonsterDieNetworkData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecbWriter = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (monsterDie, entity) in SystemAPI.Query<MonsterDieNetworkData>().WithEntityAccess()) {
            foreach (var killMonster in monsterDie.KillMonsters) {
                foreach (var (monsterValue, monsterEntity) in SystemAPI.Query<MonsterValue>().WithEntityAccess()) {
                    //Debug.Log("compare: " + killMonster.KillMonsterIdx + " " + monsterValue.MonsterIdx);
                    if (killMonster.KillMonsterIdx != monsterValue.MonsterIdx) continue;
                    ecbWriter.AddComponent(monsterEntity, new MonsterDieTag());
                    //Debug.Log($"kill monster idx: {killMonster.KillMonsterIdx} poins: {killMonster.GainPoints} hero exp: {killMonster.GainHeroExp}" +
                    //    $"\nspell charge: {killMonster.GainSpellCharge} drop: {killMonster.GainDrop}");
                }
            }
            ecbWriter.DestroyEntity(entity);
        }
    }
}
