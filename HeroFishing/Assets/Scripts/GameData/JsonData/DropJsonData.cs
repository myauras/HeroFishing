using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;

namespace HeroFishing.Main {
    public class DropJsonData : MyJsonData {
        public static string DataName { get; set; }
        public string Name {
            get {
                return StringJsonData.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public float RTP { get; private set; }
        public string Ref { get; private set; }
        public DropType MyDropType { get; private set; }
        public string DropValue { get; private set; }

        public enum DropType {
            Spell,
        }

        protected override void GetDataFromJson(JsonData _item, string _dataName) {
            DataName = _dataName;
            JsonData item = _item;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = int.Parse(item[key].ToString());
                        break;
                    case "Ref":
                        Ref = item[key].ToString();
                        break;
                    case "RTP":
                        RTP = float.Parse(item[key].ToString());
                        break;
                    case "DropType":
                        if (MyEnum.TryParseEnum(item[key].ToString(), out DropType _type)) {
                            MyDropType = _type;
                        }
                        break;
                    case "DropValue":
                        DropValue = item[key].ToString();
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static DropJsonData GetData(int id) {
            return GameDictionary.GetJsonData<DropJsonData>(DataName, id);
        }

        protected override void ResetStaticData() {
        }
    }

}
