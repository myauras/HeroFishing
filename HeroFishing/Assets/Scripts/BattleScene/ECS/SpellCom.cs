using HeroFishing.Battle;
using Unity.Entities;
using Unity.Mathematics;

public struct SpellCom : IComponentData {
    public int PlayerID;//玩家ID
    public int BulletPrefabID;//子彈Prefab名稱
    public float3 AttackerPos;//攻擊者位置
    public float3 TargetPos;//目標位置
    public float3 Direction;//攻擊方向向量
    public float Speed;//子彈速度
    public float Radius;//子彈碰撞半徑


}