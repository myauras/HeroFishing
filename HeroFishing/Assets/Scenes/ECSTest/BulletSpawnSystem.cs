using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeroFishing.Battlefield {
    public partial struct BulletSpawnSystem : ISystem {
        uint updateCounter;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {

            // This call makes the system not update unless at least one entity in the world exists that has the Spawner component.
            state.RequireForUpdate<Spawner>();

            state.RequireForUpdate<Prefabs>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<BulletValue>().Build();


            if (spinningCubesQuery.IsEmpty) {
                var prefab = SystemAPI.GetSingleton<Spawner>().Prefab;

                Debug.Log("Spawn");
                var instances = state.EntityManager.Instantiate(prefab, 1, Allocator.Temp);

                foreach (var entity in instances) {
                    // Update the entity's LocalTransform component with the new position.
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                    float3 heroPos = SystemAPI.GetSingleton<Singleton_BattlefieldSetting>().HeroPos;
                    Debug.Log("heroPos=" + heroPos);
                    transform.ValueRW.Position = heroPos;
                }
            }
        }
    }
}
