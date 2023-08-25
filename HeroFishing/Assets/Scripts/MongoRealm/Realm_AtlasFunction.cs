using System;
using System.Collections.Generic;
using LitJson;
using Scoz.Func;

namespace Service.Realms {
    public static partial class RealmManager {

        public enum AtlasFunc {
            Signup,
        }

        public static async void CallAtlasFunc(AtlasFunc _func, params object[] _params) {
            var bsonValue = await MyApp.CurrentUser.Functions.CallAsync(_func.ToString(), _params);

            string jsonResult = null;
            if (_params.Length == 0) jsonResult = await MyApp.CurrentUser.Functions.CallAsync<string>(_func.ToString());
            else jsonResult = await MyApp.CurrentUser.Functions.CallAsync<string>(_func.ToString(), _params);
            try {
                WriteLog.LogColorFormat("jsonResult: {0}", WriteLog.LogType.Realm, jsonResult);
                Dictionary<string, object> resultDictionary = JsonMapper.ToObject<Dictionary<string, object>>(jsonResult);
                foreach (var key in resultDictionary.Keys) {
                    WriteLog.Log("key=" + key);
                }

            } catch (Exception _e) {
                WriteLog.LogError("CallAtlasFunc回傳發生錯誤: " + _e);
            }

        }

    }
}