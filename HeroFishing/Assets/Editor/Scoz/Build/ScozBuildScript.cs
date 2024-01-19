using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using Scoz.Func;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;
using System;

namespace Scoz.Editor {

    [CreateAssetMenu(fileName = "ScozBuildScript.asset", menuName = "Addressable Assets/Data Builders/Scoz Build")]
    public class ScozBuildScript : BuildScriptPackedMode {
        public override string Name => "Scoz Build";

        protected override string ConstructAssetBundleName(AddressableAssetGroup assetGroup, BundledAssetGroupSchema schema, BundleDetails info, string assetBundleName) {
            return "Bundle/" + base.ConstructAssetBundleName(assetGroup, schema, info, assetBundleName);
        }
        protected override TResult DoBuild<TResult>(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext) {
            return base.DoBuild<TResult>(builderInput, aaContext);
        }

        [MenuItem("Scoz/Build Bundle/NewBuild")]
        public static void NewBuild() {
            BuildDll();

            // 取得Addressable Asset設置
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) {
                Debug.LogError("找不到Addressable Assets設置。");
                return;
            }
            // 進行new Build
            AddressableAssetSettings.BuildPlayerContent();
            WriteLog.LogColor("New Build Bundle完成", WriteLog.LogType.Addressable);
        }

        [MenuItem("Scoz/Build Bundle/Update a previous build")]
        public static void UpdateAPreviousBuild() {
            BuildDll();
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = GetDefaultGroup();
            var path = ContentUpdateScript.GetContentStateDataPath(false);
            var result = ContentUpdateScript.BuildContentUpdate(settings, path);

            if (!string.IsNullOrEmpty(result.Error)) {
                WriteLog.LogError(result.Error);
                Debug.LogError(result.Error);
            } else {
                WriteLog.LogColor("更新Bundle完成", WriteLog.LogType.Addressable);
            }
        }
        public static AddressableAssetGroup GetDefaultGroup() {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings.DefaultGroup;
        }

        public static AddressableAssetGroup GetGroupByName(string groupName) {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings.FindGroup(groupName);
        }
        static void BuildDll() {
            BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
            string logPath = "ScozBuildLog";
            LogFile.AppendWrite(logPath, "\n");
            LogFile.AppendWrite(logPath, $"開始更新Dll  平台: {activeTarget}  版本: {VersionSetting.AppLargeVersion}");
            HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
            //HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            LogFile.AppendWrite(logPath, $"將需要的Dlls並追加.bytes結尾 並複製到AddressableAssets/Dlls/");

            // 刪除舊資料並重新建立目標資料夾
            string directoryPath = Path.Combine(Application.dataPath, "AddressableAssets/Dlls/");
            // 檢查目標資料夾是否存在
            if (Directory.Exists(directoryPath)) {
                try {
                    // 嘗試刪除目標資料夾
                    Directory.Delete(directoryPath, true);
                    LogFile.AppendWrite(logPath, $"已刪除目標資料夾{directoryPath}");
                } catch (Exception e) {
                    LogFile.AppendWrite(logPath, $"無法刪除目標資料夾({directoryPath})：{e.Message}");
                }
            }
            string targetDirectory = Path.GetDirectoryName(directoryPath);
            Directory.CreateDirectory(targetDirectory);
            LogFile.AppendWrite(logPath, $"已重新建立目標資料夾{directoryPath}");

            // 複製所有需要的Dlls並追加.bytes結尾
            LogFile.AppendWrite(logPath, $"開始複製DLLs");
            // Game
            string sourcePath = Path.Combine(Application.dataPath, $"../HybridCLRData/HotUpdateDlls/{activeTarget}/Game.dll");
            string targetPath = Path.Combine(Application.dataPath, "AddressableAssets/Dlls/Game.dll.bytes");
            try {
                File.Copy(sourcePath, targetPath);
                LogFile.AppendWrite(logPath, $"成功! 從 {sourcePath} 到 {targetPath}");
            } catch (Exception _e) {
                LogFile.AppendWrite(logPath, $"失敗! 從 {sourcePath} 到 {targetPath}  錯誤: {_e}");
            }
            // 補充元數據
            foreach (var item in AOTGenericReferences.PatchedAOTAssemblyList) {
                string dllName = item;
                // 檢查結尾是否為 ".dll" 如果不是，則追加 ".dll" (不知道為什麼Realm結尾不是.dll)
                if (!dllName.EndsWith(".dll")) {
                    dllName += ".dll";
                    LogFile.AppendWrite(logPath, $"PatchedAOTAssemblyList有資料不為.dll結尾 自動更名為.dll結尾 更名後{dllName}");
                }
                sourcePath = Path.Combine(Application.dataPath, $"../HybridCLRData/AssembliesPostIl2CppStrip/{activeTarget}/{dllName}");
                targetPath = Path.Combine(Application.dataPath, $"AddressableAssets/Dlls/{dllName}.bytes");
                try {
                    File.Copy(sourcePath, targetPath);
                    LogFile.AppendWrite(logPath, $"成功! 從 {sourcePath} 到 {targetPath}");
                } catch (Exception _e) {
                    LogFile.AppendWrite(logPath, $"失敗! 從 {sourcePath} 到 {targetPath}  錯誤: {_e}");
                }
            }
            LogFile.AppendWrite(logPath, "結束更新Dlls : " + VersionSetting.AppLargeVersion);
        }
    }
}