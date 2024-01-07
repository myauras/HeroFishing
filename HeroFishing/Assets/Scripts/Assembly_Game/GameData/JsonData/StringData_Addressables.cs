using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine.AddressableAssets;
using System.Text;

namespace Scoz.Func {
    public partial class StringJsonData {
        public static void GetStringDic_Remote(string _dataName, Action<Dictionary<string, StringJsonData>> _cb) {
            GameDictionary.AddLoadingKey(_dataName);
            Dictionary<string, StringJsonData> dic = new Dictionary<string, StringJsonData>();
            Addressables.LoadAssetAsync<TextAsset>(string.Format("Assets/AddressableAssets/Jsons/{0}.json", _dataName)).Completed += handle => {
                string jsonStr = handle.Result.text;
                JsonData jd = JsonMapper.ToObject(jsonStr);
                JsonData items = jd[_dataName];
                for (int i = 0; i < items.Count; i++) {
                    StringJsonData data = new StringJsonData(items[i], _dataName);
                    string name = GetValue<string>(items[i]["ID"].ToString());
                    if (dic.ContainsKey(name)) {
                        WriteLog.LogWarning(string.Format("{0}表的欄位({1})重複", _dataName, name));
                        break;
                    }
                    dic.Add(name, data);
                }
                Addressables.Release(handle);
                if (ShowLoadTime) {
                    WriteLog.LogFormat("<color=#398000>[Json] {0}.json載入完成</color>", _dataName);
                }
                _cb?.Invoke(dic);
            };
        }
    }
}
