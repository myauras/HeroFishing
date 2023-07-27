using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;
using JetBrains.Annotations;

namespace HeroFishing.Main {
    public class MonsterSpawnerData : MyJsonData {
        public static string DataName { get; set; }
        public int[] MonsterIDs { get; private set; }
        public enum SpawnType {
            All,
            Boss,
            Random,
        }
        public SpawnType MySpanwType { get; private set; }
        public int[] MonsterSpawnIntervalSecs = new int[2] { 0, 0 };
        protected override void GetDataFromJson(JsonData item, string dataName) {
            DataName = dataName;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = int.Parse(item[key].ToString());
                        break;
                    case "MonsterIDs":
                        MonsterIDs = TextManager.StringSplitToIntArray(item[key].ToString(), ',');
                        break;
                    case "SpawnType":
                        MySpanwType = MyEnum.ParseEnum<SpawnType>(item[key].ToString());
                        break;
                    case "MonsterSpawnIntervalSec":
                        TextManager.SetSplitToIntArray(item[key].ToString(), '~', ref MonsterSpawnIntervalSecs);
                        break;
                    default:
                        //WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static MonsterSpawnerData GetData(int id) {
            return GameDictionary.GetJsonData<MonsterSpawnerData>(DataName, id);
        }
        public int GetRandSpawnSec() {
            return UnityEngine.Random.Range(MonsterSpawnIntervalSecs[0], MonsterSpawnIntervalSecs[1] + 1);
        }
    }

}
