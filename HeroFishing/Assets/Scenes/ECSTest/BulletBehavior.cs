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


        }




    }




}