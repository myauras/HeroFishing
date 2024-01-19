using Scoz.Func;
using Unity.Entities;


namespace HeroFishing.Battle {
    [CreateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial struct MonsterDieSystem : ISystem {
        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterInstance>();
            state.RequireForUpdate<MonsterDieTag>();
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state) {
        }

        public void OnUpdate(ref SystemState state) {

            var ecbWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            //bool isFrozen = SystemAPI.HasSingleton<FreezeTag>();
            foreach (var (monsterInstance, dieTag, entity) in SystemAPI.Query<MonsterInstance, MonsterDieTag>().WithEntityAccess()) {
                bool isFrozen = SystemAPI.HasComponent<MonsterFreezeTag>(entity);
                // �p�G�O�B�᪬�A�Q�����A���u�ɶ��N����
                if (isFrozen) {
                    monsterInstance.MyMonster.Explode(dieTag.HeroIndex);
                    //�b�Ǫ����騭�W�إ߲������Ҥ���
                    ecbWriter.AddComponent(entity.Index, entity, new AutoDestroyTag {
                        LifeTime = 1
                    });
                }
                else {
                    monsterInstance.MyMonster.Die(dieTag.HeroIndex);
                    //�b�Ǫ����騭�W�إ߲������Ҥ���
                    ecbWriter.AddComponent(entity.Index, entity, new AutoDestroyTag {
                        LifeTime = 3.5f
                    });
                }
                ecbWriter.RemoveComponent<MonsterDieTag>(entity.Index, entity);
            }
        }
    }
}