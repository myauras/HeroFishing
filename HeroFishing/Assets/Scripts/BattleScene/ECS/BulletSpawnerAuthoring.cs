using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace HeroFishing.Battle {
    public class BulletSpawnerAuthoring : MonoBehaviour {

        public static BulletSpawner MyBulletSpawner;

        [Serializable]
        public struct SpellPrefab {

            public int SpellMapID;
            public GameObject ProjectilePrefab;
            public GameObject HitPrefab;
            public GameObject FirePrefab;
        }

        [SerializeField] private SpellPrefab[] SpellPrefabs;

        class Baker : Baker<BulletSpawnerAuthoring> {
            public override void Bake(BulletSpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);

                NativeHashMap<int, SpellEntities> tmpHashMap = new NativeHashMap<int, SpellEntities>();

                foreach (var spellPrefab in authoring.SpellPrefabs) {
                    tmpHashMap.Add(spellPrefab.SpellMapID, new SpellEntities {
                        ProjectileEntity = GetEntity(spellPrefab.ProjectilePrefab, TransformUsageFlags.Dynamic),
                        HitEntity = GetEntity(spellPrefab.HitPrefab, TransformUsageFlags.Dynamic),
                        FireEntity = GetEntity(spellPrefab.FirePrefab, TransformUsageFlags.Dynamic)
                    });
                }
                MyBulletSpawner = new BulletSpawner {
                    SpellEntitieMap = tmpHashMap
                };
                AddComponentObject(entity, MyBulletSpawner);

                tmpHashMap.Dispose();
            }
        }

    }
    public struct SpellEntities {
        public Entity ProjectileEntity;
        public Entity HitEntity;
        public Entity FireEntity;
    }
    public class BulletSpawner : IComponentData, IDisposable {
        public NativeHashMap<int, SpellEntities> SpellEntitieMap;

        public void Dispose() {
            SpellEntitieMap.Dispose();
        }
    }
}