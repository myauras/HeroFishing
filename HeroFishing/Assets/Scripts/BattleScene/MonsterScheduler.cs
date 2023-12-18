using HeroFishing.Main;
using HeroFishing.Socket.Matchgame;
using Scoz.Func;
using System.Collections.Generic;
using System.Linq;

namespace HeroFishing.Battle {
    public class ScheduledSpawn {
        public int[] MonsterIDs;
        public int[] MonsterIdxs;
        public int RouteID;
        public bool IsBooss;
        public ScheduledSpawn(int[] monsterIDs, int[] monsterIdxs, int routeID, bool isBoss) {
            MonsterIDs = monsterIDs;
            MonsterIdxs = monsterIdxs;
            RouteID = routeID;
            IsBooss = isBoss;
        }
    }

    public class MonsterScheduler {
        public bool IsInit { get; private set; }
        public static bool BossExist { get; set; }//BOSS是否存在場上的標記

        Queue<ScheduledSpawn> SpawnMonsterQueue = new Queue<ScheduledSpawn>();//出怪排程
        Dictionary<int, int> SpawnTimerDic;//<MonsterSpawn表ID,出怪倒數秒數>

        MapJsonData CurMapJson;
        private int _localMonsterIndex = 0;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_mapData">表格-地圖資料</param>
        /// <param name="_locoTest">是否使用本地測試</param>
        public void Init(MapJsonData _mapData, bool _locoTest) {
            CurMapJson = _mapData;
            IsInit = true;
            if (_locoTest) {
                SpawnTimerDic = new Dictionary<int, int>();
                foreach (var id in _mapData.MonsterSpawnerIDs) {
                    var spawnData = MonsterSpawnerJsonData.GetData(id);
                    if (spawnData == null) continue;
                    if (spawnData.Scheduled)
                        SpawnTimerDic.Add(id, spawnData.GetRandSpawnSec());
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
            if (!IsInit) { WriteLog.LogError("SpawnCheck尚未初始化"); return; }
            var monsterIdx = GetLocalMonsterIdx(SpawnTimerDic.Count);
            foreach (var id in SpawnTimerDic.Keys.ToArray()) {
                var spawnData = MonsterSpawnerJsonData.GetData(id);
                if (spawnData == null) continue;
                if (BossExist && spawnData.MySpanwType == MonsterSpawnerJsonData.SpawnType.Boss) continue;//BOSS還活著就不會加入BOSS類型的出怪表ID
                SpawnTimerDic[id]--;

                if (SpawnTimerDic[id] <= 0) {
                    switch (spawnData.MySpanwType) {
                        case MonsterSpawnerJsonData.SpawnType.RandomGroup:
                            int[] ids = TextManager.StringSplitToIntArray(spawnData.TypeValue, ',');
                            if (ids == null || ids.Length == 0) continue;
                            var newSpawnID = Prob.GetRandomTFromTArray(ids);
                            var newSpawnData = MonsterSpawnerJsonData.GetData(newSpawnID);
                            if (newSpawnData == null) continue;
                            EnqueueMonster(newSpawnData.MonsterIDs, monsterIdx, newSpawnData.GetRandRoute(), newSpawnData.MySpanwType == MonsterSpawnerJsonData.SpawnType.Boss);
                            break;
                        case MonsterSpawnerJsonData.SpawnType.Minion:
                        case MonsterSpawnerJsonData.SpawnType.Boss:
                            EnqueueMonster(spawnData.MonsterIDs, monsterIdx, spawnData.GetRandRoute(), spawnData.MySpanwType == MonsterSpawnerJsonData.SpawnType.Boss);
                            break;
                    }
                    SpawnTimerDic[id] = spawnData.GetRandSpawnSec();
                }
            }
        }

        /// <summary>
        /// 加入到要生怪物的排程中
        /// </summary>
        public void EnqueueMonster(int[] _monsterIDs, int[] _monsterIdxs, int _routeID, bool _isBoss) {
            if (!IsInit) { WriteLog.LogError("SpawnCheck尚未初始化"); return; }
            var spawn = new ScheduledSpawn(_monsterIDs, _monsterIdxs, _routeID, _isBoss);
            //WriteLog.Log(DebugUtils.ObjToStr(spawn));
            SpawnMonsterQueue.Enqueue(spawn);//加入排程
        }

        /// <summary>
        /// ECS那邊出怪後會從排程中移除
        /// </summary>
        public ScheduledSpawn DequeueMonster() {
            //if (!IsInit) { WriteLog.LogError("SpawnCheck尚未初始化"); return null; }
            if (!SpawnMonsterQueue.Any()) return null;
            var spawn = SpawnMonsterQueue.Dequeue();
            if (spawn.IsBooss) BossExist = true;
            return spawn;
        }

        /// <summary>
        /// 需要local測試的時候，取得所需的int陣列使用
        /// </summary>
        /// <param name="count">需要多少index</param>
        /// <returns>index陣列</returns>
        private int[] GetLocalMonsterIdx(int count) {
            int[] values = new int[count];
            for (int i = 0; i < count; i++) {
                values[i] = _localMonsterIndex;
                _localMonsterIndex++;
            }
            return values;
        }
    }
}
