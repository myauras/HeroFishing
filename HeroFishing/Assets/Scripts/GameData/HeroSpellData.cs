using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;

namespace HeroFishing.Main {
    public enum SpellName {
        attack,
        spell1,
        spell2,
        spell3,
    }
    public class HeroSpellData : MyJsonData {
        public enum SpellType {
            LineShot,//直線命中路徑上第一個目標 參數說明：[指示物長度,子彈寬度,子彈速度] 
            SpreadLineShot,//錐形直線散射命中路徑上第一個目標 參數說明：[指示物長度,子彈寬度,子彈寬度,散射間隔角度,散射數量]
            LineRange,//直線命中範圍內目標 參數說明：[指示物長度,子彈寬度]
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
        public string[] SpellTypeValues { get; private set; }
        public HitType MyHitType { get; private set; }
        public string[] HitTypeValues { get; private set; }
        public int[] Threshold { get; private set; }
        public string[] Motions { get; private set; }
        public string Voice { get; private set; }
        static Dictionary<int, Dictionary<SpellName, HeroSpellData>> SpellDic = new Dictionary<int, Dictionary<SpellName, HeroSpellData>>();

        /// <summary>
        /// 重置靜態資料，當Addressable重載json資料時需要先呼叫這個方法來重置靜態資料
        /// </summary>
        public static void ResetStaticData() {
            SpellDic = null;
        }
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
                        SpellTypeValues = item[key].ToString().Split(',');
                        break;
                    case "HitType":
                        MyHitType = MyEnum.ParseEnum<HitType>(item[key].ToString());
                        break;
                    case "HitValues":
                        HitTypeValues = item[key].ToString().Split(',');
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
            AddToSpellDic(ID, this);
        }


        static void AddToSpellDic(string _id, HeroSpellData _data) {
            string[] strs = _id.Split('_');
            if (strs.Length != 2) return;
            if (string.IsNullOrEmpty(strs[1])) return;

            SpellName spellName;
            if (!MyEnum.TryParseEnum(strs[1], out spellName)) {
                WriteLog.LogErrorFormat("HeroSpell ID為 {0} 的格式應該為 HeroID_SpellName", _id);
                return;
            }
            _data.SpellName = spellName;
            if (int.TryParse(strs[0], out int _heroID)) {
                if (SpellDic.ContainsKey(_heroID)) {
                    if (SpellDic[_heroID].ContainsKey(spellName))
                        WriteLog.LogErrorFormat("重複的SpellName: {0}", strs[1]);
                    SpellDic[_heroID][spellName] = _data;
                } else {
                    SpellDic.Add(_heroID, new Dictionary<SpellName, HeroSpellData>() { { spellName, _data } });
                }
            }

        }

        public static HeroSpellData GetData(string _id) {
            return GameDictionary.GetJsonData<HeroSpellData>(DataName, _id);
        }
        public static Dictionary<SpellName, HeroSpellData> GetSpellDic(int _heroID) {
            if (!SpellDic.ContainsKey(_heroID)) return null;
            return SpellDic[_heroID];
        }
        public static HeroSpellData GetSpell(int _heroID, SpellName _spellName) {
            if (!SpellDic.ContainsKey(_heroID)) return null;
            if (!SpellDic[_heroID].ContainsKey(_spellName)) return null;
            return SpellDic[_heroID][_spellName];
        }
    }

}
