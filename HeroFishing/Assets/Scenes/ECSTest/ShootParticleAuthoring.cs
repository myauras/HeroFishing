
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battlefield {
    // An authoring component is just a normal MonoBehavior.
    public class ShootParticleAuthoring : MonoBehaviour {

        // In baking, this Baker will run once for every RotationSpeedAuthoring instance in an entity subscene.
        // (Nesting an authoring component's Baker class is simply an optional matter of style.)
        class Baker : Baker<ShootParticleAuthoring> {
            public override void Bake(ShootParticleAuthoring authoring) {
                // The entity will be moved
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootParticleValue {
                });
                AddComponent(entity, new AutoDestroyTag {
                    LifeTime = GetComponent<ParticleSystem>().main.duration,
                    ExistTime = 0f,
                });
            }
        }
    }

    public struct ShootParticleValue : IComponentData {
    }
}

