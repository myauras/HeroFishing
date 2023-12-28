using HeroFishing.Main;
using Scoz.Func;
using UniRx.Triggers;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HeroFishing.Battle {

    public partial struct MonsterSpawnSystem : ISystem {

        private EndSimulationEntityCommandBufferSystem.Singleton _ecbSingleton;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterSpawnSys>();
            state.RequireForUpdate<SpawnData>();
        }

        public void OnUpdate(ref SystemState state) {
            _ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecbCommandBuffer = _ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            bool isFreeze = SystemAPI.HasSingleton<FreezeTag>();
            foreach (var (spawn, entity) in SystemAPI.Query<SpawnData>().WithEntityAccess()) {
                if (isFreeze) {
                    ecbCommandBuffer.DestroyEntity(entity);
                    continue;
                }

                for (int i = 0; i < spawn.Monsters.Length; i++) {
                    if (!CreateMonster(spawn.Monsters[i], out var monsterData, out var monster)) continue;
                    int monsterID = spawn.Monsters[i].ID;
                    int monsterIdx = spawn.Monsters[i].Idx;

                    var routeData = spawn.RouteID != 0 ? RouteJsonData.GetData(spawn.RouteID) : null;

                    var monsterEntity = state.EntityManager.CreateEntity();
                    //state.EntityManager.AddComponentObject(monsterEntity, monster.transform);

                    //設定怪物位置與方向
                    Quaternion dirQuaternion = Quaternion.Euler(0, 180, 0);//面向方向四元數
                    Vector3 dir = Vector3.zero;//面向方向向量
                    if (routeData != null) {
                        dir = (routeData.TargetPos - routeData.SpawnPos).normalized;
                        if (spawn.SpawnTime == 0) {
                            monster.transform.localPosition = routeData.SpawnPos;
                        }
                        else {
                            var deltaTime = GameTime.Current - spawn.SpawnTime;
                            monster.transform.localPosition = routeData.SpawnPos + dir * deltaTime * monsterData.Speed;
                        }
                        dirQuaternion = Quaternion.LookRotation(dir);
                        ecbCommandBuffer.AddComponent(monsterEntity, new MonsterValue {
                            MonsterID = monsterID,
                            MonsterIdx = monsterIdx,
                            MyEntity = monsterEntity,
                            Radius = monsterData.Radius,
                            Pos = routeData.SpawnPos,
                            InField = false,
                        });
                    }
                    else {
                        monster.transform.localPosition = Vector3.zero;
                        ecbCommandBuffer.AddComponent(monsterEntity, new MonsterValue {
                            MyEntity = monsterEntity,
                            Radius = monsterData.Radius,
                            Pos = float3.zero,
                            InField = false,
                        });
                    }
                    //設定怪物資料，並在完成載入模型後設定動畫與方向
                    monster.SetData(monsterID, monsterIdx, () => {
                        monster.FaceDir(dirQuaternion);
                        monster.SetAniTrigger("run");
                    });
                    ecbCommandBuffer.AddComponent(monsterEntity, new MonsterInstance {
                        GO = monster.gameObject,
                        Trans = monster.transform,
                        MyMonster = monster,
                        Dir = dir,
                    });
                }

                ecbCommandBuffer.DestroyEntity(entity);
            }
        }

        private bool CreateMonster(MonsterData spawnData, out MonsterJsonData monsterData, out Monster monster) {
            monster = null;
            monsterData = null;
            int monsterID = spawnData.ID;
            if (monsterID < 0) return false;
            int monsterIdx = spawnData.Idx;
            GameObject monsterPrefab = ResourcePreSetter.Instance.MonsterPrefab.gameObject;
            if (monsterPrefab == null) return false;
            monsterData = MonsterJsonData.GetData(monsterID);
            if (monsterData == null) return false;
            var monsterGO = Object.Instantiate(monsterPrefab);
            monster = monsterGO.GetComponent<Monster>();
            monster.transform.SetParent(BattleManager.Instance.MonsterParent);
#if UNITY_EDITOR
            monsterGO.name = monsterData.Ref;
            //monsterGO.hideFlags |= HideFlags.HideAndDontSave;
#else
                monsterGO.hideFlags |= HideFlags.HideAndDontSave;
#endif
            return true;
        }
    }
}
