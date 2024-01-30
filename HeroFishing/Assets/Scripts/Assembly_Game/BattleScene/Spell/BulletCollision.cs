using HeroFishing.Battle;
using HeroFishing.Main;
using HeroFishing.Socket;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public struct BulletCollisionInfo {
    public int HeroIndex;
    public int AttackID;
    public string SpellID;
    public float Speed;
    public Vector3 Direction;
    public float Radius;
    public bool DestroyOnCollision;
    public bool IsSub;
    public int TargetMonsterIdx;
}

public class BulletCollision : CollisionBase {

    private BulletCollisionInfo _info;
    private Transform _t;
    private Vector3 _position;
    private Monster _targetMonster;
    private List<Monster> _alreadyHitMonsters;
    //private Monster[] _waitToNetworkMonsters = new Monster[4];

    private static Vector2Int[] _offsetGrids;
    public void Init(BulletCollisionInfo info) {
        if (_offsetGrids == null) {
            _offsetGrids = new Vector2Int[9];
            _offsetGrids[0] = new Vector2Int(0, 0);// 子彈本身所在的網格
            _offsetGrids[1] = new Vector2Int(0, 1);// 上
            _offsetGrids[2] = new Vector2Int(1, 1);// 右上
            _offsetGrids[3] = new Vector2Int(1, 0);// 右
            _offsetGrids[4] = new Vector2Int(1, -1);// 右下
            _offsetGrids[5] = new Vector2Int(0, -1);// 下
            _offsetGrids[6] = new Vector2Int(-1, -1);// 左下
            _offsetGrids[7] = new Vector2Int(-1, 0);// 左
            _offsetGrids[8] = new Vector2Int(-1, 1);// 左上
        }

        _info = info;
        _t = transform;
        _position = _t.position;
        if (!_info.DestroyOnCollision)
            _alreadyHitMonsters = new List<Monster>();
    }

    public override void OnUpdate(float deltaTime) {
        base.OnUpdate(deltaTime);

        bool hasTarget = _info.TargetMonsterIdx != -1;
        bool isTargetMonsterAlive = Monster.TryGetMonsterByIdx(_info.TargetMonsterIdx, out _targetMonster);
        if (hasTarget && !isTargetMonsterAlive)
            hasTarget = false;

        if (hasTarget) {
            var targetPos = _targetMonster.transform.position;
            var direction = (targetPos - _position).normalized;
            _position += _info.Speed * deltaTime * direction;
            _t.rotation = Quaternion.LookRotation(direction);
        }
        else
            _position += _info.Speed * deltaTime * _info.Direction;

        var gridPos = new Vector2Int(
            (int)(_position.x / MonsterGrid.CELL_SIZE),
            (int)(_position.z / MonsterGrid.CELL_SIZE)
        );

        for (int i = 0; i < _offsetGrids.Length; i++) {
            var gridToCheck = gridPos + _offsetGrids[i];
            if (!MonsterGrid.TryGetMonsters(gridToCheck, out var monsters)) continue;
            for (int j = 0; j < monsters.Count; j++) {
                var monster = monsters[j];
                if (hasTarget && monster != _targetMonster) continue;
                //if (!isTargetMonsterAlive && monster == _targetMonster) continue;
                if (!monster.IsAlive) continue;
                if (_alreadyHitMonsters != null && _alreadyHitMonsters.Contains(monster)) continue;

                var monsterPosition = monster.transform.position;
                monsterPosition.y = _position.y;
                var sqrDistance = Vector3.SqrMagnitude(monsterPosition - _position);
                var radius = _info.Radius + monster.MyData.Radius;
                if (sqrDistance < radius * radius) {
                    _alreadyHitMonsters?.Add(monster);
                    monster.OnHit(_info.SpellID, transform.forward);

                    HitParticleSpawner.Spawn(new SpawnHitParticleInfo {
                        SpellID = _info.SpellID,
                        Monster = monster,
                        HitPos = _position,
                        HitRot = _t.rotation,
                    });

                    if (!_info.IsSub) {
                        SpellHitInfo hitInfo = new SpellHitInfo {
                            HeroIndex = _info.HeroIndex,
                            AttackID = _info.AttackID,
                            HitPosition = _position,
                            Monster = monster,
                            HitRotation = _t.rotation,
                        };
                        var spellData = HeroSpellJsonData.GetData(_info.SpellID);
                        spellData.Spell.OnHit(hitInfo);
                    }


                    if (!GameConnector.Connected) {
                        float value = UnityEngine.Random.value;
                        if (value < BattleManager.Instance.LocalDieThreshold) {
                            if (WorldStateManager.Instance.IsFrozen) {
                                monster.Explode(_info.HeroIndex);
                            }
                            else {
                                monster.Die(_info.HeroIndex);
                            }
                        }
                    }
                    else /*if (_alreadyHitMonsters == null)*/ {
                        GameConnector.Instance.Hit(_info.AttackID, new int[] { monster.MonsterIdx }, _info.SpellID);
                    }

                    if (_info.DestroyOnCollision) {
                        PoolManager.Instance.Push(gameObject);
                        return;
                    }
                }
            }
        }

        // 原本想要大家蒐集後一起丟出，但不能用這方法，因為already不會清空，會一直丟封包出去
        //if (GameConnector.Connected && _alreadyHitMonsters != null) {
        //    int[] idxs = _alreadyHitMonsters.Select(m => m.MonsterIdx).ToArray();
        //    GameConnector.Instance.Hit(_info.AttackID, _alreadyHitMonsters.Select(m => m.MonsterIdx).ToArray(), _info.SpellID);
        //}

        _t.position = _position;
    }
}
