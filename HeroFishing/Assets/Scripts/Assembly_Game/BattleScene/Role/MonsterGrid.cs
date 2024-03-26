using HeroFishing.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class MonsterGrid : MonoBehaviour, IUpdate {
    public int Order { get; } = 0;

    private Monster _monster;
    private Vector2Int _gridPos;
    private Vector3 _position;
    private Transform _t;
    private bool _forceMoving;
    private static int s_currentFrameCount;
    private static Dictionary<Vector2Int, List<Monster>> s_gridMap = new Dictionary<Vector2Int, List<Monster>>();

    public const float CELL_SIZE = 1f;
    public const int GRID_WIDTH = 20;
    public const int GRID_HEIGHT = 20;
    public const float FORCE_MOVE_TIME = 0.5f;
    public const float TELEPORT_DISTANCE = 5f;
    public static readonly Vector2Int GRID_BOUDNARY_X = new Vector2Int(-10, 10);
    public static readonly Vector2Int GRID_BOUDNARY_Y = new Vector2Int(-10, 10);
    public static readonly Vector2Int REMOVAL_BOUNDARY_X = new Vector2Int(-12, 12);
    public static readonly Vector2Int REMOVAL_BOUNDARY_Y = new Vector2Int(-12, 12);

    private void OnEnable() {
        UpdateSystem.Instance.RegisterUpdate(this);
    }

    private void OnDisable() {
        UpdateSystem.Instance.UnregisterUpdate(this);
    }

    public void Init() {
        if (_monster == null)
            _monster = GetComponent<Monster>();
        _t = transform;
        _position = _t.position;
    }

    public void OnUpdate(float deltaTime) {
        if (Time.frameCount != s_currentFrameCount) {
            s_gridMap.Clear();
            s_currentFrameCount = Time.frameCount;
        }

        if (_monster.MyData.Speed != 0 && !WorldStateManager.Instance.IsFrozen && _monster.IsRunning && !_forceMoving) {
            _position += _monster.MyData.Speed * deltaTime * transform.forward;
            _t.position = _position;
        }

        _gridPos = new Vector2Int(
            (int)(_position.x / CELL_SIZE),
            (int)(_position.z / CELL_SIZE)
        );

        bool inField = PosInGridBoundary(_position);
        if (inField) {
            if (!s_gridMap.TryGetValue(_gridPos, out var list)) {
                list = new List<Monster>();
                s_gridMap.Add(_gridPos, list);
            }
            list.Add(_monster);
            _monster.HasInField = true;
        }
        _monster.InField = inField;

        if (!PosInRemovalBoundary(_position)) {
            if (_monster.HasInField) {
                _monster.DestroyGOAfterDelay(1.0f);
            }
        }
    }

    public void Teleport(Vector3 position) {
        if (Vector3.SqrMagnitude(position - _position) > TELEPORT_DISTANCE * TELEPORT_DISTANCE) {
            _position = position;
            _t.position = _position;
        }
        else {
            _forceMoving = true;
            var startPosition = _position;
            var startTime = Time.time;
            position += FORCE_MOVE_TIME * _monster.MyData.Speed * _t.forward;
            Observable.EveryUpdate().TakeWhile(_ => Time.time < startTime + FORCE_MOVE_TIME).Subscribe(_ => {
                var value = Mathf.Clamp01((Time.time - startTime) / FORCE_MOVE_TIME);
                _position = Vector3.Lerp(startPosition, position, value);
                _t.position = _position;
            }, () => {
                _forceMoving = false;
            });
        }
    }

    public static bool TryGetMonsters(Vector2Int gridPos, out List<Monster> monsters) {
        return s_gridMap.TryGetValue(gridPos, out monsters);
    }

    public static void LogAllGridMonsters() {
        Debug.Log($"{s_gridMap.Count} grids has monsters");
        foreach (var pair in s_gridMap) {
            var gridPos = pair.Key;
            var monsters = pair.Value;
            string idxs = string.Join(", ", monsters.Select(m => m.MonsterIdx));
            Debug.Log($"{gridPos} has {monsters.Count} monsters: {idxs}");
        }
    }

    public static bool PosInGridBoundary(Vector3 position) {
        if (position.x < GRID_BOUDNARY_X.x || position.x > GRID_BOUDNARY_X.y || position.z < GRID_BOUDNARY_Y.x || position.z > GRID_BOUDNARY_Y.y)
            return false;
        return true;
    }

    public static bool PosInRemovalBoundary(Vector3 position) {
        if (position.x < REMOVAL_BOUNDARY_X.x || position.x > REMOVAL_BOUNDARY_X.y || position.z < REMOVAL_BOUNDARY_Y.x || position.z > REMOVAL_BOUNDARY_Y.y)
            return false;
        return true;
    }
}
