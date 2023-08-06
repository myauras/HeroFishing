using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;

namespace HeroFishing.Main {
    public class HeroSpellData : MyJsonData {
        public enum SpellType {
            Line,
            Sector,
        }
        public enum TargetType {
            Single,
            Piercing,
        }
        public enum HitType {
            None,
            Chain,
            Explode
        }
        public static string DataName { get; set; }
        public new string ID { get; private set; }
        public string Name {
            get {
                return StringData.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public string Description {
            get {
                return StringData.GetString_static(DataName + "_" + ID, "Description");
            }
        }
        public string Ref { get; private set; }
        public SpellName SpellName { get; private set; }
        public float RTP { get; private set; }
        public float CD { get; private set; }
        public int Cost { get; private set; }
        public int Waves { get; private set; }
        public SpellType MySpellType { get; private set; }
        public string[] SpellValues { get; private set; }
        public TargetType MyTargetType { get; private set; }
        public HitType MyHitType { get; private set; }
        public string[] HitValues { get; private set; }
        public int[] Threshold { get; private set; }
        public string[] Motions { get; private set; }
        public string Voice { get; private set; }


        protected override void GetDataFromJson(JsonData _item, string _dataName) {
            DataName = _dataName;
            JsonData item = _item;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = item[key].ToString();
                        break;
                    case "Ref":
                        Ref = item[key].ToString();
                        break;
                    case "SpellName":
                        SpellName = MyEnum.ParseEnum<SpellName>(item[key].ToString());
                        break;
                    case "RTP":
                        RTP = float.Parse(item[key].ToString());
                        break;
                    case "CD":
                        CD = float.Parse(item[key].ToString());
                        break;
                    case "Cost":
                        Cost = int.Parse(item[key].ToString());
                        break;
                    case "Waves":
                        Waves = int.Parse(item[key].ToString());
                        break;
                    case "SpellType":
                        MySpellType = MyEnum.ParseEnum<SpellType>(item[key].ToString());
                        break;
                    case "SpellValues":
                        SpellValues = item[key].ToString().Split(',');
                        break;
                    case "TargetType":
                        MyTargetType = MyEnum.ParseEnum<TargetType>(item[key].ToString());
                        break;
                    case "HitType":
                        MyHitType = MyEnum.ParseEnum<HitType>(item[key].ToString());
                        break;
                    case "HitValues":
                        HitValues = item[key].ToString().Split(',');
                        break;
                    case "Threshold":
                        Threshold = TextManager.StringSplitToIntArray(item[key].ToString(), ',');
                        break;
                    case "Motions":
                        Motions = item[key].ToString().Split(',');
                        break;
                    case "Voice":
                        Voice = item[key].ToString();
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static HeroSpellData GetData(int id) {
            return GameDictionary.GetJsonData<HeroSpellData>(DataName, id);
        }

    }

}
