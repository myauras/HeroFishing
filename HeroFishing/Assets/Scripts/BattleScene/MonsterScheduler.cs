using HeroFishing.Main;
using Scoz.Func;
using System.Collections.Generic;
using System.Linq;

namespace HeroFishing.Battle {

    public class MonsterScheduler {


        public bool BossExist { get; private set; }//BOSS是否存在場上的標記



        bool LocoTest = true;//是否進行本地測試
        Queue<int> spawnMonsterQueue = new Queue<int>();//<怪物表ID>出怪排程
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
                spawnTimerDic[id]--;
                if (spawnTimerDic[id] <= 0) {
                    var spawnData = MonsterSpawnerData.GetData(id);
                    if (spawnData == null) continue;
                    if (BossExist && spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss) continue;//BOSS還活著就不會加入BOSS類型的出怪表ID

                    switch (spawnData.MySpanwType) {
                        case MonsterSpawnerData.SpawnType.RandomGroup:
                            int[] ids = TextManager.StringSplitToIntArray(spawnData.TypeValue, ',');
                            if (ids == null || ids.Length == 0) continue;
                            int rndID = Prob.GetRandomTFromTArray(ids);
                            spawnMonsterQueue.Enqueue(rndID);
                            break;
                        case MonsterSpawnerData.SpawnType.Minion:
                        case MonsterSpawnerData.SpawnType.Boss:
                            spawnMonsterQueue.Enqueue(id);
                            break;
                    }
                    spawnTimerDic[id] = spawnData.GetRandSpawnSec();
                }
            }
        }

        /// <summary>
        /// ECS那邊出怪後會從排程中移除
        /// </summary>
        public MonsterSpawnerData DequeueMonster() {
            if (!spawnMonsterQueue.Any()) return null;
            var spawnData = MonsterSpawnerData.GetData(spawnMonsterQueue.Dequeue());
            if (spawnData.MySpanwType == MonsterSpawnerData.SpawnType.Boss) BossExist = true;
            return spawnData;
        }

    }
}
