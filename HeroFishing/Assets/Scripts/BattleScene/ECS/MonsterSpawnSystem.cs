using HeroFishing.Main;
using HeroFishing.Socket.Matchgame;
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
            state.RequireForUpdate<SpawnTag>();
        }

        public void OnUpdate(ref SystemState state) {
            _ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecbCommandBuffer = _ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            bool isFreeze = SystemAPI.HasSingleton<FreezeTag>();
            foreach (var (spawn, entity) in SystemAPI.Query<SpawnData>().WithAll<SpawnTag>().WithEntityAccess()) {
                if (isFreeze) {
                    ecbCommandBuffer.DestroyEntity(entity);
                    continue;
                }

                for (int i = 0; i < spawn.Monsters.Length; i++) {
                    int monsterID = spawn.Monsters[i].ID;
                    if (monsterID <= 0) continue;
                    if (!TryGetMonsterData(monsterID, out var monsterData)) continue;

                    var routeData = spawn.RouteID != 0 ? RouteJsonData.GetData(spawn.RouteID) : null;
                    //設定怪物位置與方向
                    Quaternion dirQuaternion = Quaternion.Euler(0, 180, 0);//面向方向四元數
                    Vector3 dir = Vector3.zero;//面向方向向量
                    Vector3 pos = Vector3.zero;
                    if (routeData != null) {
                        var rotation = Quaternion.AngleAxis(spawn.PlayerIndex * 90f, Vector3.up);
                        dir = rotation * (routeData.TargetPos - routeData.SpawnPos).normalized;
                        dirQuaternion = rotation * Quaternion.LookRotation(dir);
                        if (spawn.SpawnTime == 0)
                            pos = rotation * routeData.SpawnPos;
                        else {
                            var deltaTime = GameTime.Current - spawn.SpawnTime;
                            var deltaPosition = deltaTime * monsterData.Speed * dir;
                            if (Vector3.SqrMagnitude(deltaPosition) > Vector3.SqrMagnitude(routeData.TargetPos - routeData.SpawnPos)) {
                                continue;
                            }
                            pos = rotation * routeData.SpawnPos + deltaPosition;
                        }
                    }

                    if (!TryCreateMonster(monsterID, out var monster)) continue;
                    int monsterIdx = spawn.Monsters[i].Idx;

#if UNITY_EDITOR
                    monster.name = monsterData.Ref;
                    //monster.hideFlags |= HideFlags.HideAndDontSave;
#else
                monster.hideFlags |= HideFlags.HideAndDontSave;
#endif

                    var monsterEntity = state.EntityManager.CreateEntity();
                    //state.EntityManager.AddComponentObject(monsterEntity, monster.transform);

                    monster.transform.localPosition = pos;
                    ecbCommandBuffer.AddComponent(monsterEntity, new MonsterValue {
                        MonsterID = monsterID,
                        MonsterIdx = monsterIdx,
                        MyEntity = monsterEntity,
                        Radius = monsterData.Radius,
                        Pos = pos,
                        InField = false,
                    });
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

        /// <summary>
        /// 取得怪物資料
        /// </summary>
        /// <param name="monsterID">輸入Monster ID</param>
        /// <param name="monsterData">取得目標資料</param>
        /// <returns>取得怪物資料成功與否</returns>
        private bool TryGetMonsterData(int monsterID, out MonsterJsonData monsterData) {
            monsterData = null;
            monsterData = MonsterJsonData.GetData(monsterID);
            if (monsterData == null) return false;
            return true;
        }

        /// <summary>
        /// 創建怪物實體
        /// </summary>
        /// <param name="monsterID">輸入怪物ID</param>
        /// <param name="monster">創建怪物實體</param>
        /// <returns>創建怪物成功與否</returns>
        private bool TryCreateMonster(int monsterID, out Monster monster) {
            monster = null;
            GameObject monsterPrefab = ResourcePreSetter.Instance.MonsterPrefab.gameObject;
            if (monsterPrefab == null) return false;
            var monsterGO = Object.Instantiate(monsterPrefab);
            monster = monsterGO.GetComponent<Monster>();
            monster.transform.SetParent(BattleManager.Instance.MonsterParent);

            return true;
        }
    }
}
