using UnityEngine;
using System.Collections.Generic;
using HeroFishing.Main;

namespace Scoz.Func {

    public partial class GameDictionary : MonoBehaviour {
        static Dictionary<int, GameObject> MonsterPrefabs = new Dictionary<int, GameObject>();

        public static void PreLoadMonsterPrefabs() {
            WriteLog.Log("預載MonsterPrefabs");
            var monsterDatas = GetIntKeyJsonDic<MonsterData>("Monster");
            foreach (var data in monsterDatas.Values) {
                var tmData = data;
                string path = string.Format("Monster/{0}", tmData.Ref);
                AddressablesLoader.GetPrefab(path, (go, handle) => {
                    MonsterPrefabs.Add(tmData.ID, go);
                    WriteLog.LogFormat("載入MonsterPrefab {0} 完成", tmData.Ref);
                });
            }
        }
        /// <summary>
        /// 傳入怪物表ID來取得怪物Prefab
        /// </summary>
        /// <param name="id">怪物表ID</param>
        /// <returns>回傳怪物Prefab</returns>
        public static GameObject GetMonsterPrefab(int id) {
            if (!MonsterPrefabs.ContainsKey(id)) return null;
            return MonsterPrefabs[id];
        }


    }
}