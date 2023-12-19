using Unity.Entities;
using UnityEngine;

namespace HeroFishing.Battle {
    public class ExecuteAuthoring : MonoBehaviour {
        [SerializeField] bool EnableMonsterSpawnSys;
        [SerializeField] bool EnableBulletSpawnSys;
        [SerializeField] bool EnableCollisionSys;
        [SerializeField] bool LocalDead;
        //[SerializeField] bool EnableCollisionSys;

        class Baker : Baker<ExecuteAuthoring> {
            public override void Bake(ExecuteAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.None);

                //if (authoring.EnableCollisionSys) AddComponent<CollisionSys>(entity);
                if (authoring.EnableMonsterSpawnSys) AddComponent<MonsterSpawnSys>(entity);
                if (authoring.EnableBulletSpawnSys) AddComponent<BulletSpawnSys>(entity);
                if (authoring.EnableCollisionSys) AddComponent<CollisionSys>(entity);
                if (authoring.LocalDead) AddComponent<LocalDeadSys>(entity);
            }
        }
    }
    public struct MonsterSpawnSys : IComponentData {
    }
    public struct BulletSpawnSys : IComponentData {
    }
    public struct CollisionSys : IComponentData {
    }
    public struct LocalDeadSys : IComponentData {
    }
    //public struct CollisionSys : IComponentData {
    //}

}
