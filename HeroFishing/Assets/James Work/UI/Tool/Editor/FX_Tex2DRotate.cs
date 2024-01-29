using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FX_Tex2DRotate : Editor
{
    static int isgeshi(string path) 
    {
        string[] split = path.Split('.');
        int size = split.Length -1;
        int zhenjia = 0;
        if (split[size] == "jpg" || split[size] == "JPG")
        {
            zhenjia = 0;
        }
        else if (split[size] == "png" || split[size] == "PNG")
        {
            zhenjia = 1;
        }
        else if (split[size] == "TGA" || split[size] == "tga")
        {
            zhenjia = 2;
        }
        else
        {
            zhenjia = 1;//如果沒有合適的就輸出Png
        }
        return zhenjia;
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/順時針90度", true)]
    private static bool Zjs()
    {
        if (Selection.objects[0] as Texture2D || Selection.objects.Length > 1)
            return true;
        else return false;
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/順時針90度")]
    static void Zhengjiushi()
    {
        Texture2D yuantu = Selection.objects[0] as Texture2D;
        string path1 = AssetDatabase.GetAssetPath(yuantu);
        TextureImporter tex = TextureImporter.GetAtPath(path1) as TextureImporter;
        bool dx = false;
        int geshi;
        geshi = isgeshi(path1);
        if (tex.isReadable == false)
        {
            tex.isReadable = true;
            AssetDatabase.ImportAsset(path1);
            dx = true;
        }
        Color[] endcolor = new Color[(int)(yuantu.height * yuantu.width)];
        Texture2D rotateTex = new Texture2D(yuantu.height, yuantu.width, TextureFormat.RGBAFloat, false);
        int cishu = 0;
        for (int i = 0; i < yuantu.width; i++)
        {
            for (int j = 0; j < yuantu.height; j++)
            {
                endcolor[cishu] = yuantu.GetPixel((yuantu.width - i) - 1, j);
                cishu += 1;
            }
        }
        rotateTex.SetPixels(endcolor);
        rotateTex.Apply();
        Save(endcolor, new Vector2(rotateTex.width, rotateTex.height), yuantu.name, path1, tex,geshi);
        if (dx)
        {
            tex.isReadable = false;
            AssetDatabase.ImportAsset(path1);
        }
        AssetDatabase.ImportAsset(path1);
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/逆時針90度", true)]
    private static bool fujs()
    {
        if (Selection.objects[0] as Texture2D || Selection.objects.Length > 1)
            return true;
        else return false;
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/逆時針90度")]
    static void Fujiushi()
    {
        Texture2D yuantu = Selection.objects[0] as Texture2D;
        string path1 = AssetDatabase.GetAssetPath(yuantu);
        TextureImporter tex = TextureImporter.GetAtPath(path1) as TextureImporter;
        bool dx = false;
        int geshi;
        geshi = isgeshi(path1);
        if (tex.isReadable == false)
        {
            tex.isReadable = true;
            AssetDatabase.ImportAsset(path1);
            dx = true;
        }
        Color[] endcolor = new Color[(int)(yuantu.height * yuantu.width)];
        Texture2D rotateTex = new Texture2D(yuantu.height, yuantu.width, TextureFormat.RGBAFloat, false);
        int cishu = 0;
        for (int i = 0; i < yuantu.width; i++)
        {
            for (int j = 0; j < yuantu.height; j++)
            {
                endcolor[cishu] = yuantu.GetPixel(i, (yuantu.height - j) - 1);
                cishu += 1;
            }
        }
        rotateTex.SetPixels(endcolor);
        rotateTex.Apply();
        Save(endcolor, new Vector2(rotateTex.width, rotateTex.height), yuantu.name, path1, tex, geshi);
        if (dx)
        {
            tex.isReadable = false;
            AssetDatabase.ImportAsset(path1);
        }
        AssetDatabase.ImportAsset(path1);
    }

    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/180度", true)]
    private static bool yibaiba()
    {
        if (Selection.objects[0] as Texture2D || Selection.objects.Length > 1)
            return true;
        else return false;
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/180度")]
    static void YiBaiBashi()
    {
        Texture2D yuantu = Selection.objects[0] as Texture2D;
        string path1 = AssetDatabase.GetAssetPath(yuantu);
        TextureImporter tex = TextureImporter.GetAtPath(path1) as TextureImporter;
        bool dx = false;
        int geshi;
        geshi = isgeshi(path1);
        if (tex.isReadable == false)
        {
            tex.isReadable = true;
            AssetDatabase.ImportAsset(path1);
            dx = true;
        }
        Color[] endcolor = new Color[(int)(yuantu.height * yuantu.width)];
        Color[] lowcolor = new Color[(int)(yuantu.height * yuantu.width)];
        lowcolor = yuantu.GetPixels();
        int maxcishu = yuantu.height * yuantu.width;
        maxcishu -= 1;
        Texture2D rotateTex = new Texture2D(yuantu.width, yuantu.height, TextureFormat.RGBAFloat, false);
        int cishu = 0;
        for (int i = 0; i < yuantu.width; i++)
        {
            for (int j = 0; j < yuantu.height; j++)
            {
                endcolor[cishu] = lowcolor[maxcishu];
                cishu += 1;
                maxcishu -= 1;
            }
        }
        rotateTex.SetPixels(endcolor);
        rotateTex.Apply();
        Save(endcolor, new Vector2(rotateTex.width, rotateTex.height), yuantu.name, path1, tex, geshi);
        if (dx)
        {
            tex.isReadable = false;
            AssetDatabase.ImportAsset(path1);
        }
        AssetDatabase.ImportAsset(path1);
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/橫向翻轉", true)]
    private static bool shuipingfan()
    {
        if (Selection.objects[0] as Texture2D || Selection.objects.Length > 1)
            return true;
        else return false;
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/橫向翻轉")]
    static void ShuiPing()
    {
        Texture2D yuantu = Selection.objects[0] as Texture2D;
        string path1 = AssetDatabase.GetAssetPath(yuantu);
        TextureImporter tex = TextureImporter.GetAtPath(path1) as TextureImporter;
        bool dx = false;
        int geshi;
        geshi = isgeshi(path1);
        if (tex.isReadable == false)
        {
            tex.isReadable = true;
            AssetDatabase.ImportAsset(path1);
            dx = true;
        }
        Color[] endcolor = new Color[(int)(yuantu.width * yuantu.height)];
        Texture2D rotateTex = new Texture2D(yuantu.width, yuantu.height, TextureFormat.RGBAFloat, false);
        int cishu = 0;
        for (int j = 0; j < yuantu.height; j++)
        {
            for (int i = 0; i < yuantu.width; i++)
            {
                endcolor[cishu] = yuantu.GetPixel((yuantu.width-i)-1, j);
                cishu += 1;
            }
        }
        rotateTex.SetPixels(endcolor);
        rotateTex.Apply();
        Save(endcolor, new Vector2(rotateTex.width, rotateTex.height), yuantu.name, path1, tex, geshi);
        if (dx)
        {
            tex.isReadable = false;
            AssetDatabase.ImportAsset(path1);
        }
        AssetDatabase.ImportAsset(path1);
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/垂直翻轉", true)]
    private static bool su()
    {
        if (Selection.objects[0] as Texture2D || Selection.objects.Length > 1)
            return true;
        else return false;
    }
    [MenuItem("Assets/FX_貼圖工具箱/貼圖旋轉/垂直翻轉")]
    static void ZhongXiang()
    {
        Texture2D yuantu = Selection.objects[0] as Texture2D;
        string path1 = AssetDatabase.GetAssetPath(yuantu);
        TextureImporter tex = TextureImporter.GetAtPath(path1) as TextureImporter;
        bool dx = false;
        int geshi;
        geshi = isgeshi(path1);
        if (tex.isReadable == false)
        {
            tex.isReadable = true;
            AssetDatabase.ImportAsset(path1);
            dx = true;
        }
        Color[] endcolor = new Color[(int)(yuantu.width * yuantu.height)];
        Texture2D rotateTex = new Texture2D(yuantu.width, yuantu.height, TextureFormat.RGBAFloat, false);
        int cishu = 0;
        for (int j = 0; j < yuantu.height; j++)
        {
            for (int i = 0; i < yuantu.width; i++)
            {
                endcolor[cishu] = yuantu.GetPixel(i, yuantu.height-j-1);
                cishu += 1;
            }
        }
        rotateTex.SetPixels(endcolor);
        rotateTex.Apply();
        Save(endcolor, new Vector2(rotateTex.width, rotateTex.height), yuantu.name, path1, tex, geshi);
        if (dx)
        {
            tex.isReadable = false;
            AssetDatabase.ImportAsset(path1);
        }
        AssetDatabase.ImportAsset(path1);
    }
    static void Save(Color[] colors, Vector2 kuangao, string name1, string path1, TextureImporter ttt ,int geshi)
    {
        TextureFormat texformat;
        if (geshi == 0)
        {
            texformat = TextureFormat.RGB24;
        }
        else if (geshi == 1)
        {
            texformat = TextureFormat.RGBAFloat;
        }
        else if (geshi == 2)
        {
            texformat = TextureFormat.RGBAFloat;
        }
        else
        {
            texformat = TextureFormat.RGBAFloat;
        }
        Texture2D tex = new Texture2D((int)kuangao.x, (int)kuangao.y, texformat, false);
        tex.SetPixels(colors);
        tex.Apply();
        byte[] bytes;
        string houzhui ;
        if (geshi == 0) 
        {
            bytes = tex.EncodeToJPG(100);
            houzhui = ".jpg";
        }
        else if (geshi == 1) 
        {
            bytes = tex.EncodeToPNG();
            houzhui = ".png";
        }
        else if (geshi == 2)
        {
            bytes = tex.EncodeToTGA();
            houzhui = ".tga";
        }
        else 
        {
            bytes = tex.EncodeToPNG();
            houzhui = ".png";
        }
        string temp1 = Jueduilujing();
        string temp2 = Jueduilujing1(path1);
        File.WriteAllBytes((temp1 + temp2) + "/" + name1 + houzhui, bytes);
        AssetDatabase.Refresh();
        Debug.Log(string.Format("<color=#4CFFB3>{0}</color>", "成功了！We did it！" + "  本次輸出新文件：" + name1 + houzhui));
        AssetDatabase.Refresh();
    }
    static string Jueduilujing()
    {
        string patht = Application.dataPath;
        string[] split = patht.Split('/');
        string[] cc = new string[split.Length];
        string newpath = "";
        for (int i = 0; i < split.Length; i++)
        {
            cc[i] = split[i];
            if (i != split.Length - 1)
            {
                if (i != 0)
                {
                    newpath = newpath + "/" + cc[i];
                }
                if (i == 0)
                {
                    newpath = newpath + cc[i];
                }
            }
        }
        newpath = newpath + "/";
        return newpath;
    }
    static string Jueduilujing1(string patht)
    {
        string[] split = patht.Split('/');
        string[] cc = new string[split.Length];
        string newpath = "";
        for (int i = 0; i < split.Length; i++)
        {
            cc[i] = split[i];
            if (i != split.Length - 1)
            {
                if (i != 0)
                {
                    newpath = newpath + "/" + cc[i];
                }
                if (i == 0)
                {
                    newpath = "" + cc[i];
                }
            }
        }
        return newpath;
    }
}
