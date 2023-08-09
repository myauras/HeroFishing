using HeroFishing.Main;
using Scoz.Func;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HeroFishing.Battle {
    public struct BulletValue : IComponentData {
        public float Speed;
        public float Radius;
        public float3 Direction;
    }
    /// <summary>
    /// 子彈參照元件，用於參照GameObject實例用
    /// </summary>
    public class BulletInstance : IComponentData, IDisposable {
        public GameObject GO;
        public Transform Trans;
        public Bullet MyBullet;
        public Vector3 Dir;
        public void Dispose() {
            UnityEngine.Object.DestroyImmediate(GO);
        }
    }
    public partial struct BulletSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletSpawnSys>();
            state.RequireForUpdate<SpellCom>();
        }

        public void OnUpdate(ref SystemState state) {


            foreach (var spellCom in SystemAPI.Query<SpellCom>()) {
                var bulletPrefab = ResourcePreSetter.Instance.BulletPrefab;
                if (bulletPrefab == null) continue;
                var bulletGO = GameObject.Instantiate(bulletPrefab.gameObject);
#if UNITY_EDITOR
                bulletGO.name = "BulletProjectile" + spellCom.BulletPrefabID;
                //bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#else
bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#endif
                var bullet = bulletGO.GetComponent<Bullet>();
                if (bullet == null) continue;

                //建立Entity
                var entity = state.EntityManager.CreateEntity();
                float3 direction = spellCom.Direction;
                quaternion bulletQuaternion = quaternion.LookRotation(spellCom.Direction, math.up());
                //設定子彈模型
                bullet.SetData(spellCom.BulletPrefabID, null);
                //設定子彈Transform
                var bulletTrans = new LocalTransform() {
                    Position = spellCom.AttackerPos,
                    Scale = 1,
                    Rotation = bulletQuaternion,
                };
                state.EntityManager.AddComponentData(entity, bulletTrans);
                //設定BulletValue
                var bulletValue = new BulletValue() {
                    Speed = spellCom.Speed,
                    Radius = spellCom.Radius,
                    Direction = direction,
                };
                state.EntityManager.AddComponentData(entity, bulletValue);
                //設定子彈參考物件
                state.EntityManager.AddComponentData(entity, new BulletInstance {
                    GO = bulletGO,
                    Trans = bulletGO.transform,
                    MyBullet = bullet,
                    Dir = direction,
                });

            }



            //var job = new SpawnJob {
            //    ECBWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            //    BulletEntity = MyBulletSpawner.BulletEntity,
            //    ShootEntity = MyBulletSpawner.ShootEntity,
            //    MyBulletValue = bulletValue,
            //    BattlefieldSetting = BattlefieldSetting,
            //}.Schedule();
            //job.Complete();

        }


        //[BurstCompile]
        //partial struct SpawnJob : IJobEntity {

        //    public EntityCommandBuffer.ParallelWriter ECBWriter;
        //    [ReadOnly] public SpellEntities BulletEntitys;

        //    public void Execute(in SpellCom _spellCom, in Entity _entity) {
        //        //建立子彈
        //        var bulletEntity = ECBWriter.Instantiate(0, BulletEntitys.ProjectileEntity);
        //        //設定子彈Value
        //        float3 direction = _spellCom.Direction;
        //        quaternion bulletQuaternion = quaternion.LookRotation(direction, math.up());
        //        //設定子彈Transform
        //        ECBWriter.SetComponent(1, bulletEntity, new LocalTransform {
        //            Position = _spellCom.AttackerPos,
        //            Scale = 1,
        //            Rotation = bulletQuaternion,
        //        });
        //        //設定BulletValue
        //        ECBWriter.SetComponent(2, bulletEntity, new BulletValue {
        //            Speed = _spellCom.Speed,
        //            Radius = _spellCom.Radius,
        //            Direction = direction,
        //        });
        //        //射擊特效
        //        var shootEntity = ECBWriter.Instantiate(3, BulletEntitys.FireEntity);
        //        ECBWriter.SetComponent(4, shootEntity, new LocalTransform {
        //            Position = _spellCom.AttackerPos + math.normalize(_spellCom.TargetPos - _spellCom.AttackerPos) * 0.8f,
        //            Scale = 1,
        //            Rotation = quaternion.Euler(_spellCom.TargetPos - _spellCom.AttackerPos),
        //        });
        //    }

        //}

    }
}
