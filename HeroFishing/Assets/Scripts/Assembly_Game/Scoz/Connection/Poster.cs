using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Scoz.Func {
    public class Poster : MonoBehaviour {
        public static async UniTask<object> Post(string _url, string _bodyJson) {
            WriteLog.LogColorFormat("SendPost URL: {0}", WriteLog.LogType.Poster, _url);

            var request = new UnityWebRequest(_url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(_bodyJson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
                // 錯誤處理
                WriteLog.Log("Error: " + request.error);
                return null;
            }


            WriteLog.Log("Response Code: " + request.responseCode);
            WriteLog.Log("Result: " + request.result);

            // 從downloadHandler獲取回應內容
            string responseContent = request.downloadHandler.text;
            return responseContent;
        }
    }
}
