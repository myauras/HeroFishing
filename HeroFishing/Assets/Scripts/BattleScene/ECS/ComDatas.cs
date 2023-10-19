using HeroFishing.Battle;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct BulletValue : IComponentData {
    public float Speed;
    public float Radius;
    public float3 Position;
    public float3 Direction;
    public uint StrIndex_SpellID;//紀錄子彈的技能ID
    public int SpellPrefabID;//技能Prefab名稱
}
/// <summary>
/// 怪物參照元件，用於參照GameObject實例用
/// </summary>
public class MonsterInstance : IComponentData, IDisposable {
    public GameObject GO;
    public Transform Trans;
    public Monster MyMonster;
    public Vector3 Dir;
    public void Dispose() {
        UnityEngine.Object.DestroyImmediate(GO);
    }
}
/// <summary>
/// 怪物資料元件
/// </summary>
public struct MonsterValue : IComponentData {
    public int MonsterID;
    public Entity MyEntity;//把自己的Enity記錄起來，之後取的時候較快
    public float Radius;
    public float3 Pos;
    public bool InField;//是否進入戰場，進入之後會改為true，並在怪物離開區域後會將InField為true的怪物移除
}
/// <summary>
/// 擊中標籤元件
/// </summary>
public struct MonsterHitTag : IComponentData {
    public int MonsterID;//受擊怪物ID 
    public uint StrIndex_SpellID;//ECSStrManager的技能ID字串索引
}
/// <summary>
/// 死亡標籤元件
/// </summary>
public struct MonsterDieTag : IComponentData { }
public struct AutoDestroyTag : IComponentData {
    public float LifeTime;//生命週期
    public float ExistTime;//目前存活秒數(預設為0)
}
/// <summary>
/// 特效元件
/// </summary>
public struct ParticleSpawnTag : IComponentData {
    public uint StrIndex_ParticlePath;//ECSStrManager的特效位置字串索引
    public float3 Pos;
    public float4 Rot;
}
/// <summary>
/// 技能特效元件
/// </summary>
public struct HitParticleSpawnTag : IComponentData {
    public int SpellPrefabID;
    public MonsterValue Monster;
    public BulletValue Bullet;
}