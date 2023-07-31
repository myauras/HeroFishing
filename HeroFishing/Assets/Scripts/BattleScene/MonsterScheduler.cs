using HeroFishing.Main;
using Scoz.Func;
using System.Collections.Generic;
using System.Linq;

namespace HeroFishing.Battle {
    public class MonsterScheduler {

        public bool BossExist { get; private set; }//BOSS是否存在場上的標記

        Queue<int> readyToSpawnIDQueue = new Queue<int>();//出怪排程
        Dictionary<int, int> spawnTimerDic = new Dictionary<int, int>();//<群組ID,出怪倒數秒數>



        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapData">表格-地圖資料</param>
        public void Init(MapData mapData) {
            foreach (var id in mapData.MonsterSpawnerIDs) {
                var spawnData = MonsterSpawnerData.GetData(id);
                spawnTimerDic.Add(id, spawnData.GetRandSpawnSec());
            }
            UniTaskManager.StartRepeatTask("SpawnCheck", SpawnCheck, 1000);
        }

        /// <summary>
        /// 檢查那些出怪群組需要被加入出怪排程中，符合條件就加入，以下為規則：
        /// 1. 同個出怪群組ID不會重複加
        /// 2. BOSS還活著就不會出BOSS群組，即使群組ID不一樣
        /// </summary>
        void SpawnCheck() {
            foreach (var id in spawnTimerDic.Keys.ToArray()) {
                spawnTimerDic[id]--;
                if (spawnTimerDic[id] <= 0) {
                    var spawnData = MonsterSpawnerData.GetData(id);
                    if (spawnData == null) continue;
                    if (BossExist && spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss) continue;//BOSS已存在就不產生
                    if (readyToSpawnIDQueue.Contains(id)) continue;//同個群組

                    spawnTimerDic[id] = spawnData.GetRandSpawnSec();
                    readyToSpawnIDQueue.Enqueue(id);
                }
            }
        }

        /// <summary>
        /// ECS那邊出怪後會從排程中移除
        /// </summary>
        public MonsterSpawnerData DequeueMonster() {
            if (!readyToSpawnIDQueue.Any()) return null;
            var spawnData = MonsterSpawnerData.GetData(readyToSpawnIDQueue.Dequeue());
            if (spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss) BossExist = true;
            return spawnData;
        }

    }
}
