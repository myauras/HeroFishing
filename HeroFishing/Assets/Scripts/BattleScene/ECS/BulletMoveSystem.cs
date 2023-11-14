using Scoz.Func;
using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
            foreach (var (moveData, bulletInstance) in SystemAPI.Query<RefRW<MoveData>, BulletInstance>()) {
                if (state.EntityManager.Exists(moveData.ValueRO.TargetMonster.MyEntity)) {
                    bulletInstance.Trans.localPosition = moveData.ValueRO.Position;
                }
                else {
                    bulletInstance.Trans.localPosition = moveData.ValueRO.Position;
                    bulletInstance.Trans.localRotation = quaternion.LookRotationSafe(moveData.ValueRO.Direction, math.up());
                }
            }
        }

    }
}

