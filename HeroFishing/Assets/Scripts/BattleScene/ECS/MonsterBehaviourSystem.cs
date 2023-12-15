using Scoz.Func;
using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battle {
    public partial struct MonsterBehaviourSystem : ISystem {

        Entity GridEntity; // 網格實體
        Entity BoundaryEntity; // 怪物移除邊界實體
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<MonsterInstance>();

            // 創建網格實體並新增網格元件
            GridEntity = state.EntityManager.CreateEntity();
            var gridData = new MapGridData {
                CellSize = 1f,//網格大小是2x2
                GridWidth = 12,//網格寬度為12
                GridHeight = 10,//網格高度為10
                BoundaryX = new int2(-6, 6),//網格索引範圍
                BoundaryY = new int2(-5, 5),//網格索引範圍
                GridMap = new NativeParallelMultiHashMap<int2, MonsterValue>(120, Allocator.Persistent)
            };
            state.EntityManager.AddComponentData(GridEntity, gridData);

            // 創建邊界實體並新增怪物移除邊界元件
            BoundaryEntity = state.EntityManager.CreateEntity();
            var boundaryData = new RemoveMonsterBoundaryData {
                BoundaryX = new int2(-8, 8),//X軸範圍
                BoundaryZ = new int2(-7, 7),//Z軸範圍
            };
            state.EntityManager.AddComponentData(BoundaryEntity, boundaryData);

        }
        public void OnDestroy(ref SystemState state) {
            // 銷毀網格
            var gridData = SystemAPI.GetComponent<MapGridData>(GridEntity);
            gridData.GridMap.Dispose();
            state.EntityManager.DestroyEntity(GridEntity);
        }
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;

            var ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ECB = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            //取得網格資料
            var gridData = SystemAPI.GetComponent<MapGridData>(GridEntity);
            // 清空網格
            gridData.GridMap.Clear();
            //取得邊界資料
            var boundaryData = SystemAPI.GetComponent<RemoveMonsterBoundaryData>(BoundaryEntity);


            //遍歷所有怪物
            foreach (var (monsterValue, monsterInstance, entity) in SystemAPI.Query<RefRW<MonsterValue>, MonsterInstance>()
                .WithAbsent<AutoDestroyTag>().WithEntityAccess()) {

                //怪物移動
                if (monsterInstance.MyMonster.MyData.Speed != 0 && !state.EntityManager.HasComponent<MonsterFreezeTag>(entity)) {
                    monsterInstance.Trans.localPosition += (monsterInstance.Dir * monsterInstance.MyMonster.MyData.Speed) * deltaTime;
                    monsterValue.ValueRW.Pos = monsterInstance.Trans.localPosition;
                }

                //取得怪物移動後的網格位置
                int2 gridPosition = new int2(
                    (int)(monsterValue.ValueRO.Pos.x / gridData.CellSize),
                    (int)(monsterValue.ValueRO.Pos.z / gridData.CellSize)
                );
                // 將在範圍內的怪物存入網格中
                if (PosInGridBoundary(gridData, gridPosition)) {
                    gridData.GridMap.Add(gridPosition, monsterValue.ValueRW);
                    monsterValue.ValueRW.InField = true;
                }
                //將邊界外的怪物加入移除標籤
                if (!PosInRemoveMonsterBoundary(boundaryData, monsterValue.ValueRO.Pos)) {
                    if (monsterValue.ValueRW.InField == true) {
                        //已經進入過區域的怪物離開區域才會被移除
                        monsterValue.ValueRW.InField = false;
                        ECB.AddComponent(monsterValue.ValueRW.MyEntity, new AutoDestroyTag { LifeTime = 1, ExistTime = 0 });
                    }
                }


            }
        }
        //在網格邊界內才返回true
        bool PosInGridBoundary(MapGridData _mapData, int2 _pos) {
            if (_pos.x < _mapData.BoundaryX.x || _pos.x >= _mapData.BoundaryX.y || _pos.y < _mapData.BoundaryY.x || _pos.y >= _mapData.BoundaryY.y)
                return false;
            return true;
        }
        //在怪物移除邊界內才返回true
        bool PosInRemoveMonsterBoundary(RemoveMonsterBoundaryData _boundaryData, float3 _pos) {
            if (_pos.x < _boundaryData.BoundaryX.x || _pos.x > _boundaryData.BoundaryX.y || _pos.z < _boundaryData.BoundaryZ.x || _pos.z > _boundaryData.BoundaryZ.y)
                return false;
            return true;
        }
    }

}

