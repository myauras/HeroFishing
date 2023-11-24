using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class ClearTrailBehaviour : MonoBehaviour
{
    /// <summary>
    /// �j�q����i��y��trail�b��q���X�Ӫ��ɭԴN�X�{�A�ݭnclear
    /// </summary>
    [SerializeField]
    private TrailRenderer _trail;

    private void Reset() {
        _trail = GetComponent<TrailRenderer>();
    }

    private void OnDisable() {
        _trail.Clear();
    }
}
