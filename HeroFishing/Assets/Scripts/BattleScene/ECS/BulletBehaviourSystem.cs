using Scoz.Func;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;


namespace HeroFishing.Battle {
    public partial struct BulletBehaviourSystem : ISystem {



        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletValue>();
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            float deltaTime = SystemAPI.Time.DeltaTime;
            var monsterValues = new NativeList<MonsterValue>(Allocator.TempJob);
            var monsterEntities = new NativeList<Entity>(Allocator.TempJob); ;

            foreach (var (monsterValue, entity) in SystemAPI.Query<MonsterValue>().WithAbsent<AutoDestroyTag>().WithEntityAccess()) {
                monsterValues.Add(monsterValue);
                monsterEntities.Add(entity);
            }

            uint seed = (uint)(deltaTime * 1000000f);

            new MoveJob {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletHitEntity = SystemAPI.GetSingleton<BulletSpawner>().BulletHitEntity,
                DeltaTime = deltaTime,
                MonsterValues = monsterValues,
                MonsterEntities = monsterEntities,
                Seed = seed,
            }.ScheduleParallel();

            //釋放資源
            monsterValues.Dispose(state.Dependency);
            monsterEntities.Dispose(state.Dependency);

        }
        [BurstCompile]
        partial struct MoveJob : IJobEntity {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public Entity BulletHitEntity;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public NativeList<MonsterValue> MonsterValues;
            [ReadOnly] public NativeList<Entity> MonsterEntities;
            [ReadOnly] public uint Seed;

            public void Execute(ref LocalTransform _trans, in BulletValue _bullet, in Entity _entity) {
                _trans.Position += (_bullet.Speed * _bullet.Direction) * DeltaTime;
                for (int i = 0; i < MonsterValues.Length; i++) {
                    float dist = math.distance(MonsterValues[i].Pos, _trans.Position);
                    if (dist < (_bullet.Radius + MonsterValues[i].Radius)) {//怪物在子彈的命中範圍內

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
                            ECB.AddComponent<MonsterDieTag>(3, MonsterEntities[i]);
                            //在怪物實體身上建立移除標籤元件
                            var autoDestroyTag = new AutoDestroyTag { LifeTime = 6 };
                            ECB.AddComponent(3, MonsterEntities[i], autoDestroyTag);
                        } else {
                            //在怪物實體身上建立被擊中的標籤元件，讓其他系統知道要處理被擊中後該做什麼
                            ECB.AddComponent<MonsterHitTag>(3, MonsterEntities[i]);
                        }

                        ECB.DestroyEntity(4, _entity);//銷毀子彈
                    }
                }

            }
        }



    }




}