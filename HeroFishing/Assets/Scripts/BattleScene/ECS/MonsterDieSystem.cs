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
            //bool isFrozen = SystemAPI.HasSingleton<FreezeTag>();
            foreach (var (monsterInstance, _, entity) in SystemAPI.Query<MonsterInstance, MonsterDieTag>().WithEntityAccess()) {
                bool isFrozen = SystemAPI.HasComponent<MonsterFreezeTag>(entity);
                // 如果是冰凍狀態被擊殺，較短時間就消失
                if (isFrozen) {
                    monsterInstance.MyMonster.Explode();
                    //在怪物實體身上建立移除標籤元件
                    ecbWriter.AddComponent(entity.Index, entity, new AutoDestroyTag {
                        LifeTime = 1
                    });
                }
                else {
                    monsterInstance.MyMonster.Die();
                    //在怪物實體身上建立移除標籤元件
                    ecbWriter.AddComponent(entity.Index, entity, new AutoDestroyTag {
                        LifeTime = 2
                    });
                }
                ecbWriter.RemoveComponent<MonsterDieTag>(entity.Index, entity);
            }
        }
    }
}