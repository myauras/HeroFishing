using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBtn : MonoBehaviour {
    [SerializeField]
    private Button _button;
    [SerializeField]
    private SpellLevelUI _levelButton;
    [SerializeField]
    private ChargeUI _chargeButton;
    [SerializeField]
    private GameObject _dragObject;
    [SerializeField]
    private RectTransform _dragBigCircle;
    [SerializeField]
    private RectTransform _dragSmallCircle;
    [SerializeField]
    private Image[] _imgIcons;

    private int _chargeValue;
    public int MaxChargeValue => _spellData.Cost;

    public int SpellLevel => _levelButton.Level;
    public bool IsMaxLevel => _levelButton.IsMaxLevel;
    public int Threshold {
        get {
            if (_spellData.Threshold == null || _spellData.Threshold.Length == 0)
                return 0;
            return _spellData.Threshold[_levelButton.Level];
        }
    }
    public bool CanUse => !_levelButton.IsNoLevel && _chargeButton.IsFullCharge;

    public event Action<SpellName> OnSpellUpgrade;
    public event Action<SpellName> OnSpellUpgradeAnimationComplete;

    private HeroSpellJsonData _spellData;
    private RectTransform _rt;
    public SpellName SpellName => _spellData.SpellName;

    private static float MAX_DRAG_CIRCLE_LENGTH = 150;

    public void Init(HeroSpellJsonData spellData, int maxLevel) {
        if (spellData.SpellName == SpellName.attack) {
            WriteLog.LogErrorFormat("spell button輸入的spell name是普通攻擊");
            return;
        }

        _spellData = spellData;
        //if (!CheckSpell(_spellName)) return;
        _levelButton.Init(maxLevel);
        _chargeButton.Init();
        _button.interactable = false;
        _rt = GetComponent<RectTransform>();

        AddressablesLoader.GetSpriteAtlas("SpellIcon", atlas => {
            for (int i = 0; i < _imgIcons.Length; i++) {
                _imgIcons[i].sprite = atlas.GetSprite(_spellData.Ref);
            }
        });
    }

    public void OpenUpgradeBtn() {
        _levelButton.TurnOnLevelUp();
    }

    public void CloseUpgradeBtn() {
        _levelButton.TurnOffLevelUp();
    }

    public async void Upgrade() {
        var task = _levelButton.Upgrade();
        OnSpellUpgrade?.Invoke(_spellData.SpellName);
        await task;
        OnSpellUpgradeAnimationComplete?.Invoke(_spellData.SpellName);
    }

    [ContextMenu("charge")]
    public void AddChargeValue() {
        if (_levelButton.IsNoLevel) return;
        float value = (float)++_chargeValue / MaxChargeValue;
        _chargeButton.SetValue(value);
        if (_chargeButton.IsFullCharge) {
            _button.interactable = true;
        }
    }

    public void Play() {
        _chargeButton.ResetValue();
        _chargeValue = 0;
        _button.interactable = false;
    }

    public void Press() {
        _dragObject.SetActive(true);
    }

    public void Drag() {
        // 取得Screen Position，在此Rect Transform的相對位置
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, Input.mousePosition, Camera.main, out var localPos);
        // 取得相對方向
        var direction = localPos.normalized;
        // 取得相對角度 (上方角度為0，所以原點方向為Vector2.up)
        var angle = Vector2.SignedAngle(Vector2.up, direction);
        _dragBigCircle.transform.localEulerAngles = new Vector3(0, 0, angle);
        // 取得位置，最大距離為150
        _dragSmallCircle.transform.localPosition = Mathf.Min(MAX_DRAG_CIRCLE_LENGTH, localPos.magnitude) * direction;
    }

    public void Release() {
        _dragObject.SetActive(false);
    }

    //[ContextMenu("Level Up")]
    //public void LevelUp() {
    //    _levelButton.Upgrade();
    //}

    //[ContextMenu("Charge")]
    //public void AddChargeValue() {
    //    if (_levelButton.IsNoLevel) return;
    //    _chargeButton.AddValue();
    //    if (_chargeButton.IsFullCharge)
    //        _button.interactable = true;
    //}

    //private bool CheckSpell(SpellName spellName) {
    //    if (BattleManager.Instance == null) return false;
    //    var hero = BattleManager.Instance.GetHero(0);
    //    if (hero == null) { WriteLog.LogError("玩家英雄不存在"); return false; }
    //    _hero = hero;
    //    var spellData = HeroSpellJsonData.GetSpell(_hero.MyData.ID, spellName);
    //    if (spellData == null) { WriteLog.LogErrorFormat("玩家英雄的 {0} 不存在", spellName); return false; }
    //    _spellData = spellData;
    //    return true;
    //}

    //[ContextMenu("Charge")]
    //public void ChargeTest() {
    //    _chargeValue += 0.2f;
    //    _chargeButton.SetValue(_chargeValue);
    //}
}
