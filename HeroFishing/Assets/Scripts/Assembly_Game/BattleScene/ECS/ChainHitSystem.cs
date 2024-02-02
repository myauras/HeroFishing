//using HeroFishing.Battle;
//using Scoz.Func;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.UIElements;

//[BurstCompile]
//[CreateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
//public partial struct ChainHitSystem : ISystem {
//    EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
//    [ReadOnly] private BufferLookup<MonsterBuffer> monsterLookup;

//    [BurstCompile]
//    public void OnCreate(ref SystemState state) {
//        state.RequireForUpdate<ChainHitData>();
//        monsterLookup = state.GetBufferLookup<MonsterBuffer>();
//        ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
//    }

//    [BurstCompile]
//    public void OnDestroy(ref SystemState state) {

//    }


//    [BurstCompile]
//    public void OnUpdate(ref SystemState state) {

//        var writer = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
//        var ecb = new EntityCommandBuffer(Allocator.Temp);

//        monsterLookup.Update(ref state);
//        //尋找目標怪物
//        foreach (var (hitData, entity) in SystemAPI.Query<RefRO<ChainHitData>>().WithEntityAccess()) {

//            if (!monsterLookup.HasBuffer(entity))
//                ecb.AddBuffer<MonsterBuffer>(entity);

//            foreach (var monster in SystemAPI.Query<MonsterValue>().WithAbsent<AutoDestroyTag>()) {
//                // 檢查是否有怪物，是否是最初擊中的怪物，以及怪物是否已經進場
//                if (monster.MyEntity == Entity.Null || hitData.ValueRO.OnHitMonster.MyEntity == monster.MyEntity || !monster.InField) continue;

//                // 檢查怪物角度是否小於等於設定值
//                float3 monsterDir = monster.Pos - hitData.ValueRO.OnHitMonster.Pos;
//                float angle = MathUtility.angle(monsterDir, hitData.ValueRO.HitDirection);
//                if (angle > hitData.ValueRO.Angle / 2) continue;

//                // 尋找距離接近的怪物，加入dynamic buffer
//                float distance = math.distancesq(hitData.ValueRO.OnHitMonster.Pos, monster.Pos);
//                var radius = monster.Radius + hitData.ValueRO.TriggerRange;
//                if (distance < radius * radius) {
//                    ecb.AppendToBuffer(entity, new MonsterBuffer { Monster = monster, Distance = distance });
//                }

//            }
//        }

//        // 執行Command Buffer，填充Monster們
//        ecb.Playback(state.EntityManager);
//        // 因為SystemState有所改變，Lookup要Update
//        monsterLookup.Update(ref state);

//        foreach (var (hitData, entity) in SystemAPI.Query<ChainHitData>().WithEntityAccess()) {
//            if (monsterLookup.TryGetBuffer(entity, out var buffer)) {
//                // 將buffer重新排列，並且縮減為需要的size
//                var monsterArray = buffer.AsNativeArray();
//                monsterArray.Sort();
//                if (buffer.Length > hitData.MaxChainCount)
//                    buffer.Resize(hitData.MaxChainCount, NativeArrayOptions.UninitializedMemory);

//                // 將buffer的變成Chain Hit
//                for (int i = 0; i < buffer.Length; i++) {
//                    var monsterBuffer = buffer[i];

//                    var spellEntity = state.EntityManager.CreateEntity();
//                    var direction = monsterBuffer.Monster.Pos - hitData.OnHitMonster.Pos;
//                    var position = hitData.HitPosition;
//                    var spawnData = new SpellSpawnData {
//                        AttackID = hitData.AttackID,
//                        SpellPrefabID = hitData.SpellPrefabID,
//                        SubSpellPrefabID = hitData.SubSpellPrefabID,
//                        InitPosition = position,
//                        InitDirection = direction,
//                        IgnoreFireModel = true
//                    };

//                    writer.AddComponent(0, spellEntity, new SpellBulletData {
//                        HeroIndex = hitData.HeroIndex,
//                        SpawnData = spawnData,
//                        DestroyOnCollision = true,
//                        StrIndex_SpellID = hitData.StrIndex_SpellID,
//                        Speed = hitData.Speed,
//                        LifeTime = hitData.LifeTime,
//                        Radius = hitData.Radius,
//                        IsSub = true,
//                        TargetMonster = monsterBuffer.Monster
//                    });
//                }
//            }
//            writer.DestroyEntity(1, entity);
//        }
//        ecb.Dispose();
//    }
//}
