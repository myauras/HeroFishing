using Scoz.Func;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


namespace HeroFishing.Battle {
    [CreateAfter(typeof(MonsterBehaviourSystem))]
    [UpdateAfter(typeof(MonsterBehaviourSystem))]
    public partial struct BulletBehaviourSystem : ISystem {

        //NativeArray<MonsterValue> MonsterValues;
        //NativeArray<Entity> MonsterEntities;
        //EntityQuery MonsterQuery;
        MapGridData GridData; //網格資料

        //[BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletValue>();
            //MonsterQuery = state.GetEntityQuery(typeof(MonsterValue), ComponentType.Exclude<AutoDestroyTag>());
            GridData = SystemAPI.GetSingleton<MapGridData>();
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state) {

        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state) {


            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            float deltaTime = SystemAPI.Time.DeltaTime;

            //MonsterValues = MonsterQuery.ToComponentDataArray<MonsterValue>(Allocator.TempJob);
            //MonsterEntities = MonsterQuery.ToEntityArray(Allocator.TempJob);


            uint seed = (uint)(deltaTime * 1000000f);

            new MoveJob {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletHitEntity = SystemAPI.GetSingleton<BulletSpawner>().BulletHitEntity,
                DeltaTime = deltaTime,
                //MonsterValues = MonsterValues,
                //MonsterEntities = MonsterEntities,
                Seed = seed,
                GridData = GridData,
            }.ScheduleParallel();


            //釋放資源
            //MonsterValues.Dispose(state.Dependency);
            //MonsterEntities.Dispose(state.Dependency);


        }
        //[BurstCompile]
        partial struct MoveJob : IJobEntity {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public Entity BulletHitEntity;
            [ReadOnly] public float DeltaTime;
            //[ReadOnly] public NativeArray<MonsterValue> MonsterValues;
            //[ReadOnly] public NativeArray<Entity> MonsterEntities;
            [ReadOnly] public uint Seed;
            [ReadOnly] public MapGridData GridData;

            public void Execute(ref LocalTransform _trans, in BulletValue _bullet, in Entity _entity) {

                _trans.Position += (_bullet.Speed * _bullet.Direction) * DeltaTime;

                // 計算子彈的網格索引
                int2 gridIndex = new int2(
                    (int)(_trans.Position.x / GridData.CellSize),
                    (int)(_trans.Position.z / GridData.CellSize)
                );

                MonsterValue monsterValue;
                if (GridData.GridMap.TryGetFirstValue(gridIndex, out monsterValue, out var iterator)) {
                    // 這裡放第一個找到的value要做的事情
                    do {
                        // 使用當前找到的value要做某些事情
                        float dist = math.distance(monsterValue.Pos, _trans.Position);
                        if (dist < (_bullet.Radius + monsterValue.Radius)) {//怪物在子彈的命中範圍內

                            //創造擊中特效並更改位置
                            var particleEntity = ECB.Instantiate(1, BulletHitEntity);
                            ECB.SetComponent(2, particleEntity, new LocalTransform {
                                Position = _trans.Position,
                                Rotation = Quaternion.identity,
                                Scale = 1,
                            });


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
                            } else {
                                //在怪物實體身上建立被擊中的標籤元件，讓其他系統知道要處理被擊中後該做什麼
                                ECB.AddComponent<MonsterHitTag>(3, monsterValue.MyEntity);
                            }

                            ECB.DestroyEntity(4, _entity);//銷毀子彈
                        }

                    } while (GridData.GridMap.TryGetNextValue(out monsterValue, ref iterator)); // 如果該key還有其他值就繼續
                }



                //for (int i = 0; i < MonsterValues.Length; i++) {
                //    float dist = math.distance(MonsterValues[i].Pos, _trans.Position);
                //    if (dist < (_bullet.Radius + MonsterValues[i].Radius)) {//怪物在子彈的命中範圍內

                //        //創造擊中特效並更改位置
                //        var particleEntity = ECB.Instantiate(1, BulletHitEntity);
                //        ECB.SetComponent(2, particleEntity, new LocalTransform {
                //            Position = _trans.Position,
                //            Rotation = Quaternion.identity,
                //            Scale = 1,
                //        });


                //        //本地端測試用，有機率擊殺怪物
                //        var random = new Unity.Mathematics.Random(Seed);
                //        float value = random.NextFloat(); // 產生一個0.0到1.0之間的浮點數
                //        if (value < 0.01f) {
                //            //在怪物實體身上建立死亡標籤元件，讓其他系統知道要死亡後該做什麼
                //            ECB.AddComponent<MonsterDieTag>(3, MonsterEntities[i]);
                //            //在怪物實體身上建立移除標籤元件
                //            var autoDestroyTag = new AutoDestroyTag { LifeTime = 6 };
                //            ECB.AddComponent(3, MonsterEntities[i], autoDestroyTag);
                //        } else {
                //            //在怪物實體身上建立被擊中的標籤元件，讓其他系統知道要處理被擊中後該做什麼
                //            ECB.AddComponent<MonsterHitTag>(3, MonsterEntities[i]);
                //        }

                //        ECB.DestroyEntity(4, _entity);//銷毀子彈
                //    }
                //}


            }
        }



    }




}