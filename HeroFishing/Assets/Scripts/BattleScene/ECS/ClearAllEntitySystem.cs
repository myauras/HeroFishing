using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct ClearAllTag : IComponentData { }

[BurstCompile]
public partial struct ClearAllEntitySystem : ISystem {
    private EntityQuery _query;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ClearAllTag>();
        EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp);
        _query = builder.WithAny<BulletInstance, MonsterInstance, ClearAllTag>().Build(ref state);
        builder.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.EntityManager.DestroyEntity(_query);
    }
}
