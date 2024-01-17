using Scoz.Func;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;

public class ECSStrManager {
    static NativeHashMap<uint, NativeArray<char>> HashToCharArrayMap = new NativeHashMap<uint, NativeArray<char>>(10, Allocator.Persistent);

    public static uint AddStr(string str) {
        uint hash = HashUtility.GenerateHash(str);
        // 重複利用一樣的字串
        if (HashToCharArrayMap.ContainsKey(hash))
            return hash;
        var nativeArray = HashUtility.StrToNativeArray(str);
        NativeArray<char> persistentNativeStr = new NativeArray<char>(str.Length, Allocator.Persistent);
        nativeArray.CopyTo(persistentNativeStr);
        HashToCharArrayMap.TryAdd(hash, persistentNativeStr);
        nativeArray.Dispose();
        return hash;
    }

    [BurstCompile]
    public static void GetStr(uint _idx, out NativeArray<char> _out) {

        if (HashToCharArrayMap.TryGetValue(_idx, out NativeArray<char> result)) {
            NativeArray<char> tmpNativeArray = new NativeArray<char>(result.Length, Allocator.Temp);
            for (int i = 0; i < result.Length; i++) {
                tmpNativeArray[i] = result[i];
            }
            _out = tmpNativeArray;
            return;
        }
        _out = new NativeArray<char>(0, Allocator.Temp);
    }
    public static string GetStr(uint _idx) {
        NativeList<char> tmpNativeList = new NativeList<char>(Allocator.TempJob);
        if (HashToCharArrayMap.TryGetValue(_idx, out NativeArray<char> result)) {
            for (int i = 0; i < result.Length; i++) {
                tmpNativeList.Add(result[i]);
            }
        }
        var tmpNativeArray = tmpNativeList.ToArray(Allocator.TempJob);
        string str = new string(tmpNativeArray);
        tmpNativeArray.Dispose();
        tmpNativeList.Dispose();
        return str;
    }


}
