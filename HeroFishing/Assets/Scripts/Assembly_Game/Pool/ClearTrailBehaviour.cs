using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class ClearTrailBehaviour : MonoBehaviour
{
    /// <summary>
    /// 大量普攻可能造成trail在剛從池出來的時候就出現，需要clear
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
