using LitJson;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroFishing.Main {
    public class DropSpellJsonData : MyJsonData {
        public enum EffectType { Frozen, Speedup, Circle, HeroSpell }
        public static string DataName { get; set; }
        public string Name => StringJsonData.GetString_static(DataName + "_" + ID, "Name");
        public float RTP { get; private set; }
        public string[] Motion { get; private set; }
        public string Voice { get; private set; }
        public bool IsAttack { get; private set; }
        public bool HasTimeline { get; private set; }
        public EffectType MyEffectType { get; private set; }
        public float EffectValue1 { get; private set; }
        public float EffectValue2 { get; private set; }

        protected override void GetDataFromJson(JsonData _item, string _dataName) {
            DataName = _dataName;
            JsonData item = _item;
            foreach (string key in item.Keys) {
                switch(key) {
                    case "ID":
                        ID = int.Parse(item[key].ToString());
                        break;
                    case "RTP":
                        RTP = float.Parse(item[key].ToString());
                        break;
                    case "Motion":
                        Motion = item[key].ToString().Split(',');
                        break;
                    case "Voice":
                        Voice = item[key].ToString();
                        break;
                    case "IsAttack":
                        IsAttack = bool.Parse(item[key].ToString());
                        break;
                    case "HasTimeline":
                        HasTimeline = bool.Parse(item[key].ToString());
                        break;
                    case "EffectType":
                        MyEffectType = MyEnum.ParseEnum<EffectType>(item[key].ToString());
                        break;
                    case "EffectValue1":
                        EffectValue1 = float.Parse(item[key].ToString());
                        break;
                    case "EffectValue2":
                        EffectValue2 = float.Parse(item[key].ToString());
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }

        protected override void ResetStaticData() {

        }

        public static DropSpellJsonData GetData(int id) {
            return GameDictionary.GetJsonData<DropSpellJsonData>(DataName, id);
        }
    }
}
