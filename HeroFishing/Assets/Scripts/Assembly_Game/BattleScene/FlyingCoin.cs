using HeroFishing.Battle;
using HeroFishing.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FlyingCoin : MonoBehaviour {
    [SerializeField]
    private MultiRandomBeizerController _beizerController;
    [Header("Others")]
    [SerializeField]
    private float _speed;
    [SerializeField]
    private List<Transform> _flyingCoins = new List<Transform>();

    private float _timer;
    private float _progress;

    private const string FLYING_COIN_KEY = "OtherEffect/Script_FlyingCoinEffect";
    private const string GET_COIN_KEY = "OtherEffect/Script_GetCoin";

    public void Init(MonsterJsonData.MonsterSize size, int heroIndex) {
        Hero hero = BattleManager.Instance.GetHero(heroIndex);
        Vector3 startPos = transform.position;
        Vector3 endPos = hero.transform.position;
        endPos.y = 1.19f;

        int coinCount = GetFlyingCoinCount(size);
        _beizerController.Create(coinCount, startPos, endPos);

        for (int i = 0; i < coinCount; i++) {
            PoolManager.Instance.Pop(FLYING_COIN_KEY, Vector3.zero, Quaternion.identity, transform, OnCoinSpawn);
        }

        Observable.EveryUpdate().SkipWhile(_ => _flyingCoins.Count != coinCount).TakeWhile(_ => _progress < 1)
            .TimeInterval().Subscribe(timeInterval => {
            var deltaTime = timeInterval.Interval;
            _timer += (float)deltaTime.TotalSeconds;
            var distance = (endPos - startPos).magnitude;
            _progress = Mathf.Clamp01(_timer * _speed / distance);
            for (int i = 0; i < coinCount; i++) {
                var coin = _flyingCoins[i];
                if (_beizerController.Update(i, _progress, out Vector3 pos)) {
                    coin.transform.position = pos;
                }
            }
        }, () => {
            for (int i = 0; i < _flyingCoins.Count; i++) {
                PoolManager.Instance.Push(_flyingCoins[i].gameObject);
            }
            _flyingCoins.Clear();
            _progress = _timer = 0;
            PoolManager.Instance.Push(gameObject);
            endPos.y = 0;
            PoolManager.Instance.Pop(GET_COIN_KEY, endPos);
            hero.AddPoints();
        });
    }

    private void OnCoinSpawn(GameObject go) {
        _flyingCoins.Add(go.transform);
    }

    private int GetFlyingCoinCount(MonsterJsonData.MonsterSize size) {
        switch (size) {
            case MonsterJsonData.MonsterSize.Small:
                return 1;
            case MonsterJsonData.MonsterSize.Mid:
                return 3;
            case MonsterJsonData.MonsterSize.Large:
                return 5;
        }
        throw new System.Exception("size not match coin hit effect: " + size);
    }
}
