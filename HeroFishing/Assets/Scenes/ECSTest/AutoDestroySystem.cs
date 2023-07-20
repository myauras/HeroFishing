
using Unity.Burst;
using Unity.Entities;

namespace HeroFishing.Battlefield
{
    public partial struct AutoDestroySystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (autoDestroyCom, entity) in SystemAPI.Query<RefRW<AutoDestroyTag>>().WithEntityAccess())
            {
                autoDestroyCom.ValueRW.ExistTime += deltaTime;
                if (autoDestroyCom.ValueRW.ExistTime >= autoDestroyCom.ValueRW.LifeTime)
                {
                    ecb.DestroyEntity(entity);
                }
            }

        }
    }
}