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
    /// ���������I�ݭn�h�֭�
    /// </summary>
    [SerializeField, MinMax(1, 4)]
    private Vector2Int _minMaxKeyCount;

    /// <summary>
    /// �����ȷ|�b0~1�����Ӧa�����
    /// </summary>
    [SerializeField, MinMax(0, 1)]
    private Vector2 _minMaxKeyValues;

    /// <summary>
    /// �����I�Z��
    /// </summary>
    [SerializeField, MinMax(0, 1)]
    private Vector2 _minMaxDistance;

    [SerializeField, MinMax(0, 1)]
    private Vector2 _minMaxXDirDistanceOnYType;

    /// <summary>
    /// �����I�Z������
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
            // �p�G�S��list�A���s��list
            if (!_beizerPositions.TryGetValue(i, out var positions)) {
                positions = new List<Vector3>();
                _beizerPositions[i] = positions;
            }
            // �Nlist�M��
            positions.Clear();

            // ���o�����I���ƶq
            var keyCount = random.NextInt(_minMaxKeyCount.x, _minMaxKeyCount.y + 1);
            // ���o�@�}�l����V(���Υk)
            bool reverse = random.NextBool();
            for (int j = 0; j < keyCount; j++) {
                // ���o�����I�b����u�q����m
                // �p�G��0.25~0.75�A�I�N���|�����W�Τ��U���ƭȡA�p�G���W�L2���I�A������u���b�U�ۨ��o0.25~0.75
                var keyValue = random.NextFloat(_minMaxKeyValues.x, _minMaxKeyValues.y) / keyCount + (float)j * 1 / keyCount;
                // �Z���A����0~1�������A�t���ƪ��j�p�A�����ƭȪ��վ�
                var distance = random.NextFloat(_minMaxDistance.x, _minMaxDistance.y) * _distanceMultiplier;
                // ���o��e���Ÿ��A�Ĥ@�����k�A�ĤG���N�����C
                int sign = reverse ^ j % 2 == 1 ? -1 : 1;
                // ���o��e��X��V
                var xDirection = sign * Vector3.Cross(endPosition - startPosition, Vector3.up);
                // ���o��V�A�Y�ϬOY���]�n�t�@�IX��V�A���M���Ӧn��
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
