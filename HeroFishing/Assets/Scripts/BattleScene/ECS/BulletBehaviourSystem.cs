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
    public partial struct BulletBehaviourSystem : ISystem {

        NativeArray<int2> OffsetGrids;//定義碰撞檢定9宮格


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletValue>();

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

        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
            OffsetGrids.Dispose();
        }

        public void OnUpdate(ref SystemState state) {


            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            float deltaTime = SystemAPI.Time.DeltaTime;


            uint seed = (uint)(deltaTime * 1000000f);
            //取得網格資料
            var gridData = SystemAPI.GetSingleton<MapGridData>();


            new MoveJob {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                DeltaTime = deltaTime,
                Seed = seed,
                GridData = gridData,
                OffsetGrids = OffsetGrids,
                MonsterCollisionPosOffset = BattleManager.MonsterCollisionPosOffset,
            }.ScheduleParallel();



        }
        [BurstCompile]
        partial struct MoveJob : IJobEntity {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public uint Seed;
            [ReadOnly] public MapGridData GridData;
            [ReadOnly] public NativeArray<int2> OffsetGrids;
            [ReadOnly] public float3 MonsterCollisionPosOffset;

            public void Execute(ref BulletValue _bullet, in Entity _entity) {

                _bullet.Position += (_bullet.Speed * _bullet.Direction) * DeltaTime;

                // 計算子彈的網格索引
                int2 gridIndex = new int2(
                    (int)(_bullet.Position.x / GridData.CellSize),
                    (int)(_bullet.Position.z / GridData.CellSize)
                );



                foreach (var offset in OffsetGrids) {
                    int2 gridToCheck = gridIndex + offset;

                    MonsterValue monsterValue;
                    if (GridData.GridMap.TryGetFirstValue(gridToCheck, out monsterValue, out var iterator)) {
                        // 這裡放第一個找到的value要做的事情
                        do {
                            // 使用當前找到的value要做某些事情
                            float dist = math.distance(monsterValue.Pos + MonsterCollisionPosOffset, _bullet.Position);
                            if (dist < (_bullet.Radius + monsterValue.Radius)) {//怪物在子彈的命中範圍內

                                //本地端測試用，有機率擊殺怪物
                                var random = new Unity.Mathematics.Random(Seed);
                                float value = random.NextFloat(); // 產生一個0.0到1.0之間的浮點數
                                if (value < 0.5f) {
                                    //在怪物實體身上建立死亡標籤元件，讓其他系統知道要死亡後該做什麼
                                    ECB.AddComponent<MonsterDieTag>(3, monsterValue.MyEntity);
                                    //在怪物實體身上建立移除標籤元件
                                    var autoDestroyTag = new AutoDestroyTag { LifeTime = 6 };
                                    ECB.AddComponent(3, monsterValue.MyEntity, autoDestroyTag);
                                    //目前不實做將死亡怪物從網格中移除，因為MonsterBehaviourSystem中每幀都會清空網格資料，所以各別移除就沒那麼需要
                                } else {
                                    //在怪物實體身上建立被擊中的標籤元件，讓其他系統知道要處理被擊中後該做什麼
                                    var hitTag = new MonsterHitTag { MonsterID = monsterValue.MonsterID, StrIndex_SpellID = _bullet.StrIndex_SpellID };
                                    ECB.AddComponent(3, monsterValue.MyEntity, hitTag);

                                }

                                //加入子彈擊中特效標籤元件
                                Entity effectEntity = ECB.CreateEntity(4);
                                var effectSpawnTag = new HitParticleSpawnTag { Monster = monsterValue, SpellPrefabID = _bullet.SpellPrefabID, Bullet = _bullet };
                                ECB.AddComponent(5, effectEntity, effectSpawnTag);

                                ECB.DestroyEntity(6, _entity);//銷毀子彈
                            }

                        } while (GridData.GridMap.TryGetNextValue(out monsterValue, ref iterator)); // 如果該key還有其他值就繼續
                    }

                }


            }
        }



    }




}