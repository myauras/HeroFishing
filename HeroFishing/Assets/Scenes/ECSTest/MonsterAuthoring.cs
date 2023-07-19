
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battlefield {
    public class MonsterAuthoring : MonoBehaviour {

        class Baker : Baker<MonsterAuthoring> {
            public override void Bake(MonsterAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MonsterValue {
                });
            }
        }
    }

    public struct MonsterValue : IComponentData {
    }
}

