using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBackPool : MonoBehaviour {
    [SerializeField]
    private float _backTime;

    private PoolManager _pool;

    //private float _scheduledTime;

    private async void OnEnable() {
        if (_pool == null) _pool = PoolManager.Instance;
        await UniTask.WaitForSeconds(_backTime);
        _pool.Push(gameObject);
    }
}
