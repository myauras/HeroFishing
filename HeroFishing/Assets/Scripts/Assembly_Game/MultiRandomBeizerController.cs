using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[Serializable]
public class MultiRandomBeizerController {
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
            if (!_beizerPositions.TryGetValue(i, out var positions)) {
                positions = new List<Vector3>();
                _beizerPositions[i] = positions;
            }
            positions.Clear();

            var keyCount = random.NextInt(_minMaxKeyCount.x, _minMaxKeyCount.y + 1);
            bool reverse = random.NextBool();
            for (int j = 0; j < keyCount; j++) {
                var keyValue = random.NextFloat(_minMaxKeyValues.x, _minMaxKeyValues.y) / keyCount + (float)j * 1 / keyCount;
                var distance = random.NextFloat(_minMaxDistance.x, _minMaxDistance.y) * _distanceMultiplier;
                int sign = reverse ^ j % 2 == 1 ? -1 : 1;
                var keyPosition = Vector3.Lerp(startPosition, endPosition, keyValue) + sign * Vector3.Cross(endPosition - startPosition, Vector3.up) * distance;
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
