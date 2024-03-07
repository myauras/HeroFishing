using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using LitJson;
using System;
using System.Linq;
using static HeroFishing.Main.HeroJsonData;

namespace HeroFishing.Main {
    public class HeroJsonData : MyJsonData {
        public enum RoleCategory {
            All,
            DOTA2,
            LOL,
        }
        public enum RoleType {
            Shooter,
            Mage,
            AGI,
            INT,
            STR,
        }
        public static string DataName { get; set; }
        public string Name {
            get {
                return StringJsonData.GetString_static(DataName + "_" + ID, "Name");
            }
        }
        public string Title {
            get {
                return StringJsonData.GetString_static(DataName + "_" + ID, "Title");
            }
        }
        public string Ref { get; private set; }
        public RoleCategory MyRoleCategory { get; private set; }
        public RoleType MyRoleType { get; private set; }
        public string[] IdleMotions { get; private set; }
        protected override void GetDataFromJson(JsonData _item, string _dataName) {
            DataName = _dataName;
            JsonData item = _item;
            foreach (string key in item.Keys) {
                switch (key) {
                    case "ID":
                        ID = int.Parse(item[key].ToString());
                        break;
                    case "RoleType":
                        MyRoleType = MyEnum.ParseEnum<RoleType>(item[key].ToString());
                        break;
                    case "Ref":
                        Ref = item[key].ToString();
                        break;
                    case "RoleCategory":
                        MyRoleCategory = MyEnum.ParseEnum<RoleCategory>(item[key].ToString());
                        break;
                    case "IdleMotions":
                        IdleMotions = item[key].ToString().Split(',');
                        break;
                    default:
                        WriteLog.LogWarning(string.Format("{0}表有不明屬性:{1}", DataName, key));
                        break;
                }
            }
        }
        public static HeroJsonData GetData(int id) {
            return GameDictionary.GetJsonData<HeroJsonData>(DataName, id);
        }


        protected override void ResetStaticData() {
        }
    }

}
