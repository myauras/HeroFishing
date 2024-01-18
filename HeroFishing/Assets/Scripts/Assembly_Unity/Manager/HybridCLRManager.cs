using Cysharp.Threading.Tasks;
using HybridCLR;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class HybridCLRManager : MonoBehaviour {

    /// <summary>
    /// 載入GameDll
    /// </summary>
    public static async UniTask LoadAssembly() {
#if UNITY_EDITOR
        return;
#endif
        await LoadMetadataForAOTAssemblies();
        await LoadGameAssembly();
    }
    static async UniTask LoadGameAssembly() {
        WriteLog_UnityAssembly.LogColorFormat("開始載入Game Assembly", WriteLog_UnityAssembly.LogType.HybridCLR);
        var result = await AddressablesLoader_UnityAssebly.GetResourceByFullPath_Async<TextAsset>("Assets/AddressableAssets/Dlls/Game.dll.bytes");
        TextAsset dll = result.Item1;
        var gameAssembly = System.Reflection.Assembly.Load(dll.bytes);
        Addressables.Release(result.Item2);
        WriteLog_UnityAssembly.LogColorFormat("載入Game Assembly完成: {0}", WriteLog_UnityAssembly.LogType.HybridCLR, gameAssembly);
    }
    static async UniTask LoadMetadataForAOTAssemblies() {
        WriteLog_UnityAssembly.LogColorFormat("開始補充元數據", WriteLog_UnityAssembly.LogType.HybridCLR);
        List<string> aotDllList = new List<string>
        {
            "Cinemachine.dll",
            "DOTween.dll",
            "FlutterUnityIntegration.dll",
            "LitJson.dll",
            "Loxodon.Framework.dll",
            "Realm.dll",
            "SerializableDictionary.dll",
            "System.Core.dll",
            "System.dll",
            "UniRx.dll",
            "UniTask.dll",
            "Unity.Addressables.dll",
            "Unity.Burst.dll",
            "Unity.Collections.dll",
            "Unity.Entities.Hybrid.dll",
            "Unity.Entities.dll",
            "Unity.RenderPipelines.Core.Runtime.dll",
            "Unity.ResourceManager.dll",
            "Unity.VisualScripting.Core.dll",
            "UnityEngine.AndroidJNIModule.dll",
            "UnityEngine.CoreModule.dll",
            "mscorlib.dll",
        };

        foreach (var aotDllName in aotDllList) {
            string path = string.Format("Assets/AddressableAssets/Dlls/{0}.bytes", aotDllName);
            WriteLog_UnityAssembly.LogColorFormat("載入元數據Dll: {0}", WriteLog_UnityAssembly.LogType.Addressable, path);
            var result = await AddressablesLoader_UnityAssebly.GetResourceByFullPath_Async<TextAsset>(path);
            TextAsset dll = result.Item1;
            var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dll.bytes, HomologousImageMode.SuperSet);
            Addressables.Release(result.Item2);
            WriteLog_UnityAssembly.LogColorFormat("LoadMetadataForAOTAssembly:{0}. ret:{1}", WriteLog_UnityAssembly.LogType.HybridCLR, aotDllName, err);

        }
        WriteLog_UnityAssembly.LogColorFormat("補充元數據完成", WriteLog_UnityAssembly.LogType.HybridCLR);
    }
}
