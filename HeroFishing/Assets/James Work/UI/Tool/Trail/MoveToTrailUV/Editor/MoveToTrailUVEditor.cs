using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveToTrailUV))][CanEditMultipleObjects]
public class MoveToTrailUVEditor : Editor
{
   

    SerializedProperty m_moveObject_sp;
    SerializedProperty m_shaderPropertyName_sp;
    SerializedProperty m_shaderPropertyID_sp;
    SerializedProperty m_materialData_sp;

    private MoveToTrailUV m_mttuv;

    private void OnEnable()
    {
        
        m_moveObject_sp = serializedObject.FindProperty("m_moveObject");
        m_shaderPropertyName_sp = serializedObject.FindProperty("m_shaderPropertyName");
        m_shaderPropertyID_sp = serializedObject.FindProperty("m_shaderPropertyID");
        m_materialData_sp = serializedObject.FindProperty("m_materialData");

        m_mttuv = target as MoveToTrailUV;
        
        serializedObject.Update();
        InitializeEditor();
        serializedObject.ApplyModifiedProperties();
    }

    private void OnDisable()
    {
        serializedObject.Update(); 

        for (int i = 0; i < m_materialData_sp.arraySize; i++)
        {
            SerializedProperty materialDataElement_sp = m_materialData_sp.GetArrayElementAtIndex(i);
            TrailRenderer trailRenderer = (TrailRenderer)materialDataElement_sp.FindPropertyRelative("m_trailRenderer").objectReferenceValue;
            if (trailRenderer != null)
            {
                Material mat = trailRenderer.sharedMaterial;
                if (mat != null)
                {
                    mat.SetFloat(m_shaderPropertyID_sp.intValue, 0f);
                }
            }
        }
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        int checkTrailRenderertile = CheckTrailRendererTile();
        if (checkTrailRenderertile != -1)
        {
            string message = String.Format(" 請用Tile mode >////< ", checkTrailRenderertile);
            EditorGUILayout.HelpBox(message, MessageType.Warning);
        }

        string checkTrailRendererShader = CheckTrailRendererShader();
        if (checkTrailRendererShader != "")
        {
            EditorGUILayout.HelpBox(checkTrailRendererShader, MessageType.Warning);
        }

        EditorGUI.BeginChangeCheck();
        {
      
            EditorGUILayout.PropertyField(m_moveObject_sp);
            EditorGUILayout.PropertyField(m_shaderPropertyName_sp);
            EditorGUILayout.PropertyField(m_materialData_sp);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "MoveToTrailUV changed");
            InitializeEditor();
        }

        for (int i = 0; i < m_materialData_sp.arraySize; i++)
        {
            SerializedProperty materialDataElement_sp = m_materialData_sp.GetArrayElementAtIndex(i);
            SyncElementTiling(materialDataElement_sp); 
        }
        serializedObject.ApplyModifiedProperties();
        
    }

    private void SyncElementTiling(SerializedProperty materialDataElement_sp) 
    {
        TrailRenderer trailRenderer = (TrailRenderer)materialDataElement_sp.FindPropertyRelative("m_trailRenderer").objectReferenceValue;
        
        if (trailRenderer != null)
        {
            
            Material mat = trailRenderer.sharedMaterial;
            if (mat != null)
            {
                if (materialDataElement_sp.FindPropertyRelative("m_uvTiling").vector2Value != mat.mainTextureScale)
                {
                    
                    if (false)
                    {
                        
                    }
                    else
                    {
                       
                        materialDataElement_sp.FindPropertyRelative("m_uvTiling").vector2Value = mat.mainTextureScale;
                    }
                }
            }
        }
    }

    
    private void InitializeEditor()
    {
        if (m_materialData_sp.serializedObject.targetObject == null || m_materialData_sp.arraySize == 0)
            return;

        m_shaderPropertyID_sp.intValue = Shader.PropertyToID(m_shaderPropertyName_sp.stringValue);

        
        for (int i = 0; i < m_materialData_sp.arraySize; i++)
        {
            SerializedProperty materialDataElement_sp = m_materialData_sp.GetArrayElementAtIndex(i);
            SyncElementTiling(materialDataElement_sp); 

            
            materialDataElement_sp.FindPropertyRelative("m_move").floatValue = 0f;
        }
    }

   
    private int CheckTrailRendererTile()
    {
        if (m_mttuv.m_materialData.Length == 0)
            return -1;

        for (int i = 0; i < m_mttuv.m_materialData.Length; i++)
        {
            if (m_mttuv.m_materialData[i].m_trailRenderer == null)
                continue;
            TrailRenderer trailRenderer = (TrailRenderer)m_mttuv.m_materialData[i].m_trailRenderer;
            if (trailRenderer.textureMode != LineTextureMode.Tile)
            {
                return i;
            }
        }
        return -1;
    }

    private string CheckTrailRendererShader()
    {
        if (m_mttuv.m_materialData.Length == 0)
            return "";

        for (int i = 0; i < m_mttuv.m_materialData.Length; i++)
        {
            TrailRenderer trailRenderer = m_mttuv.m_materialData[i].m_trailRenderer;
            if (trailRenderer == null)
                continue;
            Material mat = trailRenderer.sharedMaterial;
            
            if (mat == null)
            {
                string message = String.Format("有沒放材質球喔", i);
                return message;
            }
            
            
        }

        return "";
    }
}
