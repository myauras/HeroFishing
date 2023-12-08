using HeroFishing.Main;
using Scoz.Func;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HeroFishing.Battle {

    public partial struct MonsterSpawnSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterSpawnSys>();
        }

        public void OnUpdate(ref SystemState state) {
            if (BattleManager.Instance == null || BattleManager.Instance.MyMonsterScheduler == null) return;
            var spawn = BattleManager.Instance.MyMonsterScheduler.DequeueMonster();
            if (spawn == null) return;
            if (spawn.MonsterIDs == null || spawn.MonsterIDs.Length == 0) return;
            bool isFreeze = SystemAPI.HasSingleton<FreezeTag>();
            if (isFreeze) return;

            foreach (var monsterID in spawn.MonsterIDs) {
                GameObject monsterPrefab = ResourcePreSetter.Instance.MonsterPrefab.gameObject;
                if (monsterPrefab == null) continue;
                var monsterData = MonsterJsonData.GetData(monsterID);
                if (monsterData == null) continue;
                var monsterGO = GameObject.Instantiate(monsterPrefab);
                monsterGO.transform.SetParent(BattleManager.Instance.MonsterParent);
                var routeData = spawn.RouteID != 0 ? RouteJsonData.GetData(spawn.RouteID) : null;
#if UNITY_EDITOR
                monsterGO.name = monsterData.Ref;
                //monsterGO.hideFlags |= HideFlags.HideAndDontSave;
#else
                monsterGO.hideFlags |= HideFlags.HideAndDontSave;
#endif
                var entity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentObject(entity, monsterGO.GetComponent<Transform>());
                var monster = monsterGO.GetComponent<Monster>();

                //設定怪物位置與方向
                Quaternion dirQuaternion = Quaternion.Euler(0, 180, 0);//面向方向四元數
                Vector3 dir = Vector3.zero;//面向方向向量
                if (routeData != null) {
                    monsterGO.transform.localPosition = routeData.SpawnPos;
                    dir = (routeData.TargetPos - routeData.SpawnPos).normalized;
                    dirQuaternion = Quaternion.LookRotation(dir);
                    state.EntityManager.AddComponentData(entity, new MonsterValue {
                        MonsterID = monsterData.ID,
                        MyEntity = entity,
                        Radius = monsterData.Radius,
                        Pos = routeData.SpawnPos,
                        InField = false,
                    });
                } else {
                    monsterGO.transform.localPosition = Vector3.zero;
                    state.EntityManager.AddComponentData(entity, new MonsterValue {
                        MyEntity = entity,
                        Radius = monsterData.Radius,
                        Pos = float3.zero,
                        InField = false,
                    });
                }
                //設定怪物資料，並在完成載入模型後設定動畫與方向
                monster.SetData(monsterID, () => {
                    monster.FaceDir(dirQuaternion);
                    monster.SetAniTrigger("run");
                });
                state.EntityManager.AddComponentData(entity, new MonsterInstance {
                    GO = monsterGO,
                    Trans = monsterGO.transform,
                    MyMonster = monster,
                    Dir = dir,
                });
            }

            //是BOSS就會攝影機震動
            if (spawn.IsBooss)
                CamManager.ShakeCam(CamManager.CamNames.Battle,
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_AmplitudeGain),
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_FrequencyGain),
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_Duration));

        }
    }
}
