using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace HeroFishing.Battlefield {
    public partial struct BulletSpawnSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {

            // This call makes the system not update unless at least one entity in the world exists that has the Spawner component.
            state.RequireForUpdate<BulletSpawner>();
            state.RequireForUpdate<BattlefieldSettingSingleton>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var battlefieldSetting = SystemAPI.GetSingleton<BattlefieldSettingSingleton>();
            if (battlefieldSetting.MyAttackState != BattlefieldSettingSingleton.AttackState.Attacking) return;
            battlefieldSetting.MyAttackState = BattlefieldSettingSingleton.AttackState.Ready;
            SystemAPI.SetSingleton(battlefieldSetting);

            var bulletEntity = SystemAPI.GetSingleton<BulletSpawner>().BulletEntity;
            var shootEntity = SystemAPI.GetSingleton<BulletSpawner>().ShootEntity;



            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var job = new SpawnJob {
                ECBWriter = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletEntity = bulletEntity,
                ShootEntity = shootEntity,
                BattlefieldSetting = battlefieldSetting,
            }.Schedule();
            job.Complete();

        }


        [BurstCompile]
        partial struct SpawnJob : IJob {

            public EntityCommandBuffer.ParallelWriter ECBWriter;
            [ReadOnly] public BattlefieldSettingSingleton BattlefieldSetting;
            [ReadOnly] public Entity BulletEntity;
            [ReadOnly] public Entity ShootEntity;

            public void Execute() {




                float angle = 15f;
                for (int i = -2; i < 3; i++) {


                    float3 direction = BattlefieldSetting.MyAttackData.Direction;
                    quaternion rotation = quaternion.RotateY(math.radians(angle * i));  // 建立一個偏移Y軸15度的四元數
                    float3 rotatedDirection = math.mul(rotation, direction);  // 旋转方向向量

                    //建立子彈
                    var bulletEntity = ECBWriter.Instantiate(BulletEntity.Index, BulletEntity);
                    //設定子彈Transform
                    ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new LocalTransform {
                        Position = BattlefieldSetting.MyAttackData.AttackerPos,
                        Scale = 1,
                        Rotation = quaternion.identity,
                    });

                    //設定速度
                    ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new PhysicsVelocity {
                        Linear = rotatedDirection * BattlefieldSetting.MyAttackData.BulletSpeed,
                    });
                }




                var shootEntity = ECBWriter.Instantiate(ShootEntity.Index, ShootEntity);
                ECBWriter.SetComponent(ShootEntity.Index, shootEntity, new LocalTransform {
                    Position = BattlefieldSetting.MyAttackData.AttackerPos + math.normalize(BattlefieldSetting.MyAttackData.TargetPos - BattlefieldSetting.MyAttackData.AttackerPos) * 0.8f,
                    Scale = 1,
                    Rotation = quaternion.Euler(BattlefieldSetting.MyAttackData.TargetPos - BattlefieldSetting.MyAttackData.AttackerPos),
                });
            }
        }

    }
}
