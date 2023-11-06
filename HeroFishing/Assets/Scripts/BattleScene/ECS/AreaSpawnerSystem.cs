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
                WriteLog.LogErrorFormat("Area {0}���W�S����Bullet Component", areaPrefab.name);
                continue;
            }

            //float3 direction = spellCom.Direction;
            //quaternion bulletQuaternion = quaternion.LookRotation(spellCom.Direction, math.up());
            //�]�w�l�uGameobject��Transfrom
            areaGO.transform.localPosition = spellCom.AreaPos;
            //areaGO.transform.localRotation = bulletQuaternion;

            //�إ�Entity
            var entity = state.EntityManager.CreateEntity();
            //�]�w�l�u�ҫ�
            area.SetData(spellCom.SpellPrefabID);
            //�[�JBulletValue
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
            //�[�JBulletInstance
            ECB.AddComponent(entity, new AreaInstance {
                Trans = areaGO.transform,
                MyArea = area,
            });
            //�[�J�۰ʾP��Tag
            ECB.AddComponent(entity, new AutoDestroyTag {
                LifeTime = spellCom.LifeTime,
                ExistTime = 0,
            });

            //�����I�k
            ECB.DestroyEntity(spellEntity);
        }
    }
}
