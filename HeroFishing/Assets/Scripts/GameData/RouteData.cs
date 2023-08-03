using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;
using JetBrains.Annotations;

namespace HeroFishing.Main {
    public class RouteData : MyJsonData {
        public static string DataName { get; set; }
        public Vector3 SpawnPos { get; private set; } = Vector3.zero;
        public Vector3 TargetPos { get; private set; } = Vector3.zero;

        protected override void GetDataFromJson(JsonData item, string dataName) {
            DataName = dataName;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = int.Parse(item[key].ToString());
                        break;
                    case "SpawnPos":
                        SpawnPos = TextManager.ParseTextToVector3(item[key].ToString(), ',');
                        break;
                    case "TargetPos":
                        TargetPos = TextManager.ParseTextToVector3(item[key].ToString(), ',');
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static RouteData GetData(int id) {
            return GameDictionary.GetJsonData<RouteData>(DataName, id);
        }
    }

}
