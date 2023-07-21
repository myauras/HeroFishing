using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace HeroFishing.Battlefield {
    public partial struct BulletSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
        BattlefieldSettingSingleton BattlefieldSetting;
        BulletSpawner MyBulletSpawner;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {

            // This call makes the system not update unless at least one entity in the world exists that has the Spawner component.
            state.RequireForUpdate<BulletSpawner>();
            state.RequireForUpdate<BattlefieldSettingSingleton>();


        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            BattlefieldSetting = SystemAPI.GetSingleton<BattlefieldSettingSingleton>();
            if (BattlefieldSetting.MyAttackState != BattlefieldSettingSingleton.AttackState.Attacking) return;
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            MyBulletSpawner = SystemAPI.GetSingleton<BulletSpawner>();

            BattlefieldSetting.MyAttackState = BattlefieldSettingSingleton.AttackState.Ready;
            SystemAPI.SetSingleton(BattlefieldSetting);


            var bulletValue = state.EntityManager.GetComponentData<BulletValue>(MyBulletSpawner.BulletEntity);

            var job = new SpawnJob {
                ECBWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletEntity = MyBulletSpawner.BulletEntity,
                ShootEntity = MyBulletSpawner.ShootEntity,
                MyBulletValue = bulletValue,
                BattlefieldSetting = BattlefieldSetting,
            }.Schedule();
            job.Complete();

        }


        [BurstCompile]
        partial struct SpawnJob : IJob {

            public EntityCommandBuffer.ParallelWriter ECBWriter;
            [ReadOnly] public BattlefieldSettingSingleton BattlefieldSetting;
            [ReadOnly] public BulletValue MyBulletValue;
            [ReadOnly] public Entity BulletEntity;
            [ReadOnly] public Entity ShootEntity;

            public void Execute() {

                float angle = 15f;
                for (int i = -4; i < 5; i++) {

                    //建立子彈
                    var bulletEntity = ECBWriter.Instantiate(BulletEntity.Index, BulletEntity);
                    //設定子彈Transform
                    ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new LocalTransform {
                        Position = BattlefieldSetting.MyAttackData.AttackerPos,
                        Scale = 1,
                        Rotation = quaternion.identity,
                    });




                    //設定子彈Value
                    float3 direction = BattlefieldSetting.MyAttackData.Direction;
                    quaternion rotation = quaternion.RotateY(math.radians(angle * i));  // 建立一個偏移Y軸15度的四元數
                    float3 rotatedDirection = math.mul(rotation, direction);  // 選轉方向向量
                    ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new BulletValue {
                        Speed = MyBulletValue.Speed,
                        Radius = MyBulletValue.Radius,
                        Direction = rotatedDirection,
                    });

                    //不使用Unity物理 改自定義子彈移動 寫在BulletBehavior 這樣效能好很多
                    //設定速度
                    //ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new PhysicsVelocity {
                    //    Linear = rotatedDirection * BattlefieldSetting.MyAttackData.BulletSpeed,
                    //});
                }

                //射擊特效
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
