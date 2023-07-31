using Scoz.Func;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace HeroFishing.Battle {
    public struct MonsterValue : IComponentData {
        public float Radius;
    }
    public class MonsterGOInstance : IComponentData, IDisposable {
        public GameObject Instance;
        public void Dispose() {
            UnityEngine.Object.DestroyImmediate(Instance);
        }
    }
    public partial struct MonsterSpawnSystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterSpawnSys>();
        }

        public void OnUpdate(ref SystemState state) {
            if (BattleManager.Instance == null || BattleManager.Instance.MyMonsterScheduler == null) return;
            var spawnData = BattleManager.Instance.MyMonsterScheduler.DequeueMonster();
            if (spawnData == null) return;
            if (spawnData.MonsterIDs == null || spawnData.MonsterIDs.Length == 0) return;

            foreach (var monsterID in spawnData.MonsterIDs) {
                GameObject monsterPrefab = GameDictionary.GetMonsterPrefab(monsterID);
                if (monsterPrefab == null) continue;
                var monsterGO = GameObject.Instantiate(monsterPrefab);
                monsterGO.hideFlags |= HideFlags.HideAndDontSave;
                var entity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentObject(entity, monsterGO.GetComponent<Transform>());
                state.EntityManager.AddComponentData(entity, new MonsterGOInstance { Instance = monsterGO });
            }
            //BOSS出場攝影機震動
            if (spawnData.MySpanwType == Main.MonsterSpawnerData.SpawnType.Boss) CamManager.ShakeCam(CamManager.CamNames.Battle, 3, 3, 2f);



            //var query = SystemAPI.QueryBuilder().WithAll<MonsterValue>().Build();
            //var entities = query.ToEntityArray(Allocator.Temp);

            //foreach (var entity in entities) {
            //    var instance = GameObject.Instantiate(GameDictionary.GetMonsterPrefab(spawnData.ID));
            //    instance.hideFlags |= HideFlags.HideAndDontSave;
            //    state.EntityManager.AddComponentObject(entity, instance.GetComponent<Transform>());
            //    state.EntityManager.AddComponentData(entity, new MonsterGOInstance { Instance = instance });
            //    CamManager.ShakeCam(CamManager.CamNames.Battle, 3, 3, 2f);
            //    //state.EntityManager.RemoveComponent<MonsterGOPrefab>(entity);
            //}

        }
    }
}
