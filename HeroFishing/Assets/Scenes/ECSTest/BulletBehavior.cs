using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.XR;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;
using static UnityEngine.EventSystems.EventTrigger;

namespace HeroFishing.Battle {
    public partial struct BulletBehavior : ISystem {


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletValue>();
            state.RequireForUpdate<MonsterValue>();

        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            float deltaTime = SystemAPI.Time.DeltaTime;
            var monsters = new NativeList<MonsterValue>(Allocator.TempJob);
            var monsterTrans = new NativeList<LocalTransform>(Allocator.TempJob);
            foreach (var (monster, trans) in SystemAPI.Query<MonsterValue, LocalTransform>()) {
                monsters.Add(monster);
                monsterTrans.Add(trans);
            }

            new MoveJob {
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BulletHitEntity = SystemAPI.GetSingleton<BulletSpawner>().BulletHitEntity,
                DeltaTime = deltaTime,
                Monsters = monsters,
                MonsterTrans = monsterTrans,
            }.ScheduleParallel();

        }
        [BurstCompile]
        partial struct MoveJob : IJobEntity {
            public EntityCommandBuffer.ParallelWriter ECB;
            [ReadOnly] public Entity BulletHitEntity;
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public NativeList<MonsterValue> Monsters;
            [ReadOnly] public NativeList<LocalTransform> MonsterTrans;

            public void Execute(ref LocalTransform _trans, in BulletValue _bullet, in Entity _entity) {
                _trans.Position += (_bullet.Speed * _bullet.Direction) * DeltaTime;

                for (int i = 0; i < Monsters.Length; i++) {
                    float dist = math.distance(MonsterTrans[i].Position, _trans.Position);
                    if (dist < (_bullet.Radius + Monsters[i].Radius)) {
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