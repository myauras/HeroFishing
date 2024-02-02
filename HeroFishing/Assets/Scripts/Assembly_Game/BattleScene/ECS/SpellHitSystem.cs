//using HeroFishing.Main;
//using System.Linq;
//using Unity.Burst;
//using Unity.Entities;

//[CreateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
//public partial struct SpellHitSystem : ISystem {
//    EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
//    public void OnCreate(ref SystemState state) {
//        state.RequireForUpdate<SpellHitTag>();
//        ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
//    }

//    public void OnDestroy(ref SystemState state) {

//    }

//    public void OnUpdate(ref SystemState state) {

//        var ecbWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
//        foreach (var (hitTag, entity) in SystemAPI.Query<SpellHitTag>().WithEntityAccess()) {
//            // 取得hit的spell data
//            var path = ECSStrManager.GetStr(hitTag.StrIndex_SpellID);
//            string spellID = new string(path.ToArray());
//            var data = HeroSpellJsonData.GetData(spellID);
//            // 如果data有chain，進行OnHit
//            if (data != null && data.MyHitType != HeroSpellJsonData.HitType.None) {
//                data.Spell.OnHit(ecbWriter, hitTag);
//            }
//            ecbWriter.DestroyEntity(entity.Index, entity);
//        }
//    }
//}
