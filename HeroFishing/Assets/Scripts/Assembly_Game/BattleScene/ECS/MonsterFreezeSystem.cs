//using Unity.Burst;
//using Unity.Entities;

//public struct MonsterFreezeTag : IComponentData { }

//public partial struct MonsterFreezeSystem : ISystem {
//    private bool _isFrozen;
//    public void OnCreate(ref SystemState state) {
//        state.RequireForUpdate<MonsterInstance>();
//    }

//    public void OnDestroy(ref SystemState state) {

//    }

//    public void OnUpdate(ref SystemState state) {
//        bool freeze = SystemAPI.HasSingleton<FreezeTag>();
//        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
//        if (_isFrozen != freeze) {
//            _isFrozen = freeze;
//            foreach (var (monster, entity) in SystemAPI.Query<MonsterInstance>().WithAbsent<AutoDestroyTag>().WithEntityAccess()) {
//                if (_isFrozen) {
//                    monster.MyMonster.Freeze();
//                    ecb.AddComponent<MonsterFreezeTag>(entity);
//                }
//                else {
//                    monster.MyMonster.UnFreeze();
//                    ecb.RemoveComponent<MonsterFreezeTag>(entity);
//                }
//            }
//        }

//        ecb.Playback(state.EntityManager);
//        ecb.Dispose();
//    }
//}
