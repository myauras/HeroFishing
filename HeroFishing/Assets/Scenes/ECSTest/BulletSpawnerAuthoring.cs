using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace HeroFishing.Battlefield {
    public class BulletSpawnerAuthoring : MonoBehaviour {
        public GameObject Prefab;

        // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
        // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
        // is simply an optional matter of style.)
        class Baker : Baker<BulletSpawnerAuthoring> {
            public override void Bake(BulletSpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Spawner {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    struct Spawner : IComponentData {
        public Entity Prefab;
    }
}