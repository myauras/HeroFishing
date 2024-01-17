//using System.Diagnostics;
//using System.Xml;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Physics;
//using Unity.Physics.Systems;
//using Unity.Transforms;

//namespace HeroFishing.Battle {

//    [UpdateInGroup(typeof(PhysicsSystemGroup))]
//    [UpdateAfter(typeof(PhysicsSimulationGroup))]
//    public partial struct CollisionSystem : ISystem {

//        [BurstCompile]
//        public void OnCreate(ref SystemState state) {
//            state.RequireForUpdate<CollisionSys>();
//            state.RequireForUpdate<SimulationSingleton>();
//            state.RequireForUpdate<BulletValue>();
//        }
//        [BurstCompile]
//        public void OnUpdate(ref SystemState state) {

//            BulletSpawner bulletSpawner = null;
//            foreach (var spawner in
//                     SystemAPI.Query<BulletSpawner>()) {
//                bulletSpawner = spawner;
//            }
//            if (bulletSpawner == null) return;

//            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
//            state.Dependency = new TriggerJob {
//                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
//                BulletHitEntity = bulletSpawner.SpellEntitieMap[0].HitEntity,
//                BulletComs = SystemAPI.GetComponentLookup<BulletValue>(isReadOnly: true),
//                MonsterComs = SystemAPI.GetComponentLookup<MonsterValue>(isReadOnly: true),
//                TransformComponents = SystemAPI.GetComponentLookup<LocalTransform>(),
//            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
//        }

//        [BurstCompile]
//        struct TriggerJob : ITriggerEventsJob {

//            public EntityCommandBuffer ECB;
//            public Entity BulletHitEntity;
//            [ReadOnly] public ComponentLookup<BulletValue> BulletComs;
//            [ReadOnly] public ComponentLookup<MonsterValue> MonsterComs;
//            public ComponentLookup<LocalTransform> TransformComponents;

//            public void Execute(TriggerEvent triggerEvent) {
//                if (BulletComs.HasComponent(triggerEvent.EntityA)) {// 如果是子彈
//                    if (MonsterComs.HasComponent(triggerEvent.EntityB)) {// 如果子彈的碰撞目標是怪物

//                        //創造擊中特效並更改位置
//                        var particleEntity = ECB.Instantiate(BulletHitEntity);
//                        var bulletTrans = TransformComponents[triggerEvent.EntityA];
//                        ECB.SetComponent(particleEntity, new LocalTransform {
//                            Position = bulletTrans.Position,
//                            Scale = bulletTrans.Scale,
//                        });

//                        ECB.DestroyEntity(triggerEvent.EntityA);//銷毀子彈
//                    }
//                } else if (MonsterComs.HasComponent(triggerEvent.EntityA)) {// 如果是怪物
//                    if (BulletComs.HasComponent(triggerEvent.EntityB)) {// 如果怪物的碰撞目標是子彈
//                        ECB.DestroyEntity(triggerEvent.EntityB);
//                    }
//                }
//            }
//        }
//    }
//}