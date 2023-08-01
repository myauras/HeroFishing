using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
            var monsterValues = new NativeList<MonsterValue>(Allocator.TempJob); ;
            foreach (var monsterValue in SystemAPI.Query<MonsterValue>()) {
                monsterValues.Add(monsterValue);
            }

            new MoveJob {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletHitEntity = SystemAPI.GetSingleton<BulletSpawner>().BulletHitEntity,
                DeltaTime = deltaTime,
                MonsterValues = monsterValues,
            }.ScheduleParallel();

        }
        [BurstCompile]
        partial struct MoveJob : IJobEntity {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public Entity BulletHitEntity;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public NativeList<MonsterValue> MonsterValues;

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

                        ECB.DestroyEntity(3, _entity);//銷毀子彈
                    }
                }

            }
        }



    }




}