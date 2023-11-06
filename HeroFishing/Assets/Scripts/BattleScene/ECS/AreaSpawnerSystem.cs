using HeroFishing.Battle;
using Scoz.Func;
using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AreaInstance : IComponentData, IDisposable {
    public Transform Trans;
    public Area MyArea;
    public void Dispose() {
        UnityEngine.Object.Destroy(Trans.gameObject);
    }
}

[BurstCompile]
public partial struct AreaSpawnerSystem : ISystem {
    EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BulletSpawnSys>();
        state.RequireForUpdate<AreaCom>();
        ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (spellCom, spellEntity) in SystemAPI.Query<AreaCom>().WithEntityAccess()) {
            var areaPrefab = ResourcePreSetter.Instance.AreaPrefab;
            if (areaPrefab == null) continue;
            var areaGO = GameObject.Instantiate(areaPrefab.gameObject);
#if UNITY_EDITOR
            areaGO.name = "Area" + spellCom.SpellPrefabID;
            //bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#else
bulletGO.hideFlags |= HideFlags.HideAndDontSave;
#endif
            var area = areaGO.GetComponent<Area>();
            if (area == null) {
                WriteLog.LogErrorFormat("Area {0}身上沒有掛Bullet Component", areaPrefab.name);
                continue;
            }

            //float3 direction = spellCom.Direction;
            //quaternion bulletQuaternion = quaternion.LookRotation(spellCom.Direction, math.up());
            //設定子彈Gameobject的Transfrom
            areaGO.transform.localPosition = spellCom.AreaPos;
            //areaGO.transform.localRotation = bulletQuaternion;

            //建立Entity
            var entity = state.EntityManager.CreateEntity();
            //設定子彈模型
            area.SetData(spellCom.SpellPrefabID);
            //加入BulletValue
            ECB.AddComponent(entity, new AreaValue() {
                //Position = spellCom.AttackerPos,
                //Speed = spellCom.Speed,
                //Radius = spellCom.Radius,
                //Direction = direction,
                //StrIndex_SpellID = spellCom.StrIndex_SpellID,
                //SpellPrefabID = spellCom.SpellPrefabID,
                //Piercing = spellCom.Piercing,
                //MaxPiercingCount = spellCom.MaxPiercingCount,
                //PiercingCount = 0,
            });
            //加入BulletInstance
            ECB.AddComponent(entity, new AreaInstance {
                Trans = areaGO.transform,
                MyArea = area,
            });
            //加入自動銷毀Tag
            ECB.AddComponent(entity, new AutoDestroyTag {
                LifeTime = spellCom.LifeTime,
                ExistTime = 0,
            });

            //移除施法
            ECB.DestroyEntity(spellEntity);
        }
    }
}
