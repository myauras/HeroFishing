using HeroFishing.Battle;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[CreateAfter(typeof(MonsterBehaviourSystem))]
[UpdateAfter(typeof(MonsterBehaviourSystem))]
[UpdateAfter(typeof(BulletSpawnSystem))]
[BurstCompile]
public partial struct AreaCollisionSystem : ISystem {
    NativeArray<MonsterValue> Monsters;
    //[ReadOnly] EndSimulationEntityCommandBufferSystem.Singleton ECBSingleton;
    [ReadOnly] BufferLookup<HitInfoBuffer> HitInfoLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AreaCollisionData>();
        HitInfoLookup = state.GetBufferLookup<HitInfoBuffer>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecbWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        HitInfoLookup.Update(ref state);

        var monsters = SystemAPI.QueryBuilder().WithAll<MonsterValue>().WithAbsent<AutoDestroyTag>().Build().ToComponentDataArray<MonsterValue>(Allocator.TempJob);
        var jobHandle = new CollisionJob {
            ECB = ecbWriter,
            DeltaTime = SystemAPI.Time.DeltaTime,
            ElapsedTime = SystemAPI.Time.ElapsedTime,
            Monsters = monsters,
            HitInfoLookup = HitInfoLookup,
        }.ScheduleParallel(state.Dependency);

        jobHandle.Complete();
        monsters.Dispose(jobHandle);
    }

    [BurstCompile]
    partial struct CollisionJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public double ElapsedTime;
        [ReadOnly] public NativeArray<MonsterValue> Monsters;
        [ReadOnly] public BufferLookup<HitInfoBuffer> HitInfoLookup;

        public void Execute(ref AreaCollisionData _collisionData, in Entity _entity) {
            _collisionData.Timer += DeltaTime;
            // 時間在delay內，或碰撞時間外就return
            if (_collisionData.Timer < _collisionData.Delay ||
                _collisionData.Timer > _collisionData.CollisionTime + _collisionData.Delay) {
                return;
            }

            foreach (var monster in Monsters) {
                float3 deltaPos = monster.Pos - _collisionData.Position;
                float distancesq = math.lengthsq(deltaPos);
                float radius = _collisionData.Radius + monster.Radius;
                if (distancesq < radius * radius) {
                    // 如果有設定角度，對照角度
                    if (_collisionData.Angle > 0 && !math.all(_collisionData.Direction == float3.zero)) {
                        var angle = MathUtility.angle(deltaPos, _collisionData.Direction);
                        if (angle > _collisionData.Angle / 2) continue;
                    }

                    double checkTime = _collisionData.Waves > 0 ? _collisionData.CollisionTime / _collisionData.Waves : 0.5;

                    // 是否曾經打過此怪物，沒有就新增。
                    if (!HitInfoLookup.TryGetBuffer(_entity, out var buffer)) {
                        ECB.AddBuffer<HitInfoBuffer>(1, _entity)
                            .Add(new HitInfoBuffer { MonsterEntity = monster.MyEntity, HitTime = ElapsedTime, HitCount = 1 });
                    }
                    else {
                        bool alreadyCollided = false;
                        for (int i = 0; i < buffer.Length; i++) {
                            if (buffer[i].MonsterEntity == monster.MyEntity && ElapsedTime - buffer[i].HitTime < checkTime) {
                                alreadyCollided = true;
                                break;
                            }
                        }
                        if (alreadyCollided) continue;

                        ECB.AppendToBuffer(1, _entity, new HitInfoBuffer { MonsterEntity = monster.MyEntity, HitTime = ElapsedTime });
                    }

                    //本地端測試用，有機率擊殺怪物
                    uint Seed = (uint)(DeltaTime * 1000000f);
                    var random = new Unity.Mathematics.Random(Seed);
                    float value = random.NextFloat();
                    if (value < 0.01f) {
                        //在怪物實體身上建立死亡標籤元件，讓其他系統知道要死亡後該做什麼
                        ECB.AddComponent<MonsterDieTag>(3, monster.MyEntity);
                        //在怪物實體身上建立移除標籤元件
                        ECB.AddComponent(3, monster.MyEntity, new AutoDestroyTag { LifeTime = 6 });
                    }
                    else {
                        var hitTag = new MonsterHitTag {
                            MonsterID = monster.MonsterID,
                            StrIndex_SpellID = _collisionData.StrIndex_SpellID,
                        };
                        ECB.AddComponent(3, monster.MyEntity, hitTag);
                    }

                    //加入子彈擊中特效標籤元件
                    Entity effectEntity = ECB.CreateEntity(4);
                    var effectSpawnTag = new HitParticleSpawnTag {
                        Monster = monster,
                        SpellPrefabID = _collisionData.SpellPrefabID,
                        HitPos = monster.Pos,
                        HitDir = math.forward()
                    };
                    ECB.AddComponent(5, effectEntity, effectSpawnTag);
                }
            }
        }
    }
}
