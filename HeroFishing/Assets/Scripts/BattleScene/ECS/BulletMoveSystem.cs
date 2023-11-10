using Scoz.Func;
using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battle {
    //[CreateAfter(typeof(BulletBehaviourSystem))]
    //[UpdateAfter(typeof(BulletBehaviourSystem))]
    public partial struct BulletMoveSystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BulletInstance>();
            state.RequireForUpdate<MoveData>();
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            //遍歷所有怪物            
            foreach (var (moveData, bulletInstance) in SystemAPI.Query<RefRW<MoveData>, BulletInstance>()) {
                moveData.ValueRW.Position = moveData.ValueRO.Position + moveData.ValueRO.Direction * moveData.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                UnityEngine.Debug.Log(moveData.ValueRO.Position);
                bulletInstance.Trans.localPosition = moveData.ValueRO.Position;
            }
        }

    }
}

