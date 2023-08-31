using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LitJson;
using Scoz.Func;

namespace Service.Realms {
    public static partial class RealmManager {
        enum ReplyKey {
            Data,//回傳資料放這裡
            Error,//有Error文字會放這裡, 沒有就會是null
        }

        static Dictionary<string, object> HandleReplyData(string _replyJson) {
            try {
                var jsonObj = JsonMapper.ToObject(_replyJson.UnwrapQuotedJson());
                if (jsonObj == null) {
                    WriteLog.LogError("AtlasFunc回傳json無法轉為jsonObj: " + _replyJson);
                    return null;
                }
                var iDic = jsonObj as IDictionary;
                if (iDic == null) {
                    WriteLog.LogError("AtlasFunc回傳jsonObj無法轉為IDictionary類型: " + _replyJson);
                    return null;
                }
                if (!iDic.TryGetValue(ReplyKey.Error.ToString(), out object error)) {
                    WriteLog.LogError("AtlasFunc回傳的資料缺少欄位: " + ReplyKey.Error);
                    return null;
                }
                if (error != null) {
                    WriteLog.LogError("AtlasFunc錯誤: " + error);
                    return null;
                }
                if (!iDic.TryGetValue(ReplyKey.Data.ToString(), out object replyData)) {
                    WriteLog.LogError("AtlasFunc回傳的資料缺少欄位: " + ReplyKey.Error);
                    return null;
                }
                var dic = DicExtension.ConvertToStringKeyDic(replyData);
                return dic;
            } catch (Exception _e) {
                WriteLog.LogError("AtlasFunc回傳的資料解析發生錯誤: " + _e);
                return null;
            }
        }

        public enum AtlasFunc {
            InitPlayerData,//註冊玩家資料
        }

        /// <summary>
        /// 呼叫MonsgoDB Atlas 傳入方法名稱與參數 
        /// </summary>
        /// <param name="_func">方法名稱</param>
        /// <param name="_params">傳入參數字典</param>
        /// <returns>回傳結果字典</returns>
        public static async Task<Dictionary<string, object>> CallAtlasFunc(AtlasFunc _func, Dictionary<string, object> _data) {

            string jsonResult = null;
            if (_data == null) jsonResult = await MyApp.CurrentUser.Functions.CallAsync<string>(_func.ToString());
            else jsonResult = await MyApp.CurrentUser.Functions.CallAsync<string>(_func.ToString(), _data);
            try {
                //WriteLog.LogColorFormat("jsonResult: {0}", WriteLog.LogType.Realm, jsonResult);
                var dataDic = HandleReplyData(jsonResult);
                //dataDic.Log();
                return dataDic;
            } catch (Exception _e) {
                WriteLog.LogError("CallAtlasFunc回傳發生錯誤: " + _e);
                return null;
            }

        }

    }
}