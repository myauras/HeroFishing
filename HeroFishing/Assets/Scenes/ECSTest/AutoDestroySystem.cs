
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

namespace HeroFishing.Battlefield {
    public partial struct AutoDestroySystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AutoDestroyTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            float deltaTime = SystemAPI.Time.DeltaTime;


            foreach (var (autoDestroyCom, entity) in SystemAPI.Query<RefRW<AutoDestroyTag>>().WithEntityAccess()) {
                autoDestroyCom.ValueRW.ExistTime += deltaTime;

                if (autoDestroyCom.ValueRW.ExistTime >= autoDestroyCom.ValueRW.LifeTime) {
                    var job = new DestroyJob {
                        ECBWriter = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                        ReadyToDestroiedEntity = entity,
                    }.Schedule();
                    job.Complete();
                }
            }
        }

        [BurstCompile]
        partial struct DestroyJob : IJob {

            public EntityCommandBuffer.ParallelWriter ECBWriter;
            [ReadOnly] public Entity ReadyToDestroiedEntity;


            public void Execute() {
                ECBWriter.DestroyEntity(ReadyToDestroiedEntity.Index, ReadyToDestroiedEntity);
            }
        }


    }
}