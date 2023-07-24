using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace HeroFishing.Battle {
    public class MonsterSpawnerAuthoring : MonoBehaviour {

        // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
        // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
        // is simply an optional matter of style.)
        class Baker : Baker<MonsterSpawnerAuthoring> {
            public override void Bake(MonsterSpawnerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new MonsterSpawner {
                });
            }
        }

    }

    struct MonsterSpawner : IComponentData {
    }
}