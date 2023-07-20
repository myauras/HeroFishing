using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
namespace HeroFishing.Battlefield {
    public partial struct MonsterSpawnSystem : ISystem {
        bool HasSpawned;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            if (HasSpawned) return;
            var prefab = SystemAPI.GetSingleton<MonsterSpawner>().Prefab;
            var instances = state.EntityManager.Instantiate(prefab, 1, Allocator.Temp);

            foreach (var entity in instances) {
                var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                //transform.ValueRW.Position = new float3(0, 0, 0);
            }
            HasSpawned = true;
        }
    }
}
