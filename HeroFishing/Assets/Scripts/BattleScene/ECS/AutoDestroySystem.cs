
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

namespace HeroFishing.Battle {
    public partial struct AutoDestroySystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
        NativeList<Entity> EntitiesToProcess;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AutoDestroyTag>();
            EntitiesToProcess = new NativeList<Entity>(Allocator.Persistent);

        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
            EntitiesToProcess.Dispose();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            float deltaTime = SystemAPI.Time.DeltaTime;

            new DestroyJob {
                ECBWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                DeltaTime = deltaTime,
            }.ScheduleParallel();

        }

        [BurstCompile]
        partial struct DestroyJob : IJobEntity {
            public EntityCommandBuffer.ParallelWriter ECBWriter;
            [ReadOnly] public float DeltaTime;

            public void Execute(ref AutoDestroyTag _autoDestroy, in Entity _entity) {
                _autoDestroy.ExistTime += DeltaTime;
                if (_autoDestroy.ExistTime >= _autoDestroy.LifeTime)
                    ECBWriter.DestroyEntity(_entity.Index, _entity);
            }
        }

    }
}