
using HeroFishing.Main;
using Scoz.Func;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
                var rotQuaternion = Quaternion.Euler(particleSpawn.Bullet.Direction);
                switch (monsterData.HitEffectPos) {
                    case MonsterJsonData.HitEffectPosType.HitPos:
                        GameObjSpawner.SpawnParticleObjByPath(hitPath, particleSpawn.Bullet.Position, rotQuaternion, null, null);
                        break;
                    case MonsterJsonData.HitEffectPosType.Self:
                        GameObjSpawner.SpawnParticleObjByPath(hitPath, particleSpawn.Monster.Pos + new float3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY) / 2, 0), rotQuaternion, null, null);
                        break;
                }

                //移除Tag
                ECB.DestroyEntity(entity);
            }

        }
    }
}