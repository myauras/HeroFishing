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
            // �ɶ��bdelay���A�θI���ɶ��~�Nreturn
            if (_collisionData.Timer < _collisionData.Delay ||
                _collisionData.Timer > _collisionData.CollisionTime + _collisionData.Delay) {
                return;
            }

            // �i�Ʈɶ��t
            float checkTime = _collisionData.Waves > 0 ? _collisionData.CollisionTime / _collisionData.Waves : 0.5f;

            // �W�L�`�i�ƩΪ̮ɶ�����i�ƪ��I���ɶ��Nreturn
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
                    // �p�G���]�w���סA��Ө���
                    if (_collisionData.Angle > 0 && !math.all(_collisionData.Direction == float3.zero)) {
                        var angle = MathUtility.angle(deltaPos, _collisionData.Direction);
                        if (angle > _collisionData.Angle / 2) continue;
                    }

                    //�[�J���a�������ҡA���`���Ҧb�����t�Τ��B�z�A�o�ˤ~��b�Ǫ����`�ɪ��D�̫᪺������V�C
                    var hitTag = new MonsterHitTag {
                        MonsterID = monster.MonsterID,
                        StrIndex_SpellID = _collisionData.StrIndex_SpellID,
                        HitDirection = float3.zero,
                    };
                    ECB.AddComponent(3, monster.MyEntity, hitTag);

                    //�[�J�l�u�����S�ļ��Ҥ���
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
