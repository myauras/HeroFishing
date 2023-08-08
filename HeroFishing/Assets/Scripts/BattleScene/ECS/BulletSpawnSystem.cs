using Scoz.Func;
using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace HeroFishing.Battle {
    [CreateAfter(typeof(BulletSpawnerAuthoring))]
    public partial struct BulletSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
        BulletSpawner MyBulletSpawner;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {

            state.RequireForUpdate<BulletSpawner>();
            state.RequireForUpdate<SpellCom>();

            MyBulletSpawner = BulletSpawnerAuthoring.MyBulletSpawner;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {


            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();


            new SpawnJob {
                ECBWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletEntity = MyBulletSpawner. ,
                ShootEntity = MyBulletSpawner.ShootEntity,
            }.ScheduleParallel();


            //var job = new SpawnJob {
            //    ECBWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            //    BulletEntity = MyBulletSpawner.BulletEntity,
            //    ShootEntity = MyBulletSpawner.ShootEntity,
            //    MyBulletValue = bulletValue,
            //    BattlefieldSetting = BattlefieldSetting,
            //}.Schedule();
            //job.Complete();

        }


        [BurstCompile]
        partial struct SpawnJob : IJobEntity {

            public EntityCommandBuffer.ParallelWriter ECBWriter;
            [ReadOnly] public Entity BulletEntity;
            [ReadOnly] public Entity ShootEntity;

            public void Execute(in SpellCom _spellCom, in Entity _entity) {
                //建立子彈
                var bulletEntity = ECBWriter.Instantiate(BulletEntity.Index, BulletEntity);
                //設定子彈Value
                float3 direction = _spellCom.Direction;
                quaternion bulletQuaternion = quaternion.LookRotation(direction, math.up());
                //設定子彈Transform
                ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new LocalTransform {
                    Position = _spellCom.AttackerPos,
                    Scale = 1,
                    Rotation = bulletQuaternion,
                });
                //設定BulletValue
                ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new BulletValue {
                    Speed = _spellCom.Speed,
                    Radius = _spellCom.Radius,
                    Direction = direction,
                });
                //射擊特效
                var shootEntity = ECBWriter.Instantiate(ShootEntity.Index, ShootEntity);
                ECBWriter.SetComponent(ShootEntity.Index, shootEntity, new LocalTransform {
                    Position = _spellCom.AttackerPos + math.normalize(_spellCom.TargetPos - _spellCom.AttackerPos) * 0.8f,
                    Scale = 1,
                    Rotation = quaternion.Euler(_spellCom.TargetPos - _spellCom.AttackerPos),
                });
            }

            //public void Execute() {

            //    float angle = 15f;
            //    for (int i = 0; i < 1; i++) {

            //        //建立子彈
            //        var bulletEntity = ECBWriter.Instantiate(BulletEntity.Index, BulletEntity);
            //        //設定子彈Value
            //        float3 direction = BattlefieldSetting.MyAttackData.Direction;
            //        quaternion rotation = quaternion.RotateY(math.radians(angle * i));  // 建立一個偏移Y軸X度的四元數
            //        float3 rotatedDirection = math.mul(rotation, direction) * new float3(1, 0, 1);  // 選轉方向向量
            //        quaternion bulletQuaternion = quaternion.LookRotation(direction, math.up());
            //        //設定子彈Transform
            //        ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new LocalTransform {
            //            Position = BattlefieldSetting.MyAttackData.AttackerPos,
            //            Scale = 1,
            //            Rotation = bulletQuaternion,
            //        });

            //        ECBWriter.SetComponent(BulletEntity.Index, bulletEntity, new BulletValue {
            //            Speed = MyBulletValue.Speed,
            //            Radius = MyBulletValue.Radius,
            //            Direction = rotatedDirection,
            //        });
            //    }

            //    //射擊特效
            //    var shootEntity = ECBWriter.Instantiate(ShootEntity.Index, ShootEntity);
            //    ECBWriter.SetComponent(ShootEntity.Index, shootEntity, new LocalTransform {
            //        Position = BattlefieldSetting.MyAttackData.AttackerPos + math.normalize(BattlefieldSetting.MyAttackData.TargetPos - BattlefieldSetting.MyAttackData.AttackerPos) * 0.8f,
            //        Scale = 1,
            //        Rotation = quaternion.Euler(BattlefieldSetting.MyAttackData.TargetPos - BattlefieldSetting.MyAttackData.AttackerPos),
            //    });
            //}
        }

    }
}
