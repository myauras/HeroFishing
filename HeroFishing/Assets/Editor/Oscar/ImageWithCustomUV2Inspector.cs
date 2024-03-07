using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ImageWithCustomUV2))]
public class ImageWithCustomUV2Inspector : ImageEditor
{
    private readonly string[] _options = { "Custom", "Rectangle" };
    private readonly GUIContent _blLabel = new GUIContent("Button Left");
    private readonly GUIContent _brLabel = new GUIContent("Button Right");
    private readonly GUIContent _tlLabel = new GUIContent("Top Left");
    private readonly GUIContent _trLabel = new GUIContent("Top Right");
    private bool _foldout = true;
    private int _selectedOption = -1;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var prop = serializedObject.FindProperty("_uvs2");
        if(prop.arraySize != 4) {
            ResetUVs(prop);
        }

        _foldout = EditorGUILayout.Foldout(_foldout, "UV2");
        if (_foldout)
        {
            EditorGUI.indentLevel++;
            DrawUVs(prop);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawUVs(SerializedProperty prop) {
        if (_selectedOption < 0) {
            CheckSelectedOption(prop);
        }

        _selectedOption = GUILayout.Toolbar(_selectedOption, _options);
        switch (_selectedOption) {
            case 1:
                DrawRectOption(prop);
                break;
            default:
                DrawCustomOption(prop);
                break;
        }
    }

    private void CheckSelectedOption(SerializedProperty prop) {
        var bl = prop.GetArrayElementAtIndex(0).vector2Value;
        var br = prop.GetArrayElementAtIndex(3).vector2Value;
        var tl = prop.GetArrayElementAtIndex(1).vector2Value;
        var tr = prop.GetArrayElementAtIndex(2).vector2Value;

        if (bl.x == tl.x && bl.y == br.y && tr.x == br.x && tr.y == tl.y)
            _selectedOption = 1;
        else
            _selectedOption = 0;
    }

    private void DrawCustomOption(SerializedProperty prop) {
        var width = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 100;
        EditorGUILayout.BeginHorizontal();
        DrawVector2Element(prop, 1, _tlLabel);
        DrawVector2Element(prop, 2, _trLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        DrawVector2Element(prop, 0, _blLabel);
        DrawVector2Element(prop, 3, _brLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = width;
    }

    private void DrawRectOption(SerializedProperty prop) {
        var width = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 100;

        var bl = prop.GetArrayElementAtIndex(0).vector2Value;
        var tr = prop.GetArrayElementAtIndex(2).vector2Value;

        var min = bl;
        var max = tr;
        EditorGUILayout.BeginHorizontal();
        min = EditorGUILayout.Vector2Field("min", min);
        max = EditorGUILayout.Vector2Field("max", max);
        EditorGUILayout.EndHorizontal();

        if(min != bl || max != tr) {
            prop.ClearArray();
            AddVector2(prop, min);
            AddVector2(prop, new Vector2(min.x, max.y));
            AddVector2(prop, max);
            AddVector2(prop, new Vector2(max.x, min.y));
        }

        EditorGUIUtility.labelWidth = width;
    }

    private void DrawVector2Element(SerializedProperty array, int index, GUIContent label) {
        var prop = array.GetArrayElementAtIndex(index);
        EditorGUILayout.PropertyField(prop, label);
    }

    private void ResetUVs(SerializedProperty prop) {
        prop.ClearArray();
        AddVector2(prop, Vector2.zero);
        AddVector2(prop, Vector2.up);
        AddVector2(prop, Vector2.one);
        AddVector2(prop, Vector2.right);
    }

    private void AddVector2(SerializedProperty array, Vector2 value) {
        var id = array.arraySize;
        array.InsertArrayElementAtIndex(id);
        var prop = array.GetArrayElementAtIndex(id);
        prop.vector2Value = value;
    }
}
