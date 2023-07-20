using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeroFishing.Battlefield
{
    public partial struct BulletSpawnSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            // This call makes the system not update unless at least one entity in the world exists that has the Spawner component.
            state.RequireForUpdate<BulletSpawner>();
            state.RequireForUpdate<BattlefieldSettingSingleton>();



        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var battlefieldSetting = SystemAPI.GetSingleton<BattlefieldSettingSingleton>();
            if (battlefieldSetting.MyAttackState != BattlefieldSettingSingleton.AttackState.Attacking) return;

            battlefieldSetting.MyAttackState = BattlefieldSettingSingleton.AttackState.Ready;
            SystemAPI.SetSingleton(battlefieldSetting);

            var bulletEntity = SystemAPI.GetSingleton<BulletSpawner>().BulletEntity;

            //var instances = state.EntityManager.Instantiate(bulletEntity, 1, Allocator.Temp);
            //foreach (var entity in instances) {
            //    var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            //    transform.ValueRW.Position = battlefieldSetting.MyAttackData.AttackerPos;
            //    var physicsVel = SystemAPI.GetComponentRW<PhysicsVelocity>(entity);
            //    physicsVel.ValueRW.Linear = battlefieldSetting.MyAttackData.BulletSpeed * battlefieldSetting.MyAttackData.Direction;
            //}


            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var job = new SpawnJob
            {
                ECBWriter = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletEntity = bulletEntity,
                BattlefieldSetting = battlefieldSetting,
            }.Schedule();
            job.Complete();

        }


        [BurstCompile]
        partial struct SpawnJob : IJob
        {

            public EntityCommandBuffer.ParallelWriter ECBWriter;
            [ReadOnly] public BattlefieldSettingSingleton BattlefieldSetting;
            [ReadOnly] public Entity BulletEntity;

            public void Execute()
            {
                var bulletEntity = ECBWriter.Instantiate(BulletEntity.Index, BulletEntity);
                ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new LocalTransform
                {
                    Position = BattlefieldSetting.MyAttackData.AttackerPos,
                    Scale = 1,
                    Rotation = quaternion.identity,
                });
                ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new PhysicsVelocity
                {
                    Linear = BattlefieldSetting.MyAttackData.BulletSpeed * BattlefieldSetting.MyAttackData.Direction,
                });
            }
        }

    }
}
