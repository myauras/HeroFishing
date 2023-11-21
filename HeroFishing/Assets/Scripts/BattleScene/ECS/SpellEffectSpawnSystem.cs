
using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HeroFishing.Battle {

    [CreateAfter(typeof(BulletSpawnSystem))]
    [UpdateAfter(typeof(BulletSpawnSystem))]
    public partial struct SpellEffectSpawnSystem : ISystem {

        EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<HitParticleSpawnTag>();
            ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        }

        public void OnUpdate(ref SystemState state) {

            var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (particleSpawn, entity) in SystemAPI.Query<HitParticleSpawnTag>().WithEntityAccess()) {
                var monsterData = MonsterJsonData.GetData(particleSpawn.Monster.MonsterID);
                if (monsterData == null) continue;
                string hitPath = string.Format("Bullet/BulletHit{0}", particleSpawn.SpellPrefabID);
                var rotQuaternion =
                    //Quaternion.Euler(particleSpawn.HitDir);
                    quaternion.LookRotationSafe(particleSpawn.HitDir, math.up());
                var path = string.Format("Assets/AddressableAssets/Particles/{0}.prefab", hitPath);
                var pool = PoolManager.Instance;

                Vector3 position = Vector3.zero;
                switch (monsterData.HitEffectPos) {
                    case MonsterJsonData.HitEffectPosType.HitPos:
                        position = particleSpawn.HitPos;
                        break;
                    case MonsterJsonData.HitEffectPosType.Self:
                        position = particleSpawn.Monster.Pos + new float3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY) / 2, 0);
                        break;
                }
                pool.Pop(path, position, rotQuaternion, null, SpawnCallback);

                //移除Tag
                ECB.DestroyEntity(entity);
            }

        }
        
        private void SpawnCallback(GameObject go) {

        }
    }
}