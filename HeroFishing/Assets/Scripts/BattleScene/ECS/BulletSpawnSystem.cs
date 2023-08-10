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
        public float3 Position;
        public float3 Direction;
    }
    /// <summary>
    /// 子彈參照元件，用於參照GameObject實例用
    /// </summary>
    public class BulletInstance : IComponentData, IDisposable {
        public Transform Trans;
        public Bullet MyBullet;
        public Vector3 Dir;
        public void Dispose() {
            UnityEngine.Object.Destroy(Trans.gameObject);
        }
    }
    public partial struct BulletSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletSpawnSys>();
            state.RequireForUpdate<SpellCom>();
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {


            var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (spellCom, spellEntity) in SystemAPI.Query<SpellCom>().WithEntityAccess()) {
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
                if (bullet == null) {
                    WriteLog.LogErrorFormat("子彈{0}身上沒有掛Bullet Component", bulletPrefab.name);
                    continue;
                }

                float3 direction = spellCom.Direction;
                quaternion bulletQuaternion = quaternion.LookRotation(spellCom.Direction, math.up());
                //設定子彈Gameobject的Transfrom
                bulletGO.transform.localPosition = spellCom.AttackerPos;
                bulletGO.transform.localRotation = bulletQuaternion;

                //建立Entity
                var entity = state.EntityManager.CreateEntity();
                //設定子彈模型
                bullet.SetData(spellCom.BulletPrefabID);
                //加入BulletValue
                ECB.AddComponent(entity, new BulletValue() {
                    Position = spellCom.AttackerPos,
                    Speed = spellCom.Speed,
                    Radius = spellCom.Radius,
                    Direction = direction,
                });
                //加入BulletInstance
                ECB.AddComponent(entity, new BulletInstance {
                    Trans = bulletGO.transform,
                    MyBullet = bullet,
                    Dir = direction,
                });
                //加入自動銷毀Tag
                ECB.AddComponent(entity, new AutoDestroyTag {
                    LifeTime = spellCom.LifeTime,
                    ExistTime = 0,
                });

                //移除施法
                ECB.DestroyEntity(spellEntity);
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
