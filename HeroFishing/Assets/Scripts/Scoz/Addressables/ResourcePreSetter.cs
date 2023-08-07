using HeroFishing.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scoz.Func {
    public class ResourcePreSetter : MonoBehaviour {
        [Serializable] public class MaterialDicClass : SerializableDictionary<string, Material> { }

        [HeaderAttribute("==============直接引用的資源==============")]

        [SerializeField] MaterialDicClass MyMaterialDic;//材質字典
        [SerializeField] public Monster MonsterPrefab;

        //[HeaderAttribute("==============AssetReference引用的資源==============")]

        //static Dictionary<string, GameObject> BulletPrefabs = new Dictionary<string, GameObject>();

        public static ResourcePreSetter Instance;

        public void Init() {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static Material GetMaterial(string _str) {
            if (Instance == null || Instance.MyMaterialDic == null) return null;
            return Instance.MyMaterialDic.ContainsKey(_str) ? Instance.MyMaterialDic[_str] : null;
        }


        //public static void PreLoadBulletPrefabs() {
        //    WriteLog.Log("預載BulletPrefabs");
        //    var monsterDatas = GetIntKeyJsonDic<MonsterData>("HeroSkill");
        //    foreach (var data in monsterDatas.Values) {
        //        var tmData = data;
        //        if (string.IsNullOrEmpty(tmData.Ref)) continue;
        //        string path = string.Format("Monster/{0}", tmData.Ref);
        //        AddressablesLoader.GetPrefab(path, (go, handle) => {

        //            WriteLog.LogFormat("載入MonsterPrefab {0} 完成", tmData.Ref);
        //        });
        //    }
        //}
    }
}