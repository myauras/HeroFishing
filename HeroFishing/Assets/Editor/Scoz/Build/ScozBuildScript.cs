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
            // ���oAddressable Asset�]�m
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) {
                Debug.LogError("�䤣��Addressable Assets�]�m�C");
                return;
            }
            // �i��new Build
            AddressableAssetSettings.BuildPlayerContent();
            WriteLog_UnityAssembly.LogColor("New Build Bundle����", WriteLog_UnityAssembly.LogType.Addressable);
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
                WriteLog_UnityAssembly.LogColor("��sBundle����", WriteLog_UnityAssembly.LogType.Addressable);
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
            LogFile.AppendWrite(logPath, $"�}�l�]��! ���x: {activeTarget}  ����: {VersionSetting.AppLargeVersion}");
            LogFile.AppendWrite(logPath, "Build Game Assembly : " + VersionSetting.AppLargeVersion);
            HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTarget();
            LogFile.AppendWrite(logPath, $"�ƻsDll���|��AddressableAssets/Dlls/");

            string sourcePath = Path.Combine(Application.dataPath, $"../HybridCLRData/HotUpdateDlls/{activeTarget}/Game.dll");
            string targetPath = Path.Combine(Application.dataPath, "AddressableAssets/Dlls/Game.dll.bytes");
            // �T�O�ؼи�Ƨ����s�b
            string targetDirectory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDirectory)) {
                Directory.CreateDirectory(targetDirectory);
            }
            // �p�G�ؼ��ɮפw�g�s�b�A���R��
            if (File.Exists(targetPath)) {
                File.Delete(targetPath);
            }
            // �ƻs�í��s�R�W�ɮ�
            File.Copy(sourcePath, targetPath);
            LogFile.AppendWrite(logPath, $"�ƻsDll���� �q {sourcePath} �� {targetPath}");
            LogFile.AppendWrite(logPath, "�}�lBuild Bundle�] : " + VersionSetting.AppLargeVersion);
        }
    }
}