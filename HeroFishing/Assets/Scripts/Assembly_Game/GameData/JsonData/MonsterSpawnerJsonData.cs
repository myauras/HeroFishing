using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;
using JetBrains.Annotations;

namespace HeroFishing.Main {
    public class MonsterSpawnerJsonData : MyJsonData {
        public static string DataName { get; set; }
        public enum SpawnType {
            RandomGroup,
            RandomItem,
            Minion,
            Boss,
        }
        public SpawnType MySpanwType { get; private set; }
        public string TypeValue { get; private set; }
        public int[] MonsterIDs { get; private set; }
        public int[] MonsterSpawnIntervalSecs = new int[2] { 0, 0 };
        public int[] Routes { get; private set; }

        /// <summary>
        /// 測試用本地出怪時用，回傳是否需要計時執行出怪
        /// </summary>
        public bool Scheduled { get { return !(MonsterSpawnIntervalSecs[0] == 0 && MonsterSpawnIntervalSecs[1] == 0); } }
        protected override void GetDataFromJson(JsonData item, string dataName) {
            DataName = dataName;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = int.Parse(item[key].ToString());
                        break;
                    case "SpawnType":
                        MySpanwType = MyEnum.ParseEnum<SpawnType>(item[key].ToString());
                        break;
                    case "TypeValue":
                        TypeValue = item[key].ToString();
                        break;
                    case "MonsterIDs":
                        MonsterIDs = TextManager.StringSplitToIntArray(item[key].ToString(), ',');
                        break;
                    case "MonsterSpawnIntervalSec":
                        TextManager.StrSplitToIntArray(item[key].ToString(), '~', ref MonsterSpawnIntervalSecs);
                        break;
                    case "Routes":
                        Routes = TextManager.StringSplitToIntArray(item[key].ToString(), ',');
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static MonsterSpawnerJsonData GetData(int id) {
            return GameDictionary.GetJsonData<MonsterSpawnerJsonData>(DataName, id);
        }
        public int GetRandSpawnSec() {
            return UnityEngine.Random.Range(MonsterSpawnIntervalSecs[0], MonsterSpawnIntervalSecs[1] + 1);
        }
        /// <summary>
        /// 本地測試時，取得隨機路徑用
        /// </summary>
        public int GetRandRoute() {
            return Prob.GetRandomTFromTArray(Routes);
        }

        protected override void ResetStaticData() {
        }
    }

}
