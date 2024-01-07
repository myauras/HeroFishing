using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace HeroFishing.Main {
    public class HeroSkinJsonData : MyJsonData {
        public static string DataName { get; set; }
        public new string ID { get; private set; }
        public string Name {
            get {
                return StringJsonData.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public string Prefab { get; private set; }
        public string Texture { get; private set; }
        static Dictionary<int, SortedDictionary<string, HeroSkinJsonData>> SkinDic = new Dictionary<int, SortedDictionary<string, HeroSkinJsonData>>();

        protected override void GetDataFromJson(JsonData _item, string _dataName) {
            DataName = _dataName;
            JsonData item = _item;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = item[key].ToString();
                        break;
                    case "Prefab":
                        Prefab = item[key].ToString();
                        break;
                    case "Texture":
                        Texture = item[key].ToString();
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
            AddToSkinDic(ID, this);
        }

        static void AddToSkinDic(string _id, HeroSkinJsonData _data) {
            string[] strs = _id.Split('_');
            if (strs.Length != 2) return;
            if (string.IsNullOrEmpty(strs[1])) return;
            if (int.TryParse(strs[0], out int _heroID)) {
                if (SkinDic.ContainsKey(_heroID)) {
                    if (SkinDic[_heroID].ContainsKey(strs[1]))
                        WriteLog.LogErrorFormat("重複的SkinID: {0}", strs[1]);
                    SkinDic[_heroID][strs[1]] = _data;
                } else {
                    SkinDic.Add(_heroID, new SortedDictionary<string, HeroSkinJsonData>() { { strs[1], _data } });
                }
            }
        }

        public static HeroSkinJsonData GetData(string _id) {
            return GameDictionary.GetJsonData<HeroSkinJsonData>(DataName, _id);
        }
        public static SortedDictionary<string, HeroSkinJsonData> GetSkinDic(int _heroID) {
            if (!SkinDic.ContainsKey(_heroID)) return null;
            return SkinDic[_heroID];
        }
        public static HeroSkinJsonData GetSkin(int _heroID, string _skinSubID) {
            if (!SkinDic.ContainsKey(_heroID)) return null;
            if (!SkinDic[_heroID].ContainsKey(_skinSubID)) return null;
            return SkinDic[_heroID][_skinSubID];
        }

        protected override void ResetStaticData() {
            SkinDic.Clear();
        }
    }

}
