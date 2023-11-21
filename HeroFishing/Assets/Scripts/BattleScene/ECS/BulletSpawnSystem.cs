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
            var query = SystemAPI.QueryBuilder().WithAny<SpellBulletData, SpellAreaData>().Build();
            state.RequireForUpdate(query);
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {

            var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // 創建 Bullet 技能
            foreach (var (spellData, spellEntity) in SystemAPI.Query<SpellBulletData>().WithEntityAccess()) {

                if (!CreateBulletInstance(spellData.SpawnData, out Bullet bullet)) continue;

                //建立Entity
                var entity = state.EntityManager.CreateEntity();

                //設定移動
                ECB.AddComponent(entity, new MoveData {
                    Speed = spellData.Speed,
                    Position = spellData.SpawnData.InitPosition,
                    Direction = spellData.SpawnData.InitDirection,
                    TargetMonster = spellData.TargetMonster,
                });

                //float collisionTime = spellData.CollisionTime == 0 ? spellData.LifeTime : spellData.CollisionTime;
                //設定碰撞
                ECB.AddComponent(entity, new BulletCollisionData {
                    PlayerID = spellData.PlayerID,
                    StrIndex_SpellID = spellData.StrIndex_SpellID,
                    SpellPrefabID = spellData.SpawnData.SpellPrefabID,
                    Radius = spellData.Radius,
                    Destroy = spellData.DestroyOnCollision,
                    EnableBulletHit = spellData.EnableBulletHit,
                });
                //加入BulletInstance
                ECB.AddComponent(entity, new BulletInstance {
                    Trans = bullet.transform,
                    MyBullet = bullet,
                    GO = bullet.gameObject
                });
                //加入自動銷毀Tag
                ECB.AddComponent(entity, new AutoDestroyTag {
                    LifeTime = spellData.LifeTime,
                    ExistTime = 0,
                });

                //移除施法
                ECB.DestroyEntity(spellEntity);
            }

            // 創建 Area 技能
            foreach (var (spellData, spellEntity) in SystemAPI.Query<SpellAreaData>().WithEntityAccess()) {
                if (!CreateBulletInstance(spellData.SpawnData, out Bullet bullet)) continue;

                //建立Entity
                var entity = state.EntityManager.CreateEntity();

                float collisionTime = spellData.CollisionTime == 0 ? spellData.LifeTime : spellData.CollisionTime;
                //設定碰撞
                ECB.AddComponent(entity, new AreaCollisionData {
                    PlayerID = spellData.PlayerID,
                    StrIndex_SpellID = spellData.StrIndex_SpellID,
                    Position = spellData.SpawnData.InitPosition,
                    Direction = spellData.SpawnData.InitDirection,
                    SpellPrefabID = spellData.SpawnData.SpellPrefabID,
                    Waves = spellData.Waves,
                    CollisionTime = collisionTime,
                    Delay = spellData.CollisionDelay,
                    Timer = 0,
                    Angle = spellData.CollisionAngle,
                    Radius = spellData.Radius
                });
                //加入BulletInstance
                ECB.AddComponent(entity, new BulletInstance {
                    Trans = bullet.transform,
                    MyBullet = bullet,
                    GO = bullet.gameObject
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

        private bool CreateBulletInstance(SpellSpawnData spawnData, out Bullet bullet) {
            bullet = null;
            //var bulletPrefab = ResourcePreSetter.Instance.BulletPrefab;
            //if (bulletPrefab == null) return false;
            //var bulletGO = GameObject.Instantiate(bulletPrefab.gameObject);
            var pool = PoolManager.Instance;
            var bulletGO = pool.PopBullet();
#if UNITY_EDITOR
            bulletGO.name = "BulletProjectile" + spawnData.SpellPrefabID;
            //bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#else
bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#endif
            bullet = bulletGO.GetComponent<Bullet>();
            if (bullet == null) {
                WriteLog.LogErrorFormat("子彈{0}身上沒有掛Bullet Component", bulletGO.name);
                return false;
            }

            //設定子彈Gameobject的Transfrom
            bulletGO.transform.SetLocalPositionAndRotation(spawnData.InitPosition,
                quaternion.LookRotationSafe(spawnData.InitDirection, math.up()));
            bulletGO.transform.SetParent(null);
            if (spawnData.ProjectileScale != 0)
                bulletGO.transform.localScale *= spawnData.ProjectileScale;

            var firePosition = math.all(spawnData.FirePosition == float3.zero) ? spawnData.InitPosition : spawnData.FirePosition;

            //設定子彈模型
            bullet.Create(new BulletInit {
                PrefabID = spawnData.SpellPrefabID,
                SubPrefabID = spawnData.SubSpellPrefabID,
                IgnoreFireModel = spawnData.IgnoreFireModel,
                FirePosition = firePosition,
                Delay = spawnData.ProjectileDelay
            });
            return true;
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
