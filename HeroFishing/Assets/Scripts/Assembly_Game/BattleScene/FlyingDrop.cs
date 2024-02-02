using HeroFishing.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FlyingDrop : MonoBehaviour {
    [SerializeField]
    private MultiRandomBeizerController _beizerController;

    [SerializeField]
    private float _speed;
    private float _timer;
    private float _progress;

    private const string FLYING_DROP_KEY = "OtherEffect/Script_Flying{0}";
    private const string GET_DROP_KEY = "OtherEffect/Script_Get{0}";

    public void Init(int dropID, string dropRef, int heroIndex) {
        var hero = BattleManager.Instance.GetHero(heroIndex);
        var startPos = transform.position;
        var endPos = hero.transform.position;
        endPos.y = 1.19f;
        _beizerController.Create(1, startPos, endPos);

        string key = string.Format(FLYING_DROP_KEY, dropRef);
        PoolManager.Instance.Pop(key, Vector3.zero, Quaternion.identity, transform, go => {
            Observable.EveryUpdate().TakeWhile(_ => _progress < 1).TimeInterval().Subscribe(timeInterval => {
                _timer += (float)timeInterval.Interval.TotalSeconds;
                var distance = Vector3.Distance(startPos, endPos);
                _progress = Mathf.Clamp01(_timer * _speed / distance);
                if (_beizerController.Update(0, _progress, out var pos)) {
                    go.transform.position = pos;
                }
            }, () => {
                PoolManager.Instance.Push(go);
                PoolManager.Instance.Push(gameObject);
                _progress = _timer = 0;
                endPos.y = 0;
                PoolManager.Instance.Pop(string.Format(GET_DROP_KEY, dropRef), endPos);
            });
        });
    }
}
