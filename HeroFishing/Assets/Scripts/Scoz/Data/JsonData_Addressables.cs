using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.Linq;
using UnityEngine.AddressableAssets;
using HeroFishing.Main;

namespace Scoz.Func {
    /// <summary>
    /// 這是Excel輸出的Json資料父類別，繼承自這個類的都是Excel表輸出的資料
    /// </summary>
    public abstract partial class MyJsonData {

        /// <summary>
        /// 重置靜態資料，當Addressable重載json資料時需要先呼叫這個方法來重置靜態資料
        /// </summary>
        protected abstract void ResetStaticData();


        /// <summary>
        /// 依json表設定資料(Key為int)
        /// </summary>
        public static void SetData_Remote<T>(string _dataName, Action<string, Dictionary<int, MyJsonData>> _cb) where T : MyJsonData, new() {

            GameDictionary.AddLoadingKey(_dataName);
            try {
                Addressables.LoadAssetAsync<TextAsset>(string.Format("Assets/AddressableAssets/Json/{0}.json", _dataName)).Completed += handle => {
                    string jsonStr = handle.Result.text;
                    JsonData jd = JsonMapper.ToObject(jsonStr);
                    JsonData items = jd[_dataName];
                    Dictionary<int, MyJsonData> dic = new Dictionary<int, MyJsonData>();
                    for (int i = 0; i < items.Count; i++) {
                        T data = new T();
                        if (i == 0) data.ResetStaticData();//在取第一筆資料前先重置靜態資料
                        data.GetDataFromJson(items[i], _dataName);
                        int id = 0;
                        if (!int.TryParse(items[i]["ID"].ToString(), out id)) {
                            WriteLog.LogErrorFormat("Wrong ID Format DataName:{0}.json ID:{1} 也許使用SetDataStringKey_Remote而不是SetData_Remote", _dataName, items[i]["ID"].ToString());
                            continue;
                        }
                        if (!dic.ContainsKey(id))
                            dic.Add(id, data);
                        else
                            WriteLog.LogError(string.Format("{0}表有重複的ID {1}", _dataName, id));
                    }
                    Addressables.Release(handle);
                    if (ShowLoadTime) {
                        WriteLog.LogColorFormat("{0}.json載入完成", WriteLog.LogType.Json, _dataName);
                    }
                    _cb?.Invoke(_dataName, dic);
                };
            } catch {
                WriteLog.LogErrorFormat("載入{0}.json時發生錯誤", _dataName);
            }
        }
        /// <summary>
        /// 依json表設定資料(Key為String)
        /// </summary>
        public static void SetDataStringKey_Remote<T>(string _dataName, Action<string, Dictionary<string, MyJsonData>> _cb) where T : MyJsonData, new() {
            GameDictionary.AddLoadingKey(_dataName);
            try {
                Addressables.LoadAssetAsync<TextAsset>(string.Format("Assets/AddressableAssets/Json/{0}.json", _dataName)).Completed += handle => {
                    string jsonStr = handle.Result.text;
                    JsonData jd = JsonMapper.ToObject(jsonStr);
                    JsonData items = jd[_dataName];
                    Dictionary<string, MyJsonData> dic = new Dictionary<string, MyJsonData>();
                    for (int i = 0; i < items.Count; i++) {
                        T data = new T();
                        if (i == 0) data.ResetStaticData();//在取第一筆資料前先重置靜態資料
                        data.GetDataFromJson(items[i], _dataName);
                        string id = items[i]["ID"].ToString();
                        if (!dic.ContainsKey(id))
                            dic.Add(id, data);
                        else
                            WriteLog.LogError(string.Format("{0}表有重複的ID {1}", _dataName, id));

                    }
                    Addressables.Release(handle);
                    if (ShowLoadTime) {
                        WriteLog.LogColorFormat("{0}.json載入完成", WriteLog.LogType.Json, _dataName);
                    }
                    _cb?.Invoke(_dataName, dic);

                };
            } catch {
                WriteLog.LogErrorFormat("載入{0}.json時發生錯誤", _dataName);
            }

        }
    }
}
