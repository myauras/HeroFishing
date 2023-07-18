using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battlefield {
    // An authoring component is just a normal MonoBehavior.
    public class RoleAuthoring : MonoBehaviour {
        public int Index = 0;

        // In baking, this Baker will run once for every RotationSpeedAuthoring instance in an entity subscene.
        // (Nesting an authoring component's Baker class is simply an optional matter of style.)
        class Baker : Baker<RoleAuthoring> {
            public override void Bake(RoleAuthoring authoring) {
                // The entity will be moved
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RoleValue {
                    Index = authoring.Index
                });
            }
        }
    }

    public struct RoleValue : IComponentData {
        public int Index;
    }
}