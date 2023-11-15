
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
                switch (monsterData.HitEffectPos) {
                    case MonsterJsonData.HitEffectPosType.HitPos:
                        GameObjSpawner.SpawnParticleObjByPath(hitPath, particleSpawn.HitPos, rotQuaternion, null, SpawnCallback);
                        break;
                    case MonsterJsonData.HitEffectPosType.Self:
                        GameObjSpawner.SpawnParticleObjByPath(hitPath, particleSpawn.Monster.Pos + new float3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY) / 2, 0), rotQuaternion, null, SpawnCallback);
                        break;
                }

                //移除Tag
                ECB.DestroyEntity(entity);
            }

        }

        private void SpawnCallback(GameObject go, AsyncOperationHandle handle) {
            AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
            DOVirtual.DelayedCall(3f, () => {
                if (go != null)
                    Object.Destroy(go);
            });
        }
    }
}