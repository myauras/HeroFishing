//using HeroFishing.Battle;
//using Unity.Burst;
//using Unity.Entities;

//#if UNITY_EDITOR
//[BurstCompile]
//public partial struct BulletGizmoSystem : ISystem
//{
//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        state.RequireForUpdate<BulletInstance>();
//    }

//    [BurstCompile]
//    public void OnDestroy(ref SystemState state)
//    {

//    }

//    public void OnUpdate(ref SystemState state)
//    {
//        foreach(var (collisionData, moveData, instance) in SystemAPI.Query<BulletCollisionData, MoveData, BulletInstance>()) {
//            var data = new BulletGizmoData() {
//                Position = moveData.Position,
//                Radius = collisionData.Radius
//            };
//            instance.MyBullet.SetGizmoData(data);
//        }

//        foreach(var (areaData, instance) in SystemAPI.Query<AreaCollisionData, BulletInstance>()) {
//            var data = new BulletGizmoData() {
//                Position = areaData.Position,
//                Radius = areaData.Radius,
//                Direction = areaData.Direction,
//                Angle = areaData.Angle
//            };
//            instance.MyBullet.SetGizmoData(data);
//        }
//    }
//}
//#endif
