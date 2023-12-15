using Scoz.Func;
using Unity.Entities;


namespace HeroFishing.Battle {
    public partial struct MonsterDieSystem : ISystem {
        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterInstance>();
            state.RequireForUpdate<MonsterDieTag>();
        }

        public void OnDestroy(ref SystemState state) {
        }

        public void OnUpdate(ref SystemState state) {
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecbWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            bool isFrozen = SystemAPI.HasSingleton<FreezeTag>();
            foreach (var (monsterInstance, _, entity) in SystemAPI.Query<MonsterInstance, MonsterDieTag>().WithEntityAccess()) {
                //var bombTest = monsterInstance.MyMonster.GetComponentInChildren<BombTest>();
                if (isFrozen) {
                    monsterInstance.MyMonster.Explode();
                }
                else
                    monsterInstance.MyMonster.Die();
                ecbWriter.RemoveComponent<MonsterDieTag>(entity.Index, entity);
            }
        }
    }
}