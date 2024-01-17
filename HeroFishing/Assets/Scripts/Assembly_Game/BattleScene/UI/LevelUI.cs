using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : BaseUI {
    [SerializeField]
    private Image _imgExp;
    [SerializeField]
    private Text _txtLevel;

    private Hero _hero;
    private const string TXT_LEVEL = "Lv.{0}";
    public override void RefreshText() {
        
    }

    public override void Init() {
        base.Init();
        _hero = BattleManager.Instance.GetHero(0);
        _hero.OnLevelUp += OnLevelUp;
        _hero.OnExpUpdate += OnExpUpdate;

        OnLevelUp(1);
    }

    private void OnLevelUp(int level) {
        _txtLevel.text = string.Format(TXT_LEVEL, level);
        _imgExp.fillAmount = 0;
    }

    private void OnExpUpdate(int exp, int fullExp) {
        _imgExp.fillAmount = (float)exp / fullExp;
    }
}
