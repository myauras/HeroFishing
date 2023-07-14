using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;
using System.Numerics;
using UnityEngine.UIElements;

namespace ECSTest {
    [BurstCompile]
    public partial struct SpawnerSystem : ISystem {
        private bool HasSpawned;
        public void OnCreate(ref SystemState state) {
            Debug.Log("OnCreate");
        }

        public void OnDestroy(ref SystemState state) {
            Debug.Log("OnDestroy");
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            if (HasSpawned) return;
            foreach (RefRW<RoleSpawner> spawner in SystemAPI.Query<RefRW<RoleSpawner>>()) {
                ProcessSpawner(ref state, spawner);
            }

        }


        private void ProcessSpawner(ref SystemState state, RefRW<RoleSpawner> spawner) {
            HasSpawned = true;
            Debug.Log("ProcessSpawner");
            // Spawns a new entity and positions it at the spawner.
            Entity entity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
            // LocalPosition.FromPosition returns a Transform initialized with the given position.
            state.EntityManager.SetComponentData(entity, LocalTransform.FromPosition(spawner.ValueRO.Pos));
            //var hero = state.EntityManager.GetComponentObject<Hero>(entity);//<-這行會報沒有Hero
            //hero.PlayIdleMotion();

        }
    }

}