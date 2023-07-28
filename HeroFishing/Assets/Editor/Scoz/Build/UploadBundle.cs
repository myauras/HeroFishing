using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using Scoz.Func;
namespace Scoz.Editor {
    public class UploadBundle {
        const string DIALOG_MESSAGE = "<<<上傳資源包到GCP上>>>\n\n※請確認已安裝GoogleCloud工具\n※請確認已登入有權限的帳號\n\n環境: {0}\nBundle包版號: {1}\n";
        static Dictionary<EnvVersion, string> GOOGLE_PROJECT_DIC = new Dictionary<EnvVersion, string>() {
            { EnvVersion.Dev, "aurofortest"},
            { EnvVersion.Test, "aurofortest"},
            { EnvVersion.Release, "aurofortest"},
        };
        public static Dictionary<EnvVersion, string> GOOGLE_STORAGE_PATH_DIC = new Dictionary<EnvVersion, string>() {
            { EnvVersion.Dev, "aura_bundle_dev"},
            { EnvVersion.Test, "aura_bundle_dev"},
            { EnvVersion.Release, "aura_bundle_dev"},
        };
        [MenuItem("Scoz/UploadBundle/Dev")]
        public static void UploadBundleToDev() {
            bool isYes = EditorUtility.DisplayDialog("上傳資源包", string.Format(DIALOG_MESSAGE, "Dev", VersionSetting.AppLargeVersion), "好!", "先不要><");
            if (isYes)
                UploadGoogleCloud(EnvVersion.Dev);
        }
        [MenuItem("Scoz/UploadBundle/Test")]
        public static void UploadBundleToTest() {
            bool isYes = EditorUtility.DisplayDialog("上傳資源包", string.Format(DIALOG_MESSAGE, "Test", VersionSetting.AppLargeVersion), "好!", "先不要><");
            if (isYes)
                UploadGoogleCloud(EnvVersion.Test);
        }
        [MenuItem("Scoz/UploadBundle/Release")]
        public static void UploadBundleToRelease() {
            bool isYes = EditorUtility.DisplayDialog("上傳資源包", string.Format(DIALOG_MESSAGE, "Release", VersionSetting.AppLargeVersion), "好!", "先不要><");
            if (isYes)
                UploadGoogleCloud(EnvVersion.Release);
        }

        static void UploadGoogleCloud(EnvVersion _envVersion) {
            string googleProjectID = "";
            if (GOOGLE_PROJECT_DIC.TryGetValue(_envVersion, out string id)) {
                googleProjectID = id;
            } else {
                WriteLog.LogError("找不到GPC專案ID：" + _envVersion + " version.");
                return;
            }
            string storagePath = "";
            if (GOOGLE_STORAGE_PATH_DIC.TryGetValue(_envVersion, out string path)) {
                storagePath = path;
            } else {
                WriteLog.LogError("找不到GPC專案ID：" + _envVersion + " version.");
                return;
            }

            // start the child process
            Process process = new Process();
            WriteLog.LogFormat("專案ID: {0}  StoragePath: {1}  BundleVersion: {2}", googleProjectID, storagePath, VersionSetting.AppLargeVersion);
#if UNITY_EDITOR_WIN
            // redirect the output stream of the child process.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "UploadBundle.bat";
            process.StartInfo.Arguments = string.Format("{0} {1} {2}", googleProjectID, storagePath, VersionSetting.AppLargeVersion);
            process.StartInfo.WorkingDirectory = "./BuildTools";
#elif UNITY_ANDROID
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "UploadBundle.bat";
            process.StartInfo.Arguments = string.Format("{0} {1} {2}", googleProjectID, storagePath, VersionSetting.AppLargeVersion);
            process.StartInfo.WorkingDirectory = ""./BuildTools";
#elif UNITY_EDITOR_OSX
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;

        process.StartInfo.FileName = "/bin/sh";
        process.StartInfo.Arguments = Application.dataPath + string.Format("/../UploadBundle.sh {0} {1} {2}", googleProjectID, storagePath, VersionSetting.AppLargeVersion);
        process.StartInfo.WorkingDirectory = ""./BuildTools";
#endif
            int exitCode = -1;
            //string output = null;

            WriteLog.Log("開始上傳");
            try {
                process.Start();

                // do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // process.WaitForExit();

                // read the output stream first and then wait.
                //output = process.StandardOutput.ReadToEnd();
                //DebugLogger.Log(output);
                process.WaitForExit();
            } catch (Exception e) {
                WriteLog.LogError("發生錯誤：" + e.ToString()); // or throw new Exception
            } finally {
                exitCode = process.ExitCode;

                process.Dispose();
                process = null;
            }
            if (exitCode != 0) {
                WriteLog.LogError("上傳失敗 ExitCode：" + exitCode);
                EditorUtility.DisplayDialog("上傳資源包", "失敗", "嗚嗚嗚", "");
            } else {
                WriteLog.Log("上傳成功 ExitCode：" + exitCode);
                EditorUtility.DisplayDialog("上傳資源包", "成功", "好耶", "");
            }
        }
    }
}