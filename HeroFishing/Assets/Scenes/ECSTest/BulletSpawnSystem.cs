using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeroFishing.Battlefield {
    public partial struct BulletSpawnSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {

            // This call makes the system not update unless at least one entity in the world exists that has the Spawner component.
            state.RequireForUpdate<BulletSpawner>();

            state.RequireForUpdate<Prefabs>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            if (AttackTrigger.MyAttackState != AttackTrigger.AttackState.Attacking) return;

            var prefab = SystemAPI.GetSingleton<BulletSpawner>().Prefab;
            var instances = state.EntityManager.Instantiate(prefab, 1, Allocator.Temp);

            foreach (var entity in instances) {
                var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = AttackTrigger.MyAttackData.AttackerPos;
                var physicsVel = SystemAPI.GetComponentRW<PhysicsVelocity>(entity);
                physicsVel.ValueRW.Linear = AttackTrigger.MyAttackData.BulletVelocity;
            }
            AttackTrigger.EndAttack();
        }
    }
}
