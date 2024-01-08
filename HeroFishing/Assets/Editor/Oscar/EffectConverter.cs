using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectConverter : EditorWindow {
    private GameObject _prefab;
    [MenuItem("Tools/Effect Converter")]
    public static void ShowWindow() {
        EffectConverter wnd = GetWindow<EffectConverter>();
        wnd.titleContent = new GUIContent("Effect Converter");
    }

    private void OnGUI() {
        _prefab = EditorGUILayout.ObjectField(new GUIContent("Prefab"), _prefab, typeof(GameObject), false) as GameObject;
        if (_prefab != null) {
            var path = AssetDatabase.GetAssetPath(_prefab);
            if (GUILayout.Button("Convert")) {
                var variantPath = path.Replace(".prefab", " Variant.prefab");

                var variant = AssetDatabase.LoadAssetAtPath<GameObject>(variantPath);
                if (variant == null) {
                    var srcPrefab = PrefabUtility.InstantiatePrefab(_prefab) as GameObject;
                    variant = PrefabUtility.SaveAsPrefabAsset(srcPrefab, variantPath);
                    DestroyImmediate(srcPrefab);
                }

                if (_prefab.TryGetComponent<AutoBackPool>(out var srcABP)) {
                    var dstABP = variant.AddComponent<AutoBackPool>();
                    Debug.Log("auto back pool copy success");
                    EditorUtility.CopySerialized(srcABP, dstABP);
                    Observable.ReturnUnit().DelayFrame(1).Subscribe(_ => {
                        DestroyImmediate(srcABP, true);
                    });

                }
                else {
                    Debug.LogWarning("auto back pool no component copy");
                }

                var srcTrails = _prefab.GetComponentsInChildren<ClearTrailBehaviour>();
                if (srcTrails != null)
                    Debug.Log(srcTrails.Length);
                if (srcTrails != null && srcTrails.Length > 0) {
                    var oldTrails = variant.GetComponentsInChildren<ClearTrailBehaviour>();

                    for (int i = 0; i < srcTrails.Length; i++) {
                        var dstTrail = oldTrails[i].gameObject.AddComponent<ClearTrailBehaviour>();
                        EditorUtility.CopySerialized(srcTrails[i], dstTrail);
                        Debug.Log($"clear trail {i} success");
                        var deleteTrail = srcTrails[i];
                        Observable.ReturnUnit().DelayFrame(1).Subscribe(_ => {
                            DestroyImmediate(deleteTrail, true);
                        });
                    }
                }
                AssetDatabase.RenameAsset(variantPath, $"Script_{_prefab.name}");
            }
        }
    }
}
