using Scoz.Func;
using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battle {
    [CreateAfter(typeof(BulletBehaviourSystem))]
    [UpdateAfter(typeof(BulletBehaviourSystem))]
    public partial struct BulletMoveSystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletInstance>();
            state.RequireForUpdate<BulletValue>();
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            //遍歷所有怪物            
            foreach (var (bulletValue, bulletInstance) in SystemAPI.Query<RefRO<BulletValue>, BulletInstance>()) {
                bulletInstance.Trans.localPosition = bulletValue.ValueRO.Position;
            }
        }

    }
}

