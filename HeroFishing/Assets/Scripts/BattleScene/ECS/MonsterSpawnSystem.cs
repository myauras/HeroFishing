using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace HeroFishing.Battle {
    public partial struct MonsterSpawnSystem : ISystem {
        bool HasSpawned;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterGOPrefab>();
        }

        public void OnUpdate(ref SystemState state) {
            if (HasSpawned) return;
            HasSpawned = true;
            var query = SystemAPI.QueryBuilder().WithAll<MonsterGOPrefab>().Build();
            var entities = query.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities) {
                var monsterGOPrefab = state.EntityManager.GetComponentData<MonsterGOPrefab>(entity);
                var instance = GameObject.Instantiate(monsterGOPrefab.Prefab);
                instance.hideFlags |= HideFlags.HideAndDontSave;
                state.EntityManager.AddComponentObject(entity, instance.GetComponent<Transform>());
                state.EntityManager.AddComponentData(entity, new MonsterGOInstance { Instance = instance });
                state.EntityManager.RemoveComponent<MonsterGOPrefab>(entity);
            }

        }
    }
}
