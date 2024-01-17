
using Scoz.Func;
using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace HeroFishing.Battle {

    [CreateAfter(typeof(BulletSpawnSystem))]
    [UpdateAfter(typeof(BulletSpawnSystem))]
    [CreateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial struct EffectSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ParticleSpawnTag>();
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        }

        public void OnUpdate(ref SystemState state) {

            var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (particleSpawn, entity) in SystemAPI.Query<ParticleSpawnTag>().WithEntityAccess()) {
                //產生特效
                var path = ECSStrManager.GetStr(particleSpawn.StrIndex_ParticlePath);
                Quaternion rotQuaternion = new Quaternion(particleSpawn.Rot.x, particleSpawn.Rot.y, particleSpawn.Rot.z, particleSpawn.Rot.w);
                GameObjSpawner.SpawnParticleObjByPath(path, particleSpawn.Pos, rotQuaternion, null, null);
                //移除Tag
                ECB.DestroyEntity(entity);
            }

        }
    }
}