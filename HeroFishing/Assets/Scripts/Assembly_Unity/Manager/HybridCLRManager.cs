using Cysharp.Threading.Tasks;
using HybridCLR;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        await LoadGameAssembly();
        await LoadMetadataForAOTAssemblies();
    }
    static async UniTask LoadGameAssembly() {
        WriteLog_UnityAssembly.LogColorFormat("開始載入Game Assembly", WriteLog_UnityAssembly.LogType.HybridCLR);
        var result = await AddressablesLoader_UnityAssebly.GetResourceByFullPath_Async<TextAsset>("Assets/AddressableAssets/Dlls/Dlls/Game.dll.bytes");
        TextAsset dll = result.Item1;
        var gameAssembly = System.Reflection.Assembly.Load(dll.bytes);
        Addressables.Release(result.Item2);
        WriteLog_UnityAssembly.LogColorFormat("載入Game Assembly完成: {0}", WriteLog_UnityAssembly.LogType.HybridCLR, gameAssembly);
    }
    static async UniTask LoadMetadataForAOTAssemblies() {
        WriteLog_UnityAssembly.LogColorFormat("開始補充元數據", WriteLog_UnityAssembly.LogType.HybridCLR);
        List<string> aotDllList = GetGameAssemblyAotMetaData();
        foreach (var aotDllName in aotDllList) {
            string path = string.Format("Assets/AddressableAssets/Dlls/Dlls/{0}.bytes", aotDllName);
            WriteLog_UnityAssembly.LogColorFormat("載入元數據Dll: {0}", WriteLog_UnityAssembly.LogType.Addressable, path);
            var result = await AddressablesLoader_UnityAssebly.GetResourceByFullPath_Async<TextAsset>(path);
            TextAsset dll = result.Item1;
            var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dll.bytes, HomologousImageMode.SuperSet);
            Addressables.Release(result.Item2);
            WriteLog_UnityAssembly.LogColorFormat("LoadMetadataForAOTAssembly:{0}. ret:{1}", WriteLog_UnityAssembly.LogType.HybridCLR, aotDllName, err);

        }
        WriteLog_UnityAssembly.LogColorFormat("補充元數據完成", WriteLog_UnityAssembly.LogType.HybridCLR);
    }
    /// <summary>
    /// 呼叫GameAssembly取需要下載的元數據清單
    /// </summary>
    static List<string> GetGameAssemblyAotMetaData() {
        Assembly targetAssembly = Assembly.Load("Game");
        // 取得 AOTMetadata 類別型別
        Type aotMetadataType = targetAssembly.GetType("AOTMetadata");

        if (aotMetadataType != null) {
            // 取 AotDllList 屬性資訊
            PropertyInfo aotDllListProperty = aotMetadataType.GetProperty("AotDllList");

            if (aotDllListProperty != null) {
                // 取 AotDllList 屬性的值
                List<string> aotDllList = (List<string>)aotDllListProperty.GetValue(null);
                foreach (string dllName in aotDllList) {
                    WriteLog_UnityAssembly.LogColor("需載入元數據:" + dllName, WriteLog_UnityAssembly.LogType.HybridCLR);
                }
                return aotDllList;
            } else {
                WriteLog_UnityAssembly.LogError("抓不到AotDllList 屬性, 有可能Game Assembly的AOTMetadata名稱或命名空間有人改到");
            }
        } else {
            WriteLog_UnityAssembly.LogError("抓不到AotDllList 屬性, 有可能Game Assembly的AOTMetadata名稱或命名空間有人改到");
        }

        return null;

    }
}
