//using HeroFishing.Socket;
//using Unity.Entities;
//using UnityEngine;

//namespace HeroFishing.Battle {
//    public class ExecuteAuthoring : MonoBehaviour {
//        [SerializeField] bool EnableMonsterSpawnSys;
//        [SerializeField] bool EnableBulletSpawnSys;
//        [SerializeField] bool EnableCollisionSys;
//        [SerializeField] bool LocalTest;
//        [SerializeField] float LocalDeadThreshold;
//        //[SerializeField] bool EnableCollisionSys;

//        class Baker : Baker<ExecuteAuthoring> {
//            public override void Bake(ExecuteAuthoring authoring) {
//                var entity = GetEntity(TransformUsageFlags.None);


//                //if (authoring.EnableCollisionSys) AddComponent<CollisionSys>(entity);
//                if (authoring.EnableMonsterSpawnSys) AddComponent<MonsterSpawnSys>(entity);
//                if (authoring.EnableBulletSpawnSys) AddComponent<BulletSpawnSys>(entity);
//                if (authoring.EnableCollisionSys) AddComponent<CollisionSys>(entity);
//                if (authoring.LocalTest /*|| !GameConnector.Connected*/) AddComponent(entity,
//                    new LocalTestSys { DeadThreshold = authoring.LocalDeadThreshold });
//            }
//        }
//    }
//    public struct MonsterSpawnSys : IComponentData {
//    }
//    public struct BulletSpawnSys : IComponentData {
//    }
//    public struct CollisionSys : IComponentData {
//    }
//    public struct LocalTestSys : IComponentData {
//        public float DeadThreshold;
//    }
//    //public struct CollisionSys : IComponentData {
//    //}

//}
