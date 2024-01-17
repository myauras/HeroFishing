using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad] // 確保腳本在 Unity 加載時被初始化
public class PlayModeCleanUp {
    static PlayModeCleanUp() {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingEditMode) {
            foreach (var testObject in Object.FindObjectsOfType<DestroyOnPlay>()) {
                Object.DestroyImmediate(testObject.gameObject);
            }
        }
    }
}
