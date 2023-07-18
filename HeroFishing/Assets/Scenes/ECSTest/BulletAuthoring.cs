
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battlefield {
    // An authoring component is just a normal MonoBehavior.
    public class BulletAuthoring : MonoBehaviour {
        public float Scale = 0.3f;

        // In baking, this Baker will run once for every RotationSpeedAuthoring instance in an entity subscene.
        // (Nesting an authoring component's Baker class is simply an optional matter of style.)
        class Baker : Baker<BulletAuthoring> {
            public override void Bake(BulletAuthoring authoring) {
                // The entity will be moved
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BulletValue {
                    Scale = authoring.Scale
                });
            }
        }
    }

    public struct BulletValue : IComponentData {
        public float Scale;
        public float Angle;
        public float Speed;
    }
}

