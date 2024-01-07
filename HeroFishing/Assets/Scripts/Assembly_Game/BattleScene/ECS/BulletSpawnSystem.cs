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
                    AttackID = spellData.SpawnData.AttackID,
                    StrIndex_SpellID = spellData.StrIndex_SpellID,
                    SpellPrefabID = spellData.SpawnData.SpellPrefabID,
                    Radius = spellData.Radius,
                    Delay = spellData.SpawnData.ProjectileDelay,
                    Destroy = spellData.DestroyOnCollision,
                    IsSub = spellData.IsSub,
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
                    AttackID = spellData.SpawnData.AttackID,
                    StrIndex_SpellID = spellData.StrIndex_SpellID,
                    Position = spellData.SpawnData.InitPosition,
                    Direction = spellData.SpawnData.InitDirection,
                    SpellPrefabID = spellData.SpawnData.SpellPrefabID,
                    Waves = spellData.Waves,
                    CollisionTime = collisionTime,
                    Delay = spellData.CollisionDelay,
                    Timer = 0,
                    Angle = spellData.CollisionAngle,
                    Radius = spellData.Radius,
                    IgnoreMonster = spellData.IgnoreMonster,
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
        }

        private bool CreateBulletInstance(SpellSpawnData spawnData, out Bullet bullet) {
            bullet = null;
            var pool = PoolManager.Instance;
            var bulletGO = pool.PopBullet(spawnData.SpellPrefabID, spawnData.SubSpellPrefabID);
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
                bulletGO.transform.localScale = Vector3.one * spawnData.ProjectileScale;

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
    }
}
