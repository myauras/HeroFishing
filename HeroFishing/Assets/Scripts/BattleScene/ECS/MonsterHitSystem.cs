using Scoz.Func;
using Unity.Entities;


namespace HeroFishing.Battle {
    public partial struct MonsterHitSystem : ISystem {
        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterInstance>();
            state.RequireForUpdate<MonsterHitTag>();
        }

        public void OnDestroy(ref SystemState state) {
        }

        public void OnUpdate(ref SystemState state) {
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecbWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            foreach (var (monsterInstance, _, entity) in SystemAPI.Query<MonsterInstance, MonsterHitTag>().WithEntityAccess()) {
                monsterInstance.MyMonster.OnHit();
                ecbWriter.RemoveComponent<MonsterHitTag>(entity.Index, entity);
            }
        }
    }
}