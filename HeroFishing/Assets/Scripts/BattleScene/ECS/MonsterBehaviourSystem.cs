using Scoz.Func;
using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace HeroFishing.Battle {
    public partial struct MonsterBehaviourSystem : ISystem {

        Entity GridEntity; // 網格實體

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

        }
        public void OnDestroy(ref SystemState state) {
            // 銷毀網格
            var gridData = SystemAPI.GetComponent<MapGridData>(GridEntity);
            gridData.GridMap.Dispose();
            state.EntityManager.DestroyEntity(GridEntity);
        }
        public void OnUpdate(ref SystemState state) {
            float deltaTime = SystemAPI.Time.DeltaTime;

            // 清空網格
            var gridData = SystemAPI.GetComponent<MapGridData>(GridEntity);
            gridData.GridMap.Clear();

            //遍歷所有怪物
            foreach (var (monsterValue, monsterInstance) in SystemAPI.Query<RefRW<MonsterValue>, MonsterInstance>().WithAbsent<AutoDestroyTag>()) {

                //怪物移動
                if (monsterInstance.MyMonster.MyData.Speed != 0) {
                    monsterInstance.Trans.localPosition += (monsterInstance.Dir * monsterInstance.MyMonster.MyData.Speed) * deltaTime;
                    monsterValue.ValueRW.Pos = monsterInstance.Trans.localPosition;
                }

                // 將在範圍內的怪物的移動後位置存入網格中
                int2 gridPosition = new int2(
                    (int)(monsterValue.ValueRW.Pos.x / gridData.CellSize),
                    (int)(monsterValue.ValueRW.Pos.z / gridData.CellSize)
                );
                if (PosInGridBoundary(gridData, gridPosition))
                    gridData.GridMap.Add(gridPosition, monsterValue.ValueRW);

            }
        }
        //在網格邊界內才返回true
        bool PosInGridBoundary(MapGridData _mapData, int2 _pos) {
            if (_pos.x < _mapData.BoundaryX.x || _pos.x >= _mapData.BoundaryX.y || _pos.y < _mapData.BoundaryY.x || _pos.y >= _mapData.BoundaryY.y)
                return false;
            return true;
        }
    }

}

