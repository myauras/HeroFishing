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
                // �p�G�O�B�᪬�A�Q�����A���u�ɶ��N����
                if (isFrozen) {
                    monsterInstance.MyMonster.Explode();
                    //�b�Ǫ����騭�W�إ߲������Ҥ���
                    ecbWriter.AddComponent(entity.Index, entity, new AutoDestroyTag {
                        LifeTime = 1
                    });
                }
                else {
                    monsterInstance.MyMonster.Die();
                    //�b�Ǫ����騭�W�إ߲������Ҥ���
                    ecbWriter.AddComponent(entity.Index, entity, new AutoDestroyTag {
                        LifeTime = 2
                    });
                }
                ecbWriter.RemoveComponent<MonsterDieTag>(entity.Index, entity);
            }
        }
    }
}