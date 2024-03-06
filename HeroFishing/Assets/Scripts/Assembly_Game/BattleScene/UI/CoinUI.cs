using Cysharp.Threading.Tasks;
using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUI : BaseUI {
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Animator _txtAnimator;
    [SerializeField]
    private TextMeshProUGUI _txtCoin;

    private static readonly int PARAMS_GET_COIN = Animator.StringToHash("GetCoin");

    private Hero _hero;
    public override void RefreshText() {

    }

    public override void Init() {
        base.Init();
        _hero = BattleManager.Instance.GetHero(0);
        _hero.OnPointUpdate += OnPointUpdate;

        OnPointUpdate(_hero.Points, false);
    }

    private void OnPointUpdate(int points, bool needAnimation = false) {
        if (needAnimation) {
            _animator.SetTrigger(PARAMS_GET_COIN);
            _txtAnimator.SetTrigger(PARAMS_GET_COIN);
        }
        if (points < 0)
            points = 0;
        _txtCoin.text = points.ToString();
    }
}
