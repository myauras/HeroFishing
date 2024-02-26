using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpawnMonsterInfo {
    public struct MonsterInfo {
        public int ID;
        public int Idx;
    }

    public int RouteID;
    public float SpawnTime;
    public bool IsBoss;
    public int PlayerIndex;
    public IList<MonsterInfo> Monsters;
}

public static class MonsterSpawner {
    public static bool Spawn(SpawnMonsterInfo spawn, out Monster monster) {
        monster = null;
        if (WorldStateManager.Instance.IsFrozen) return false;
        bool hasSpawn = false;
        for (int i = 0; i < spawn.Monsters.Count; i++) {
            var spawnMonster = spawn.Monsters[i];
            if (Monster.IdxToMonsterMapping.ContainsKey(spawnMonster.Idx)) continue;
            int monsterID = spawnMonster.ID;
            if (monsterID <= 0) continue;

            if (!TryGetMonsterData(monsterID, out var monsterData)) continue;
            var routeData = spawn.RouteID != 0 ? RouteJsonData.GetData(spawn.RouteID) : null;
            Quaternion initRotation = Quaternion.Euler(0, 180, 0);
            Vector3 initPosition = Vector3.zero;
            if (routeData != null) {
                var rotation = Quaternion.AngleAxis(spawn.PlayerIndex * 90f, Vector3.up);
                initRotation = Quaternion.LookRotation(routeData.TargetPos - routeData.SpawnPos);
                if (spawn.SpawnTime == 0)
                    initPosition = rotation * routeData.SpawnPos;
                else {
                    var deltaTime = GameTime.Current - spawn.SpawnTime;
                    var deltaPosition = deltaTime * monsterData.Speed * (initRotation * Vector3.forward);
                    if (Vector3.SqrMagnitude(deltaPosition) > Vector3.SqrMagnitude(routeData.TargetPos - routeData.SpawnPos))
                        continue;
                    initPosition = rotation * (routeData.SpawnPos + deltaPosition);
                }
            }

            if (!TryCreateMonster(monsterID, out monster)) continue;
            int monsterIdx = spawnMonster.Idx;
            monster.transform.localPosition = initPosition;
            Monster tmpMonster = monster;
            monster.SetData(monsterID, monsterIdx, () => {
                tmpMonster.FaceDir(initRotation);
                tmpMonster.SetAniTrigger("run");
            });

            if (!monster.gameObject.TryGetComponent<MonsterGrid>(out var grid)) {
                grid = monster.gameObject.AddComponent<MonsterGrid>();
            }
            grid.Init();
        }
        return hasSpawn;
    }

    /// <summary>
    /// 取得怪物資料
    /// </summary>
    /// <param name="monsterID">輸入Monster ID</param>
    /// <param name="monsterData">取得目標資料</param>
    /// <returns>取得怪物資料成功與否</returns>
    private static bool TryGetMonsterData(int monsterID, out MonsterJsonData monsterData) {
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
    private static bool TryCreateMonster(int monsterID, out Monster monster) {
        monster = null;
        GameObject monsterPrefab = ResourcePreSetter.Instance.MonsterPrefab.gameObject;
        if (monsterPrefab == null) return false;
        var monsterGO = Object.Instantiate(monsterPrefab);
        monster = monsterGO.GetComponent<Monster>();
        monster.transform.SetParent(BattleManager.Instance.MonsterParent);

        return true;
    }
}
