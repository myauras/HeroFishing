using HeroFishing.Main;
using Scoz.Func;
using System.Collections.Generic;
using System.Linq;

namespace HeroFishing.Battle {
    public class MonsterScheduler {

        Queue<int> readyToSpawnIDQueue = new Queue<int>();
        Dictionary<int, int> spawnTimerDic = new Dictionary<int, int>();

        public bool BossExist { get; private set; }


        public void Init(MapData mapData) {
            foreach (var id in mapData.MonsterSpawnerIDs) {
                var spawnData = MonsterSpawnerData.GetData(id);
                spawnTimerDic.Add(id, spawnData.GetRandSpawnSec());
            }
            UniTaskManager.StartRepeatTask("test", SpawnCheck, 1000);
        }

        void SpawnCheck() {
            foreach (var id in spawnTimerDic.Keys.ToArray()) {
                spawnTimerDic[id]--;
                if (spawnTimerDic[id] <= 0) {
                    var spawnData = MonsterSpawnerData.GetData(id);
                    if (spawnData == null) continue;
                    if (BossExist && spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss) continue;//BOSS已存在就不產生
                    if (readyToSpawnIDQueue.Contains(id)) continue;

                    spawnTimerDic[id] = spawnData.GetRandSpawnSec();
                    readyToSpawnIDQueue.Enqueue(id);
                }
            }
        }

        /// <summary>
        /// 取出排程中的MonsterSpawnID
        /// </summary>
        public MonsterSpawnerData DequeueMonster() {
            if (!readyToSpawnIDQueue.Any()) return null;
            var spawnData = MonsterSpawnerData.GetData(readyToSpawnIDQueue.Dequeue());
            if (spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss) BossExist = true;
            return spawnData;
        }

    }
}
