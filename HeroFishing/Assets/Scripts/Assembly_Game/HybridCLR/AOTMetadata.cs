using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ※這裡遊戲中會透過反射來取資料所以不要追加命名空間或更名, 否則反射會抓不到
public static class AOTMetadata {
	public static string Version { get; private set; } = "0.9.25";
    public static List<string> AotDllList = new List<string> {};
}
