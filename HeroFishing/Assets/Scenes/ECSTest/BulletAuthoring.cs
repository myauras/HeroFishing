
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battlefield {
    // An authoring component is just a normal MonoBehavior.
    public class BulletAuthoring : MonoBehaviour {

        [SerializeField] float Speed;
        [SerializeField] float Radius;

        // In baking, this Baker will run once for every RotationSpeedAuthoring instance in an entity subscene.
        // (Nesting an authoring component's Baker class is simply an optional matter of style.)
        class Baker : Baker<BulletAuthoring> {
            public override void Bake(BulletAuthoring authoring) {
                // The entity will be moved
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BulletValue {
                    Speed = authoring.Speed,
                    Radius = authoring.Radius,
                });
                AddComponent(entity, new AutoDestroyTag {
                    LifeTime = GetComponent<ParticleSystem>().main.duration,
                    ExistTime = 0f,
                });
            }
        }
    }

    public struct BulletValue : IComponentData {
        public float Speed;
        public float Radius;
        public float3 Direction;
    }
}

