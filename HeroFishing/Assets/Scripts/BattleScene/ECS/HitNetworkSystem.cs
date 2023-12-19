using HeroFishing.Socket;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct HitNetworkSystem : ISystem {
    private EndSimulationEntityCommandBufferSystem.Singleton _singleton;
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SpellHitNetworkData>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    public void OnUpdate(ref SystemState state) {
        _singleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var writer = _singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        foreach (var (hitData, entity) in SystemAPI.Query<SpellHitNetworkData>().WithEntityAccess()) {
            var monsterData = SystemAPI.GetBuffer<MonsterHitNetworkData>(entity);
            var path = ECSStrManager.GetStr(hitData.StrIndex_SpellID);
            string spellID = new string(path.ToCharArray());
            var array = monsterData.AsNativeArray();
            var monsterIdxs = new int[array.Length];
            string idxs = string.Empty;
            for (int i = 0; i < array.Length; i++) {
                monsterIdxs[i] = monsterData[i].Monster.MonsterIdx;
                idxs += monsterIdxs[i].ToString() + ", ";
            }
            //idxs.Replace(", ", ")");
            Debug.Log($"hit network attack id: {hitData.AttackID} monsters {idxs} spell id {spellID}");
            writer.DestroyEntity(entity.Index, entity);
            if (GameConnector.Connected)
                GameConnector.Instance.Hit(hitData.AttackID, monsterIdxs, spellID);
        }
    }
}
