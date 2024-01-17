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
            WriteLog_UnityAssembly.LogColor("New Build Bundle完成", WriteLog_UnityAssembly.LogType.Addressable);
        }

        [MenuItem("Scoz/Build Bundle/Update a previous build")]
        public static void UpdateAPreviousBuild() {
            BuildDll();
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = GetDefaultGroup();
            var path = ContentUpdateScript.GetContentStateDataPath(false);
            var result = ContentUpdateScript.BuildContentUpdate(settings, path);

            if (!string.IsNullOrEmpty(result.Error)) {
                WriteLog_UnityAssembly.LogError(result.Error);
                Debug.LogError(result.Error);
            } else {
                WriteLog_UnityAssembly.LogColor("更新Bundle完成", WriteLog_UnityAssembly.LogType.Addressable);
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
            LogFile.AppendWrite(logPath, $"開始包版! 平台: {activeTarget}  版本: {VersionSetting.AppLargeVersion}");
            LogFile.AppendWrite(logPath, "Build Game Assembly : " + VersionSetting.AppLargeVersion);
            HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            LogFile.AppendWrite(logPath, $"複製Dll路徑到AddressableAssets/Dlls/");

            string sourcePath = Path.Combine(Application.dataPath, $"../HybridCLRData/HotUpdateDlls/{activeTarget}/Game.dll");
            string targetPath = Path.Combine(Application.dataPath, "AddressableAssets/Dlls/Game.dll.bytes");
            // 確保目標資料夾夾存在
            string targetDirectory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDirectory)) {
                Directory.CreateDirectory(targetDirectory);
            }
            // 如果目標檔案已經存在，先刪除
            if (File.Exists(targetPath)) {
                File.Delete(targetPath);
            }
            // 複製並重新命名檔案
            File.Copy(sourcePath, targetPath);
            LogFile.AppendWrite(logPath, $"複製Dll完成 從 {sourcePath} 到 {targetPath}");
            LogFile.AppendWrite(logPath, "開始Build Bundle包 : " + VersionSetting.AppLargeVersion);
        }
    }
}