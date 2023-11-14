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
                if (moveData.ValueRO.TargetMonster.MyEntity == Entity.Null) {
                    moveData.ValueRW.Position = moveData.ValueRO.Position + moveData.ValueRO.Direction * moveData.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                    bulletInstance.Trans.localPosition = moveData.ValueRO.Position;
                }
                else {
                    var targetPos = moveData.ValueRO.TargetMonster.Pos;
                    var direction = math.normalize(targetPos - moveData.ValueRO.Position);
                    direction.y = 0;
                    moveData.ValueRW.Direction = direction;
                    moveData.ValueRW.Position = moveData.ValueRO.Position + moveData.ValueRO.Direction * moveData.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                    bulletInstance.Trans.localPosition = moveData.ValueRO.Position;
                    bulletInstance.Trans.localRotation = quaternion.LookRotationSafe(direction, math.up());
                }
            }
        }

    }
}

