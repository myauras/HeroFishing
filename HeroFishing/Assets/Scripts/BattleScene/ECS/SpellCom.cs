using HeroFishing.Battle;
using HeroFishing.Main;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct BulletCom : IComponentData {
    public int PlayerID;//玩家ID
    public uint StrIndex_SpellID;//ECSStringManager的技能ID索引
    public int SpellPrefabID;//子彈Prefab名稱
    public float3 AttackerPos;//攻擊者位置
    public float3 Direction;//攻擊方向向量
    public float Speed;//子彈速度
    public float Radius;//子彈碰撞半徑
    public float LifeTime;//子彈生命週期
    public bool Piercing;//子彈是否貫穿
    public int MaxPiercingCount;//子彈貫穿最大數量
}

public struct AreaCom : IComponentData
{
    public int PlayerID;//玩家ID
    public uint StrIndex_SpellID;//ECSStringManager的技能ID索引
    public int SpellPrefabID;//Prefab名稱
    public AreaValue.ShapeType ShapeType;//範圍形狀，Line或者Circle
    public float2 AreaValues;//範圍的數值，Line情況x為長度，y為寬度; Circle情況x為半徑，y為扇形角度
    public float3 AreaPos;//範圍位置
    public float3 AttackerPos;//攻擊者位置
    public float LifeTime;//生命週期
    public int WaveCount;//攻擊波數
}