using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace HeroFishing.Battlefield {
    public partial struct BulletBehavior : ISystem {

        EndSimulationEntityCommandBufferSystem ECBSystem;
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Prefabs>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            //EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);


            //foreach (var (col, entity) in
            //         SystemAPI.Query<RefRO<PhysicsCollider>>()
            //             .WithAll<BulletValue>()
            //             .WithEntityAccess()) {
            //}


            state.Dependency = new CollisionJob().Schedule(
            SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }

        EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state) {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }

        [BurstCompile]
        private struct CollisionJob : ICollisionEventsJob {
            public void Execute(CollisionEvent collisionEvent) {
                Debug.Log($"A: {collisionEvent.EntityA}, B: {collisionEvent.EntityB}");
            }
        }

    }


    [BurstCompile]
    struct ProcessCollideJob : ICollisionEventsJob {

        public void Execute(CollisionEvent collisionEvent) {
            Debug.Log($"Collision between entities {collisionEvent.EntityA.Index} and {collisionEvent.EntityB.Index}");
        }
    }


}