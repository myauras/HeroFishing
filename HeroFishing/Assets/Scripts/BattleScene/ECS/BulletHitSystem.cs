using HeroFishing.Main;
using System.Linq;
using Unity.Burst;
using Unity.Entities;

public partial struct BulletHitSystem : ISystem {
    EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BulletHitTag>();
    }

    public void OnDestroy(ref SystemState state) {

    }

    public void OnUpdate(ref SystemState state) {
        ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecbWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        foreach (var (hitTag, entity) in SystemAPI.Query<BulletHitTag>().WithEntityAccess()) {
            // 取得hit的spell data
            var path = ECSStrManager.GetStr(hitTag.StrIndex_SpellID);
            string spellID = new string(path.ToArray());
            var data = HeroSpellJsonData.GetData(spellID);
            // 如果data有chain，進行OnHit
            if (data != null && data.MyHitType != HeroSpellJsonData.HitType.None) {
                data.Spell.OnHit(ecbWriter, hitTag);
            }
            ecbWriter.DestroyEntity(entity.Index, entity);
        }
    }
}
