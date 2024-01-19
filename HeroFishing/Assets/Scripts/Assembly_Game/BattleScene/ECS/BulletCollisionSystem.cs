using HeroFishing.Main;
using HeroFishing.Socket;
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
        [ReadOnly] EntityStorageInfoLookup StorageInfoLookup;
        [ReadOnly] ComponentLookup<MonsterValue> MonsterValueLookup;
        [ReadOnly] ComponentLookup<AutoDestroyTag> AutoDestroyTagLookup;

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
            MonsterValueLookup = state.GetComponentLookup<MonsterValue>(true);
            AutoDestroyTagLookup = state.GetComponentLookup<AutoDestroyTag>(true);
            StorageInfoLookup = state.GetEntityStorageInfoLookup();
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
            OffsetGrids.Dispose();
        }

        public void OnUpdate(ref SystemState state) {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            float deltaTime = SystemAPI.Time.DeltaTime;
            double elapsedTime = SystemAPI.Time.ElapsedTime;
            bool localTest = SystemAPI.HasSingleton<LocalTestSys>();
            uint seed = (uint)(deltaTime * 1000000f);
            //取得網格資料
            var gridData = SystemAPI.GetSingleton<MapGridData>();

            HitInfoLookup.Update(ref state);
            StorageInfoLookup.Update(ref state);
            MonsterValueLookup.Update(ref state);
            AutoDestroyTagLookup.Update(ref state);

            new MoveJob {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                DeltaTime = deltaTime,
                ElapsedTime = elapsedTime,
                Seed = seed,
                GridData = gridData,
                OffsetGrids = OffsetGrids,
                MonsterCollisionPosOffset = BattleManager.MonsterCollisionPosOffset,
                HitInfoLookup = HitInfoLookup,
                StorageInfoLookup = StorageInfoLookup,
                MonsterValueLookup = MonsterValueLookup,
                AutoDestroyTagLookup = AutoDestroyTagLookup,
                IsNetwork = !localTest,
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
            [ReadOnly] public ComponentLookup<MonsterValue> MonsterValueLookup;
            [ReadOnly] public ComponentLookup<AutoDestroyTag> AutoDestroyTagLookup;
            [ReadOnly] public bool IsNetwork;

            public void Execute(ref BulletCollisionData _collisionData, ref MoveData _moveData, in Entity _entity) {
                _collisionData.Timer += DeltaTime;
                if (_collisionData.Timer < _collisionData.Delay) {
                    return;
                }

                // 計算移動，如果有目標則朝著目標，沒有的話走直線，另外也檢查是否死亡。
                bool hasTargetMonster = StorageInfoLookup.Exists(_moveData.TargetMonster.MyEntity);
                // 如果直接取用的話 _moveData.TargetMonster.Pos 不會持續更新，需要用lookup拿最新的monster value
                bool hasMonsterValue = MonsterValueLookup.TryGetComponent(_moveData.TargetMonster.MyEntity, out var targetMonster);
                // 查看目標是否死亡，若是死亡，關閉target monster
                bool isMonsterDead = AutoDestroyTagLookup.HasComponent(_moveData.TargetMonster.MyEntity);
                if (isMonsterDead)
                    hasTargetMonster = false;

                if (!hasTargetMonster || !hasMonsterValue) {
                    _moveData.Position += _moveData.Direction * _moveData.Speed * DeltaTime;
                }
                else {
                    //var targetPos = _moveData.TargetMonster.Pos;
                    var targetPos = targetMonster.Pos;
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

                IsNetwork &= _collisionData.HeroIndex == 0;
                Entity networkEntity = Entity.Null;

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

                            // 如果是該目標，但是已經死亡，也不碰撞
                            if (isMonsterDead && _moveData.TargetMonster.MyEntity == monsterValue.MyEntity) {
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

                                //加入本地擊中標籤，死亡標籤在擊中系統中處理，這樣才能在怪物死亡時知道最後的擊中方向。
                                var hitTag = new MonsterHitTag {
                                    HeroIndex = _collisionData.HeroIndex,
                                    MonsterID = monsterValue.MonsterID,
                                    StrIndex_SpellID = _collisionData.StrIndex_SpellID,
                                    HitDirection = _moveData.Direction
                                };
                                ECB.AddComponent(3, monsterValue.MyEntity, hitTag);

                                //加入子彈擊中特效標籤元件
                                Entity effectEntity = ECB.CreateEntity(4);
                                var effectSpawnTag = new HitParticleSpawnTag { Monster = monsterValue, SpellPrefabID = _collisionData.SpellPrefabID, HitPos = _moveData.Position, HitDir = _moveData.Direction };
                                ECB.AddComponent(5, effectEntity, effectSpawnTag);

                                //加入擊中tag
                                if (!_collisionData.IsSub) {
                                    var position = _moveData.Position;
                                    position.y = 0;
                                    Entity hitEntity = ECB.CreateEntity(6);
                                    var bulletHitTag = new SpellHitTag {
                                        AttackID = _collisionData.AttackID,
                                        Monster = monsterValue,
                                        StrIndex_SpellID = _collisionData.StrIndex_SpellID,
                                        HitPosition = monsterValue.Pos,
                                        HitDirection = _moveData.Direction,
                                        BulletPosition = position,
                                    };
                                    ECB.AddComponent(7, hitEntity, bulletHitTag);
                                }

                                if (IsNetwork) {
                                    if (networkEntity == Entity.Null) {
                                        networkEntity = ECB.CreateEntity(0);
                                        ECB.AddComponent(1, networkEntity, new SpellHitNetworkData {
                                            AttackID = _collisionData.AttackID,
                                            StrIndex_SpellID = _collisionData.StrIndex_SpellID
                                        });
                                        ECB.AddBuffer<MonsterHitNetworkData>(1, networkEntity);
                                    }
                                    ECB.AppendToBuffer(9, networkEntity, new MonsterHitNetworkData {
                                        Monster = monsterValue,
                                    });
                                }

                                if (_collisionData.Destroy) {
                                    ECB.DestroyEntity(8, _entity);//銷毀子彈                                    
                                    return;
                                }
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