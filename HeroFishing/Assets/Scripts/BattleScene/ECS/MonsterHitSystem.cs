using HeroFishing.Main;
using Scoz.Func;
using System.Linq;
using Unity.Collections;
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
            foreach (var (monsterInstance, hitTag, entity) in SystemAPI.Query<MonsterInstance, MonsterHitTag>().WithEntityAccess()) {
                var path = ECSStrManager.GetStr(hitTag.StrIndex_SpellID);
                string spellID = new string(path.ToArray());
                monsterInstance.MyMonster.OnHit(spellID);
                ecbWriter.RemoveComponent<MonsterHitTag>(entity.Index, entity);
            }
        }
    }
}