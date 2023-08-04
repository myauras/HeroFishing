using HeroFishing.Main;
using Scoz.Func;
using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HeroFishing.Battle {
    /// <summary>
    /// 資料元件
    /// </summary>
    public struct MonsterValue : IComponentData {
        public Entity MyEntity;
        public float Radius;
        public float3 Pos;
    }
    /// <summary>
    /// 擊中標籤元件
    /// </summary>
    public struct MonsterHitTag : IComponentData { }
    /// <summary>
    /// 死亡標籤元件
    /// </summary>
    public struct MonsterDieTag : IComponentData { }
    /// <summary>
    /// 怪物參照元件，用於參照GameObject實例用
    /// </summary>
    public class MonsterInstance : IComponentData, IDisposable {
        public GameObject GO;
        public Transform Trans;
        public Monster MyMonster;
        public Vector3 Dir;
        public void Dispose() {
            UnityEngine.Object.DestroyImmediate(GO);
        }
    }
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

            foreach (var monsterID in spawn.MonsterIDs) {
                GameObject monsterPrefab = GameDictionary.GetMonsterPrefab(monsterID);
                if (monsterPrefab == null) continue;
                var monsterData = MonsterData.GetData(monsterID);
                if (monsterData == null) continue;
                var monsterGO = GameObject.Instantiate(monsterPrefab);
                var routeData = spawn.RouteID != 0 ? RouteData.GetData(spawn.RouteID) : null;
                monsterGO.hideFlags |= HideFlags.HideAndDontSave;
                var entity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentObject(entity, monsterGO.GetComponent<Transform>());
                var monster = monsterGO.GetComponent<Monster>();
                monster.SetData(monsterData);
                //設定怪物位置與方向
                Vector3 dir = Vector3.zero;
                if (routeData != null) {
                    monsterGO.transform.localPosition = routeData.SpawnPos;
                    dir = (routeData.TargetPos - routeData.SpawnPos).normalized;
                    state.EntityManager.AddComponentData(entity, new MonsterValue {
                        MyEntity = entity,
                        Radius = monsterData.Radius,
                        Pos = routeData.SpawnPos,
                    });
                } else {
                    monsterGO.transform.localPosition = Vector3.zero;
                    state.EntityManager.AddComponentData(entity, new MonsterValue {
                        MyEntity = entity,
                        Radius = monsterData.Radius,
                        Pos = float3.zero,
                    });
                }
                monster.FaceDir(Quaternion.LookRotation(dir));
                monster.SetAniTrigger("run");
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
                    GameSettingData.GetFloat(GameSetting.CamShake_BossDebut_AmplitudeGain),
                    GameSettingData.GetFloat(GameSetting.CamShake_BossDebut_FrequencyGain),
                    GameSettingData.GetFloat(GameSetting.CamShake_BossDebut_Duration));

        }
    }
}
