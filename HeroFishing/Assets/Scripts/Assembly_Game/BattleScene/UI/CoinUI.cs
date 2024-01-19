using Cysharp.Threading.Tasks;
using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinUI : BaseUI {
    [SerializeField]
    private Animator _animator;

    private static readonly int PARAMS_GET_COIN = Animator.StringToHash("GetCoin");

    private Hero _hero;
    public override void RefreshText() {
        
    }

    public override void Init() {
        base.Init();
        _hero = BattleManager.Instance.GetHero(0);
        _hero.OnPointUpdate += OnPointUpdate;
    }

    private void OnPointUpdate(int points) {
        _animator.SetTrigger(PARAMS_GET_COIN);
    }
}
