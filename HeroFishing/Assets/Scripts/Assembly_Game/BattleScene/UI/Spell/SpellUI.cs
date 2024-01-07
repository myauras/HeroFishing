using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellUI : BaseUI {
    [SerializeField]
    private SpellBtn _spellBtn1;
    [SerializeField]
    private SpellBtn _spellBtn2;
    [SerializeField]
    private SpellBtn _spellBtn3;
    [SerializeField]
    private Button _cancelBtn;
    [SerializeField]
    private PlayerAttackController _attackController;

    public bool IsSpellTest => BattleSceneManager.Instance != null && BattleSceneManager.Instance.IsSpellTest;
    public int TotalSpellLevel => _spellBtn1.SpellLevel + _spellBtn2.SpellLevel + _spellBtn3.SpellLevel;
    private Hero _hero;
    private SpellBtn _pressingBtn;
    private const float CANCEL_SPELL_THRESHOLD = 50;

    public override void RefreshText() {

    }

    public override void Init() {
        base.Init();
        _hero = BattleManager.Instance.GetHero(0);
        _spellBtn1.Init(HeroSpellJsonData.GetSpell(_hero.MyData.ID, SpellName.spell1), 4);
        _spellBtn2.Init(HeroSpellJsonData.GetSpell(_hero.MyData.ID, SpellName.spell2), 4);
        _spellBtn3.Init(HeroSpellJsonData.GetSpell(_hero.MyData.ID, SpellName.spell3), 2);

        _spellBtn1.OnSpellUpgrade += OnUpgrade;
        _spellBtn2.OnSpellUpgrade += OnUpgrade;
        _spellBtn3.OnSpellUpgrade += OnUpgrade;
        _spellBtn1.OnSpellUpgradeAnimationComplete += OnUpgradeComplete;
        _spellBtn2.OnSpellUpgradeAnimationComplete += OnUpgradeComplete;
        _spellBtn3.OnSpellUpgradeAnimationComplete += OnUpgradeComplete;

        _hero.OnLevelUp += OnLevelUp;
        _hero.OnSpellCharge += OnCharge;
        _hero.OnSpellPlay += OnPlay;
        OnLevelUp(1);
    }

    public void OnLevelUp(int level) {
        //Debug.Log(level + " " + TotalSpellLevel);
        if (level > TotalSpellLevel) {
            if (CanUpgrade(_spellBtn1, level))
                _spellBtn1.OpenUpgradeBtn();
            if (CanUpgrade(_spellBtn2, level))
                _spellBtn2.OpenUpgradeBtn();
            if (CanUpgrade(_spellBtn3, level))
                _spellBtn3.OpenUpgradeBtn();
        }
    }

    public bool CanUse(SpellName spellName) {
        return GetSpellBtn(spellName).CanUse;
    }

    private void OnUpgrade(SpellName spellName) {
        if (_hero.Level <= TotalSpellLevel) {
            CloseUpgradeBtn(_spellBtn1, spellName);
            CloseUpgradeBtn(_spellBtn2, spellName);
            CloseUpgradeBtn(_spellBtn3, spellName);
        }
        else {
            if (!CanUpgrade(_spellBtn1, _hero.Level))
                CloseUpgradeBtn(_spellBtn1, spellName);
            if (!CanUpgrade(_spellBtn2, _hero.Level))
                CloseUpgradeBtn(_spellBtn2, spellName);
            if (!CanUpgrade(_spellBtn3, _hero.Level))
                CloseUpgradeBtn(_spellBtn3, spellName);
        }
    }

    private void OnUpgradeComplete(SpellName spellName) {
        if (_hero.Level <= TotalSpellLevel) return;
        var btn = GetSpellBtn(spellName);
        if (CanUpgrade(btn, _hero.Level))
            btn.OpenUpgradeBtn();
    }

    private SpellBtn GetSpellBtn(SpellName spellName) {
        switch (spellName) {
            case SpellName.spell1:
                return _spellBtn1;
            case SpellName.spell2:
                return _spellBtn2;
            case SpellName.spell3:
                return _spellBtn3;
        }
        throw new Exception("spell name is not match " + spellName);
    }

    private void OnCharge(SpellName spellName) {
        var btn = GetSpellBtn(spellName);
        btn.AddChargeValue();
    }

    public void Press(string spellStr) {
        if (_attackController.ControlLock) return;
        if (MyEnum.TryParseEnum<SpellName>(spellStr, out var spellName))
            Press(spellName);
    }

    public void Press(SpellName spellName) {
        if (spellName == SpellName.attack) return;
        _pressingBtn = GetSpellBtn(spellName);
        _pressingBtn.Press();
        _attackController.OnPointerDown(spellName);
        _cancelBtn.gameObject.SetActive(true);
    }

    public void Drag() {
        if (_pressingBtn == null) return;
        _pressingBtn.Drag();
        _attackController.OnDrag();
    }

    public void Release() {
        if (_pressingBtn == null) return;
        _pressingBtn.Release();
        _pressingBtn = null;
        bool cancel = IsCancelBtnNearby();
        _attackController.OnPointerUp(cancel);
        _cancelBtn.gameObject.SetActive(false);
    }

    private void OnPlay(SpellName spellName) {
        if (spellName == SpellName.attack) return;
        var btn = GetSpellBtn(spellName);
        btn.Play();
    }

    private bool IsCancelBtnNearby() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_cancelBtn.targetGraphic.rectTransform, Input.mousePosition, Camera.main, out var localPos);
        return localPos.sqrMagnitude <= CANCEL_SPELL_THRESHOLD * CANCEL_SPELL_THRESHOLD;
    }

    private static bool CanUpgrade(SpellBtn spellBtn, int level) {
        return !spellBtn.IsMaxLevel && level >= spellBtn.Threshold;
    }

    private static void CloseUpgradeBtn(SpellBtn spellBtn, SpellName upgradeSpell) {
        if (spellBtn.SpellName != upgradeSpell)
            spellBtn.CloseUpgradeBtn();
    }
}
