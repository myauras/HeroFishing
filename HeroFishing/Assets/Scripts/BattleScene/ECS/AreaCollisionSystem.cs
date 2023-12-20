using HeroFishing.Battle;
using HeroFishing.Socket;
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
        state.RequireForUpdate<CollisionSys>();
        HitInfoLookup = state.GetBufferLookup<HitInfoBuffer>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ECBSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecbWriter = ECBSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        bool localTest = SystemAPI.HasSingleton<LocalTestSys>();
        HitInfoLookup.Update(ref state);

        var monsters = SystemAPI.QueryBuilder().WithAll<MonsterValue>().WithAbsent<AutoDestroyTag>().Build().ToComponentDataArray<MonsterValue>(Allocator.TempJob);
        var jobHandle = new CollisionJob {
            ECB = ecbWriter,
            DeltaTime = SystemAPI.Time.DeltaTime,
            ElapsedTime = SystemAPI.Time.ElapsedTime,
            Monsters = monsters,
            HitInfoLookup = HitInfoLookup,
            IsNetwork = !localTest,
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
        [ReadOnly] public bool IsNetwork;

        public void Execute(ref AreaCollisionData _collisionData, in Entity _entity) {
            _collisionData.Timer += DeltaTime;
            // 時間在delay內，或碰撞時間外就return
            if (_collisionData.Timer < _collisionData.Delay ||
                _collisionData.Timer > _collisionData.CollisionTime + _collisionData.Delay) {
                return;
            }

            // 波數時間差
            float checkTime = _collisionData.Waves > 0 ? _collisionData.CollisionTime / _collisionData.Waves : 0.5f;

            // 超過總波數或者時間不到波數的碰撞時間就return
            if (_collisionData.WaveIndex >= _collisionData.Waves || _collisionData.Timer < _collisionData.Delay + checkTime * _collisionData.WaveIndex) {
                return;
            }
            _collisionData.WaveIndex++;

            Entity networkEntity = Entity.Null;
            if (IsNetwork) {
                networkEntity = ECB.CreateEntity(0);
                ECB.AddComponent(1, networkEntity, new SpellHitNetworkData {
                    AttackID = _collisionData.AttackID,
                    StrIndex_SpellID = _collisionData.StrIndex_SpellID
                });
                ECB.AddBuffer<MonsterHitNetworkData>(1, networkEntity);
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

                    //加入本地擊中標籤，死亡標籤在擊中系統中處理，這樣才能在怪物死亡時知道最後的擊中方向。
                    var hitTag = new MonsterHitTag {
                        MonsterID = monster.MonsterID,
                        StrIndex_SpellID = _collisionData.StrIndex_SpellID,
                        HitDirection = float3.zero,
                    };
                    ECB.AddComponent(3, monster.MyEntity, hitTag);

                    //加入子彈擊中特效標籤元件
                    Entity effectEntity = ECB.CreateEntity(4);
                    var effectSpawnTag = new HitParticleSpawnTag {
                        Monster = monster,
                        SpellPrefabID = _collisionData.SpellPrefabID,
                        HitPos = monster.Pos,
                        HitDir = math.forward()
                    };
                    ECB.AddComponent(5, effectEntity, effectSpawnTag);

                    if (IsNetwork) {
                        ECB.AppendToBuffer(6, networkEntity, new MonsterHitNetworkData {
                            Monster = monster,
                        });
                    }
                }
            }
        }
    }
}
