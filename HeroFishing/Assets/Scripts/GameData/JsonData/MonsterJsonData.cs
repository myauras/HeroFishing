using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;

namespace HeroFishing.Main {
    public class MonsterJsonData : MyJsonData {
        public static string DataName { get; set; }
        public string Name {
            get {
                return StringJsonData.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public string Ref { get; private set; }
        public float Odds { get; private set; }
        public float Radius { get; private set; }
        public float Speed { get; private set; }
        public int[] MonsterIDs { get; private set; }
        public enum MonsterType {
            Minion,
            Boss,
            Summon,
        }
        public MonsterType MyMonsterType { get; private set; }
        public int DropID { get; private set; }
        public int[] SummonSkillID { get; private set; }
        public enum HitEffectPosType {
            HitPos,
            Self,
        }
        public HitEffectPosType HitEffectPos = HitEffectPosType.HitPos;

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
                    case "Odds":
                        Odds = float.Parse(item[key].ToString());
                        break;
                    case "Radius":
                        Radius = float.Parse(item[key].ToString());
                        break;
                    case "Speed":
                        Speed = float.Parse(item[key].ToString());
                        break;
                    case "MonsterType":
                        MyMonsterType = MyEnum.ParseEnum<MonsterType>(item[key].ToString());
                        break;
                    case "DropID":
                        DropID = int.Parse(item[key].ToString());
                        break;
                    case "SummonSkillID":
                        SummonSkillID = TextManager.StringSplitToIntArray(item[key].ToString(), ',');
                        break;
                    case "HitEffectPos":
                        HitEffectPos = MyEnum.ParseEnum<HitEffectPosType>(item[key].ToString());
                        break;
                    default:
                        //WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static MonsterJsonData GetData(int id) {
            return GameDictionary.GetJsonData<MonsterJsonData>(DataName, id);
        }

        protected override void ResetStaticData() {
        }
    }

}
