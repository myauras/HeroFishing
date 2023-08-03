using Scoz.Func;
using Unity.Entities;

namespace HeroFishing.Battle {
    public partial struct MonsterBehaviourSystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterInstance>();

        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (monsterValue, monsterInstance) in SystemAPI.Query<RefRW<MonsterValue>, MonsterInstance>().WithAbsent<AutoDestroyTag>()) {
                //怪物移動
                if (monsterInstance.MyMonster.MyData.Speed != 0) {
                    monsterInstance.Trans.localPosition += (monsterInstance.Dir * monsterInstance.MyMonster.MyData.Speed) * deltaTime;
                    monsterValue.ValueRW.Pos = monsterInstance.Trans.localPosition;
                }
            }
        }
    }

}

