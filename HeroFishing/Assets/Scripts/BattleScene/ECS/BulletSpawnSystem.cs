using HeroFishing.Main;
using Scoz.Func;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace HeroFishing.Battle {
    public partial struct BulletSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletSpawnSys>();
            state.RequireForUpdate<SpellData>();
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {

            var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (spellData, spellEntity) in SystemAPI.Query<SpellData>().WithEntityAccess()) {
                var bulletPrefab = ResourcePreSetter.Instance.BulletPrefab;
                if (bulletPrefab == null) continue;
                var bulletGO = GameObject.Instantiate(bulletPrefab.gameObject);
#if UNITY_EDITOR
                bulletGO.name = "BulletProjectile" + spellData.SpellPrefabID;
                //bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#else
bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#endif
                var bullet = bulletGO.GetComponent<Bullet>();
                if (bullet == null) {
                    WriteLog.LogErrorFormat("子彈{0}身上沒有掛Bullet Component", bulletPrefab.name);
                    continue;
                }

                //float3 direction = spellData.Direction;
                //quaternion bulletQuaternion = quaternion.LookRotation(spellData.r, math.up());
                //設定子彈Gameobject的Transfrom
                bulletGO.transform.localPosition = spellData.InitPosition;
                bulletGO.transform.localRotation = spellData.InitRotation;

                //建立Entity
                var entity = state.EntityManager.CreateEntity();
                //設定子彈模型
                bullet.SetData(spellData.SpellPrefabID);
                Debug.Log(math.forward(spellData.InitRotation));
                //設定移動
                if (spellData.Speed > 0) {
                    ECB.AddComponent(entity, new MoveData {
                        Speed = spellData.Speed,
                        Position = spellData.InitPosition,
                        Direction = math.forward(spellData.InitRotation)
                    });
                }
                //設定碰撞
                ECB.AddComponent(entity, new CollisionData {
                    PlayerID = spellData.PlayerID,
                    StrIndex_SpellID = spellData.StrIndex_SpellID,
                    SpellPrefabID = spellData.SpellPrefabID,
                    Radius = spellData.Radius,
                    Waves = spellData.Waves,
                    Destroy = spellData.DestoryOnCollision
                });
                ////加入BulletValue
                //ECB.AddComponent(entity, new BulletValue() {
                //    Position = spellData.AttackerPos,
                //    Speed = spellData.Speed,
                //    Radius = spellData.Radius,
                //    Direction = direction,
                //    StrIndex_SpellID = spellData.StrIndex_SpellID,
                //    SpellPrefabID = spellData.SpellPrefabID,
                //});
                //加入BulletInstance
                ECB.AddComponent(entity, new BulletInstance {
                    Trans = bulletGO.transform,
                    MyBullet = bullet,
                    GO = bulletGO
                });
                //加入自動銷毀Tag
                ECB.AddComponent(entity, new AutoDestroyTag {
                    LifeTime = spellData.LifeTime,
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
