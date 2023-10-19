
using Scoz.Func;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace HeroFishing.Battle {

    [CreateAfter(typeof(BulletSpawnSystem))]
    [UpdateAfter(typeof(BulletSpawnSystem))]
    public partial struct SpellEffectSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SpellParticleSpawnTag>();
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        }

        public void OnUpdate(ref SystemState state) {

            var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (particleSpawn, entity) in SystemAPI.Query<SpellParticleSpawnTag>().WithEntityAccess()) {
                //產生特效
                string hitPath = string.Format("Bullet/BulletHit{0}", particleSpawn.PrefabID);
                Quaternion rotQuaternion = new Quaternion(particleSpawn.Rot.x, particleSpawn.Rot.y, particleSpawn.Rot.z, particleSpawn.Rot.w);
                GameObjSpawner.SpawnParticleObjByPath(hitPath, particleSpawn.Pos, rotQuaternion, null, null);
                //移除Tag
                ECB.DestroyEntity(entity);
            }

        }
    }
}