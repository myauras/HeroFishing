using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;

namespace HeroFishing.Main {
    public class MapJsonData : MyJsonData {
        public static string DataName { get; set; }
        public string Name {
            get {
                return StringJsonData.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public string Ref { get; private set; }
        public string MapName { get; private set; }
        public float Multiplier { get; private set; }
        public int[] MonsterSpawnerIDs { get; private set; }
        public Vector2 ForegroundPos { get; private set; }
        protected override void GetDataFromJson(JsonData _item, string _dataName) {
            DataName = _dataName;
            JsonData item = _item;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = int.Parse(item[key].ToString());
                        break;
                    case "Name":
                        MapName = item[key].ToString();
                        break;
                    case "Ref":
                        Ref = item[key].ToString();
                        break;
                    case "Multiplier":
                        Multiplier = float.Parse(item[key].ToString());
                        break;
                    case "MonsterSpawnerIDs":
                        MonsterSpawnerIDs = TextManager.StringSplitToIntArray(item[key].ToString(), ',');
                        break;
                    case "ForegroundPos":
                        ForegroundPos = TextManager.ParseTextToVector2(item[key].ToString(), ',');
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static MapJsonData GetData(int id) {
            return GameDictionary.GetJsonData<MapJsonData>(DataName, id);
        }

        protected override void ResetStaticData() {
        }
    }

}
