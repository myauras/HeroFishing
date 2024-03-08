using Cysharp.Threading.Tasks;
using LitJson;
using Scoz.Func;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Main {
    public class StartSceneManager : MonoBehaviour {
        [SerializeField] bool EditTest = true;
        private void Start() {
#if UNITY_EDITOR
            if (EditTest) {
                UniTask.Void(async () => {
                    var result = await SendRestfulAPI("game/getstate", "test");
                    JsonData jsonData = JsonMapper.ToObject(result.ToString());
                    string dataValue = jsonData["data"].ToString();
                    var review = bool.Parse(dataValue);
                    WriteLog_UnityAssembly.Log("review=" + review);
                    if (review) {
                        SceneManager.LoadScene(MyScene.menu.ToString(), LoadSceneMode.Single);
                    } else {
                        BaseManager.CreateNewInstance();
                    }
                });
            } else
                BaseManager.CreateNewInstance();
#else
            UniTask.Void(async () => {
                var result = await SendRestfulAPI("game/getstate", "test");
                JsonData jsonData = JsonMapper.ToObject(result.ToString());
                string dataValue = jsonData["data"].ToString();
                var review = bool.Parse(dataValue);
                WriteLog_UnityAssembly.Log("review=" + review);
                if (review) {
                    SceneManager.LoadScene(MyScene.menu.ToString(), LoadSceneMode.Single);
                } else {
                    BaseManager.CreateNewInstance();
                }
            });
#endif
        }
        /// <summary>
        /// 送RestfulAPI請求
        /// </summary>
        public static async UniTask<object> SendRestfulAPI(string _endPoint, string _valueJson) {
            string baseURL = "https://aurafordev.com/";
            string url = baseURL + _endPoint;
            string jsonPayload = $"{{}}";
            var result = await Poster.Post(url, jsonPayload);
            return result;
        }
    }
}