using HeroFishing.Main;
using Scoz.Func;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HeroFishing.Battle {
    [CreateAfter(typeof(MonsterBehaviourSystem))]
    [UpdateAfter(typeof(MonsterBehaviourSystem))]
    [UpdateAfter(typeof(BulletSpawnSystem))]
    [UpdateAfter(typeof(BulletMoveSystem))]
    public partial struct BulletCollisionSystem : ISystem {

        NativeArray<int2> OffsetGrids;//定義碰撞檢定9宮格
        [ReadOnly] BufferLookup<HitInfoBuffer> HitInfoLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletCollisionData>();
            state.RequireForUpdate<CollisionSys>();
            // 定義一個範圍來檢查子彈所在和周圍的網格
            OffsetGrids = new NativeArray<int2>(9, Allocator.Persistent);
            OffsetGrids[0] = new int2(0, 0);// 子彈本身所在的網格
            OffsetGrids[1] = new int2(0, 1);// 上
            OffsetGrids[2] = new int2(1, 1);// 右上
            OffsetGrids[3] = new int2(1, 0);// 右
            OffsetGrids[4] = new int2(1, -1);// 右下
            OffsetGrids[5] = new int2(0, -1);// 下
            OffsetGrids[6] = new int2(-1, -1);// 左下
            OffsetGrids[7] = new int2(-1, 0);// 左
            OffsetGrids[8] = new int2(-1, 1);// 左上

            HitInfoLookup = state.GetBufferLookup<HitInfoBuffer>(false);
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
            OffsetGrids.Dispose();
        }

        public void OnUpdate(ref SystemState state) {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            float deltaTime = SystemAPI.Time.DeltaTime;
            double elapsedTime = SystemAPI.Time.ElapsedTime;


            uint seed = (uint)(deltaTime * 1000000f);
            //取得網格資料
            var gridData = SystemAPI.GetSingleton<MapGridData>();
            var storageInfoLookup = state.GetEntityStorageInfoLookup();
            HitInfoLookup.Update(ref state);

            new MoveJob {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                DeltaTime = deltaTime,
                ElapsedTime = elapsedTime,
                Seed = seed,
                GridData = gridData,
                OffsetGrids = OffsetGrids,
                MonsterCollisionPosOffset = BattleManager.MonsterCollisionPosOffset,
                HitInfoLookup = HitInfoLookup,
                StorageInfoLookup = storageInfoLookup,
            }.ScheduleParallel();

        }
        [BurstCompile]
        partial struct MoveJob : IJobEntity {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public double ElapsedTime;
            [ReadOnly] public uint Seed;
            [ReadOnly] public MapGridData GridData;
            [ReadOnly] public NativeArray<int2> OffsetGrids;
            [ReadOnly] public float3 MonsterCollisionPosOffset;
            [ReadOnly] public BufferLookup<HitInfoBuffer> HitInfoLookup;
            [ReadOnly] public EntityStorageInfoLookup StorageInfoLookup;

            public void Execute(ref BulletCollisionData _collisionData, ref MoveData _moveData, in Entity _entity) {
                _collisionData.Timer += DeltaTime;
                if (_collisionData.Timer < _collisionData.Delay) {
                    return;
                }

                // 計算移動，如果有目標則朝著目標，沒有的話走直線
                bool hasTargetMonster = StorageInfoLookup.Exists(_moveData.TargetMonster.MyEntity);
                if (!hasTargetMonster) {
                    _moveData.Position += _moveData.Direction * _moveData.Speed * DeltaTime;
                }
                else {
                    var targetPos = _moveData.TargetMonster.Pos;
                    var direction = math.normalize(targetPos - _moveData.Position);
                    direction.y = 0;
                    _moveData.Direction = direction;
                    _moveData.Position += _moveData.Direction * _moveData.Speed * DeltaTime;
                }
                //_bullet.Position += (_bullet.Speed * _bullet.Direction) * DeltaTime;

                // 計算子彈的網格索引
                int2 gridIndex = new int2(
                    (int)(_moveData.Position.x / GridData.CellSize),
                    (int)(_moveData.Position.z / GridData.CellSize)
                );

                // 如果目標不在區域範圍了，直接銷毀子彈
                if (hasTargetMonster && !_moveData.TargetMonster.InField) {
                    ECB.DestroyEntity(8, _entity);
                    return;
                }


                foreach (var offset in OffsetGrids) {
                    int2 gridToCheck = gridIndex + offset;

                    MonsterValue monsterValue;
                    if (GridData.GridMap.TryGetFirstValue(gridToCheck, out monsterValue, out var iterator)) {
                        // 這裡放第一個找到的value要做的事情
                        do {

                            // 如果已經指定目標，則除目標外的怪物都不碰撞
                            if (hasTargetMonster && _moveData.TargetMonster.MyEntity != monsterValue.MyEntity) {
                                continue;
                            }
                            // 使用當前找到的value要做某些事情
                            float dist = math.distance(monsterValue.Pos + MonsterCollisionPosOffset, _moveData.Position);
                            if (dist < (_collisionData.Radius + monsterValue.Radius)) {//怪物在子彈的命中範圍內

                                double checkTime = 0.5;
                                //確認是否已經打過
                                if (CheckAlreayHitMonster(_entity, monsterValue.MyEntity, checkTime)) continue;

                                //如果lookup裡面還沒有子彈資訊，AddBuffer新增資訊。如果已經有資訊，AppendToBuffer將新資訊放在後面。
                                if (!_collisionData.Destroy) {
                                    if (!HitInfoLookup.HasBuffer(_entity)) {
                                        ECB.AddBuffer<HitInfoBuffer>(1, _entity)
                                            .Add(new HitInfoBuffer { MonsterEntity = monsterValue.MyEntity, HitTime = ElapsedTime });
                                    }
                                    else
                                        ECB.AppendToBuffer(1, _entity, new HitInfoBuffer { MonsterEntity = monsterValue.MyEntity, HitTime = ElapsedTime });
                                }

                                //本地端測試用，有機率擊殺怪物
                                var random = new Unity.Mathematics.Random(Seed);
                                float value = random.NextFloat(); // 產生一個0.0到1.0之間的浮點數
                                if (value < 0.01f) {
                                    //在怪物實體身上建立死亡標籤元件，讓其他系統知道要死亡後該做什麼
                                    ECB.AddComponent<MonsterDieTag>(3, monsterValue.MyEntity);
                                    //在怪物實體身上建立移除標籤元件
                                    var autoDestroyTag = new AutoDestroyTag { LifeTime = 6 };
                                    ECB.AddComponent(3, monsterValue.MyEntity, autoDestroyTag);
                                    //目前不實做將死亡怪物從網格中移除，因為MonsterBehaviourSystem中每幀都會清空網格資料，所以各別移除就沒那麼需要
                                }
                                else {
                                    //在怪物實體身上建立被擊中的標籤元件，讓其他系統知道要處理被擊中後該做什麼
                                    var hitTag = new MonsterHitTag { MonsterID = monsterValue.MonsterID, StrIndex_SpellID = _collisionData.StrIndex_SpellID };
                                    ECB.AddComponent(3, monsterValue.MyEntity, hitTag);
                                }

                                //加入子彈擊中特效標籤元件
                                Entity effectEntity = ECB.CreateEntity(4);
                                var effectSpawnTag = new HitParticleSpawnTag { Monster = monsterValue, SpellPrefabID = _collisionData.SpellPrefabID, HitPos = _moveData.Position, HitDir = _moveData.Direction };
                                ECB.AddComponent(5, effectEntity, effectSpawnTag);

                                //加入擊中tag
                                if (_collisionData.EnableBulletHit) {
                                    Entity hitEntity = ECB.CreateEntity(6);
                                    var bulletHitTag = new SpellHitTag {
                                        Monster = monsterValue,
                                        StrIndex_SpellID = _collisionData.StrIndex_SpellID,
                                        HitPosition = monsterValue.Pos,
                                        HitDirection = _moveData.Direction
                                    };
                                    ECB.AddComponent(7, hitEntity, bulletHitTag);
                                }

                                if (_collisionData.Destroy)
                                    ECB.DestroyEntity(8, _entity);//銷毀子彈
                            }

                        } while (GridData.GridMap.TryGetNextValue(out monsterValue, ref iterator)); // 如果該key還有其他值就繼續
                    }
                }
            }

            private bool CheckAlreayHitMonster(Entity bullet, Entity monster, double time) {
                if (HitInfoLookup.TryGetBuffer(bullet, out DynamicBuffer<HitInfoBuffer> buffer)) {
                    for (int i = 0; i < buffer.Length; i++) {
                        if (buffer[i].MonsterEntity == monster && ElapsedTime - buffer[i].HitTime < time) {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}