using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class FX_RampCreate : EditorWindow
{
    Color[] colorall;
    bool isAlpha;
    public string tex2dName = "FX_Ramp";
    public Texture tex;
    public Gradient gradient = new Gradient();
    public ParticleSystem  particle ;
    public TrailRenderer trail;
    public LineRenderer line;
    public string path = ">>還未選擇保存位置";
    public string texname = "FX_Ramp";
    private int valueRampsource = 4;
    public int serial = 1;
    public Vector2 resolution = new Vector2(256, 8);
    public float[] gaodus;
    [MenuItem("FX/漸層圖")]
    static void RampCreateWindow ()
    {
        FX_RampCreate window = EditorWindow.GetWindow<FX_RampCreate>();
        window.minSize = new Vector2(350, 600);
        window.titleContent = new GUIContent("漸層圖生成工具");
        window.Show();
    }
    void SetPath1()
    {
        path = EditorUtility.OpenFolderPanel("", "", "");
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("創建新的漸層Gradient");
        gradient = EditorGUILayout.GradientField("Gradient", gradient);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("隨後設置大小");
        resolution = EditorGUILayout.Vector2Field(GUIContent.none, resolution);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("設為  256*8"))
            resolution = new Vector2(256, 8);
        if (GUILayout.Button("設為  512*8"))
            resolution = new Vector2(512, 8);
        EditorGUILayout.Space();
        isAlpha = EditorGUILayout.ToggleLeft("<<勾選 以導出包含Alpha通道 ( • ̀ω•́ )", isAlpha);
        GUIStyle style = new GUIStyle("textfield");
        tex2dName = EditorGUILayout.TextField("輸出文件命名：", tex2dName, style);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("當前保存位置：");
        EditorGUILayout.LabelField(path);
        bool ispath = GUILayout.Button("--選擇保存位置--");
        if (ispath)
        {
            SetPath1();
            Debug.Log("你的保存位置："+ path);
        }
        bool shengcheng = GUILayout.Button("✧◝(生成漸變圖)◜✧");
        if (shengcheng)
        {
            OutRampTex();
        }
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("---------------------------------------------------------");
        if (GUILayout.Button("打開資料夾"))
        {
            Application.OpenURL("file://" + path);
        }
    }
    void OutRampTex()
    {
        colorall = new Color[(int)(resolution.x* resolution.y)];
        if (isAlpha == false)
        {
            gaodus = new float[(int)resolution.y];
            gaodus[0] = 0;
            float gao = 0;
            for (int g = 0; g < resolution.y; g++)
            {
                if (g == 0)
                {
                }
                else
                {
                    gao += resolution.x;
                    gaodus[g] = gao;
                }
            }
            for (int a = 0; a < resolution.y; a++)
            {
                for (int c = 0; c < resolution.x; c++)
                {
                    float temp = c / resolution.x;
                    colorall[(int)gaodus[a] + c] = gradient.Evaluate(temp);
                }
            }
        }
        else
        {
            gaodus = new float[(int)resolution.y];
            gaodus[0] = 0;
            float gao = 0;
            for (int g = 0; g < resolution.y; g++)
            {
                if (g == 0)
                {
                }
                else
                {
                    gao += resolution.x;
                    gaodus[g] = gao;
                }
            }
            for (int a = 0; a < resolution.y; a++)
            {
                for (int c = 0; c < resolution.x; c++)
                {
                    float temp = c / resolution.x;
                    colorall[(int)gaodus[a] + c] = gradient.Evaluate(temp);
                    colorall[(int)gaodus[a] + c].a = gradient.Evaluate(temp).a;
                }
            }
        }
        Save(colorall);
        Debug.Log("Ramp圖片,"+"名稱："+ tex2dName+",保存位置："+ path);
    }
    void Save(Color[] colors)
    {
        TextureFormat _texFormat;
        if (isAlpha)
        {
            _texFormat = TextureFormat.ARGB32;
        }
        else
        {
            _texFormat = TextureFormat.RGB24;
        }
        Texture2D tex = new Texture2D((int)resolution.x, (int)resolution.y, _texFormat, false);
        tex.SetPixels(colors);
        tex.Apply();
        byte[] bytes;
        bytes = tex.EncodeToPNG();
        string sname = tex2dName + "_" + serial;
        serial += 1;
            File.WriteAllBytes(path + "/" + sname + ".png", bytes);
        AssetDatabase.Refresh();
    }
}
