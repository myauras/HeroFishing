using Unity.Entities;
using UnityEngine;

namespace HeroFishing.Battle {
    public class ExecuteAuthoring : MonoBehaviour {
        [SerializeField] bool EnableMonsterSpawnSys;
        [SerializeField] bool EnableCollisionSys;

        class Baker : Baker<ExecuteAuthoring> {
            public override void Bake(ExecuteAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);

                if (authoring.EnableCollisionSys) AddComponent<CollisionSys>(entity);
                if (authoring.EnableMonsterSpawnSys) AddComponent<MonsterSpawnSys>(entity);
            }
        }
    }
    public struct MonsterSpawnSys : IComponentData {
    }
    public struct CollisionSys : IComponentData {
    }

}
