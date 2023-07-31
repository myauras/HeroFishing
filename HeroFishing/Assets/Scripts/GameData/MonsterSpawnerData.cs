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
        public Vector3 SpawnPos { get; private set; } = Vector3.zero;
        public Vector3 TargetPos { get; private set; } = Vector3.zero;

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
                    case "SpawnPos":
                        SpawnPos = TextManager.ParseTextToVect3(item[key].ToString(), ',');
                        break;
                    case "TargetPos":
                        TargetPos = TextManager.ParseTextToVect3(item[key].ToString(), ',');
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
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
