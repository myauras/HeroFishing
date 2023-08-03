using HeroFishing.Main;
using Scoz.Func;
using System.Collections.Generic;
using System.Linq;

namespace HeroFishing.Battle {
    public class ScheduledSpawn {
        public int[] MonsterIDs;
        public int RouteID;
        public bool IsBooss;
        public ScheduledSpawn(int[] monsterIDs, int routeID, bool isBoss) {
            MonsterIDs = monsterIDs;
            RouteID = routeID;
            IsBooss = isBoss;
        }
    }

    public class MonsterScheduler {
        public static bool BossExist { get; set; }//BOSS是否存在場上的標記

        bool LocoTest = true;//是否進行本地測試
        Queue<ScheduledSpawn> spawnMonsterQueue = new Queue<ScheduledSpawn>();//出怪排程
        Dictionary<int, int> spawnTimerDic;//<MonsterSpawn表ID,出怪倒數秒數>


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapData">表格-地圖資料</param>
        public void Init(MapData mapData) {
            if (LocoTest) {
                spawnTimerDic = new Dictionary<int, int>();
                foreach (var id in mapData.MonsterSpawnerIDs) {
                    var spawnData = MonsterSpawnerData.GetData(id);
                    if (spawnData == null) continue;
                    if (spawnData.Scheduled)
                        spawnTimerDic.Add(id, spawnData.GetRandSpawnSec());
                }
                UniTaskManager.StartRepeatTask("SpawnCheck", SpawnCheck, 1000);
            }
        }
        /// <summary>
        /// 此程式為本地測試用，正式版出怪是由服務端負責
        /// 檢查那些出怪表ID需要被加入出怪排程中，符合條件就加入，以下為規則：
        /// 1. BOSS還活著就不會加入BOSS類型的出怪表ID
        /// </summary>
        void SpawnCheck() {
            foreach (var id in spawnTimerDic.Keys.ToArray()) {
                var spawnData = MonsterSpawnerData.GetData(id);
                if (spawnData == null) continue;
                if (BossExist && spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss) continue;//BOSS還活著就不會加入BOSS類型的出怪表ID
                spawnTimerDic[id]--;

                if (spawnTimerDic[id] <= 0) {
                    ScheduledSpawn spawn;
                    switch (spawnData.MySpanwType) {
                        case MonsterSpawnerData.SpawnType.RandomGroup:
                            int[] ids = TextManager.StringSplitToIntArray(spawnData.TypeValue, ',');
                            if (ids == null || ids.Length == 0) continue;
                            var newSpawnID = Prob.GetRandomTFromTArray(ids);
                            var newSpawnData = MonsterSpawnerData.GetData(newSpawnID);
                            if (newSpawnData == null) continue;
                            spawn = new ScheduledSpawn(newSpawnData.MonsterIDs, newSpawnData.GetRandRoute(), newSpawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss);
                            spawnMonsterQueue.Enqueue(spawn);//加入排程
                            break;
                        case MonsterSpawnerData.SpawnType.Minion:
                        case MonsterSpawnerData.SpawnType.Boss:
                            spawn = new ScheduledSpawn(spawnData.MonsterIDs, spawnData.GetRandRoute(), spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss);
                            spawnMonsterQueue.Enqueue(spawn);//加入排程
                            break;
                    }
                    spawnTimerDic[id] = spawnData.GetRandSpawnSec();
                }
            }
        }

        /// <summary>
        /// ECS那邊出怪後會從排程中移除
        /// </summary>
        public ScheduledSpawn DequeueMonster() {
            if (!spawnMonsterQueue.Any()) return null;
            var spawn = spawnMonsterQueue.Dequeue();
            if (spawn.IsBooss) BossExist = true;
            return spawn;
        }

    }
}
