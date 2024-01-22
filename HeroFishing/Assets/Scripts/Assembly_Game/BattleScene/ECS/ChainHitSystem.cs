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
//        //�M��ؼЩǪ�
//        foreach (var (hitData, entity) in SystemAPI.Query<RefRO<ChainHitData>>().WithEntityAccess()) {

//            if (!monsterLookup.HasBuffer(entity))
//                ecb.AddBuffer<MonsterBuffer>(entity);

//            foreach (var monster in SystemAPI.Query<MonsterValue>().WithAbsent<AutoDestroyTag>()) {
//                // �ˬd�O�_���Ǫ��A�O�_�O�̪��������Ǫ��A�H�ΩǪ��O�_�w�g�i��
//                if (monster.MyEntity == Entity.Null || hitData.ValueRO.OnHitMonster.MyEntity == monster.MyEntity || !monster.InField) continue;

//                // �ˬd�Ǫ����׬O�_�p�󵥩�]�w��
//                float3 monsterDir = monster.Pos - hitData.ValueRO.OnHitMonster.Pos;
//                float angle = MathUtility.angle(monsterDir, hitData.ValueRO.HitDirection);
//                if (angle > hitData.ValueRO.Angle / 2) continue;

//                // �M��Z�����񪺩Ǫ��A�[�Jdynamic buffer
//                float distance = math.distancesq(hitData.ValueRO.OnHitMonster.Pos, monster.Pos);
//                var radius = monster.Radius + hitData.ValueRO.TriggerRange;
//                if (distance < radius * radius) {
//                    ecb.AppendToBuffer(entity, new MonsterBuffer { Monster = monster, Distance = distance });
//                }

//            }
//        }

//        // ����Command Buffer�A��RMonster��
//        ecb.Playback(state.EntityManager);
//        // �]��SystemState���ҧ��ܡALookup�nUpdate
//        monsterLookup.Update(ref state);

//        foreach (var (hitData, entity) in SystemAPI.Query<ChainHitData>().WithEntityAccess()) {
//            if (monsterLookup.TryGetBuffer(entity, out var buffer)) {
//                // �Nbuffer���s�ƦC�A�åB�Y��ݭn��size
//                var monsterArray = buffer.AsNativeArray();
//                monsterArray.Sort();
//                if (buffer.Length > hitData.MaxChainCount)
//                    buffer.Resize(hitData.MaxChainCount, NativeArrayOptions.UninitializedMemory);

//                // �Nbuffer���ܦ�Chain Hit
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
