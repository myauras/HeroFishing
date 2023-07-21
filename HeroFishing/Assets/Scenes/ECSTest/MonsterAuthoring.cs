
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battlefield {
    public class MonsterAuthoring : MonoBehaviour {

        [SerializeField] float Radius;

        class Baker : Baker<MonsterAuthoring> {
            public override void Bake(MonsterAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MonsterValue {
                    Radius = authoring.Radius,
                });
            }
        }
    }

    public struct MonsterValue : IComponentData {
        public float Radius;
    }
}

