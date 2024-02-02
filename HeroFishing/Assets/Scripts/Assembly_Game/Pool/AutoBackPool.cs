using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AutoBackPool : MonoBehaviour {
    [SerializeField]
    private float _backTime;
    public float BackTime {
        get => _backTime;
        set => _backTime = value;
    }

    private PoolManager _pool;

    //private float _scheduledTime;

    private void OnEnable() {
        if (_pool == null) _pool = PoolManager.Instance;
        Observable.Timer(TimeSpan.FromSeconds(_backTime)).TakeUntilDisable(this).Subscribe(_ => {
            _pool.Push(gameObject);
        });
    }
}
