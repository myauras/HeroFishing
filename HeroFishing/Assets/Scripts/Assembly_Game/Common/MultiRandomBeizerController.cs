using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[Serializable]
public class MultiRandomBeizerController {
    public enum DirectionType {
        X, Y
    }
    [SerializeField]
    private DirectionType _directionType;
    /// <summary>
    /// 中間取樣點需要多少個
    /// </summary>
    [SerializeField, MinMax(1, 4)]
    private Vector2Int _minMaxKeyCount;

    /// <summary>
    /// 中間值會在0~1的哪個地方取樣
    /// </summary>
    [SerializeField, MinMax(0, 1)]
    private Vector2 _minMaxKeyValues;

    /// <summary>
    /// 取樣點距離
    /// </summary>
    [SerializeField, MinMax(0, 1)]
    private Vector2 _minMaxDistance;

    [SerializeField, MinMax(0, 1)]
    private Vector2 _minMaxXDirDistanceOnYType;

    /// <summary>
    /// 取樣點距離乘數
    /// </summary>
    [SerializeField]
    private float _distanceMultiplier;

    private Dictionary<int, List<Vector3>> _beizerPositions;
    public List<Vector3> this[int index] {
        get {
            if (_beizerPositions == null) return null;
            return _beizerPositions[index];
        }
    }

    public void Create(int count, Vector3 startPosition, Vector3 endPosition) {
        if (_beizerPositions == null)
            _beizerPositions = new Dictionary<int, List<Vector3>>();

        uint seed = (uint)DateTime.Now.Ticks;
        Random random = new Random(seed);
        for (int i = 0; i < count; i++) {
            // 如果沒有list，先新建list
            if (!_beizerPositions.TryGetValue(i, out var positions)) {
                positions = new List<Vector3>();
                _beizerPositions[i] = positions;
            }
            // 將list清空
            positions.Clear();

            // 取得中間點的數量
            var keyCount = random.NextInt(_minMaxKeyCount.x, _minMaxKeyCount.y + 1);
            // 取得一開始的方向(左或右)
            bool reverse = random.NextBool();
            for (int j = 0; j < keyCount; j++) {
                // 取得中間點在整條線段的位置
                // 如果為0.25~0.75，點就不會取之上或之下的數值，如果有超過2個點，讓整條線切半各自取得0.25~0.75
                var keyValue = random.NextFloat(_minMaxKeyValues.x, _minMaxKeyValues.y) / keyCount + (float)j * 1 / keyCount;
                // 距離，分為0~1的部分，差異化的大小，跟整體數值的調整
                var distance = random.NextFloat(_minMaxDistance.x, _minMaxDistance.y) * _distanceMultiplier;
                // 取得當前的符號，第一次為右，第二次就為左。
                int sign = reverse ^ j % 2 == 1 ? -1 : 1;
                // 取得當前的X方向
                var xDirection = sign * Vector3.Cross(endPosition - startPosition, Vector3.up);
                // 取得方向，即使是Y的也要配一點X方向，不然不太好看
                Vector3 direction = _directionType == DirectionType.X ? xDirection : Vector3.up + random.NextFloat(_minMaxXDirDistanceOnYType.x, _minMaxXDirDistanceOnYType.y) * xDirection ;
                var keyPosition = Vector3.Lerp(startPosition, endPosition, keyValue) + direction * distance;
                positions.Add(keyPosition);
            }
            positions.Add(endPosition);
            positions.Insert(0, startPosition);
        }
    }

    public bool Update(int index, float t, out Vector3 position) {
        position = Vector3.zero;
        if (!_beizerPositions.TryGetValue(index, out var positions)) {
            Debug.LogError($"index {index} has not created yet");
            return false;
        }

        position = BeizerCurve.GetPosition(positions, t);
        return true;
    }

    //private void OnDrawGizmos() {
    //    if (!_beizerPositions.TryGetValue(_debugGizmosIndex, out var positions)) return;
    //    for (int i = 0; i < positions.Count; i++) {
    //        Gizmos.DrawWireSphere(positions[i], 0.2f);
    //        if(i != positions.Count - 1) {
    //            Gizmos.DrawLine(positions[i], positions[i + 1]);
    //        }
    //    }
    //    Gizmos.DrawLine(positions[0], positions[positions.Count - 1]);
    //}
}
