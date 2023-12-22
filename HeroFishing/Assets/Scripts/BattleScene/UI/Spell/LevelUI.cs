using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour {
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Button _upgradeButton;

    private int _level;
    public int Level => _level;

    private int _maxLevel;
    public bool IsMaxLevel => _level == _maxLevel;
    public bool IsNoLevel => _level == 0;

    private const string LEVEL_UP_BTN = "LevelupBtn";
    private const string LEVEL_UP_BTN_CLOSE = "LevelupBtn_Close";
    private const string LEVEL_UP_BTN_CLOSE_FLICKER = "LevelupBtn_Close_Flicker";
    private const string LEVEL = "Lv{0:00}";

    private bool _isLevelBtnOpen;

    public void Init(int maxLevel) {
        _maxLevel = maxLevel;
    }


    [ContextMenu("Turn On")]
    public void TurnOnLevelUp() {
        if (_isLevelBtnOpen) return;

        _upgradeButton.enabled = true;
        _animator.SetTrigger(LEVEL_UP_BTN);
        _isLevelBtnOpen = true;
    }

    [ContextMenu("Turn Off")]
    public void TurnOffLevelUp() {
        if (!_isLevelBtnOpen) return;

        _animator.SetTrigger(LEVEL_UP_BTN_CLOSE);
        _upgradeButton.enabled = false;
        _isLevelBtnOpen = false;
    }

    [ContextMenu("Upgrade")]
    public async Task<bool> Upgrade() {
        if (_level == _maxLevel)
            return false;
        _animator.SetTrigger(LEVEL_UP_BTN_CLOSE_FLICKER);
        _upgradeButton.enabled = false;
        _isLevelBtnOpen = false;

        _level++;
        string param = string.Format(LEVEL, _level);
        _animator.SetTrigger(param);

        await UniTask.DelayFrame(1);
        //Debug.Log(state.IsName("LvupBtn.LevelupBtn_Close_Flicker"));
        //Debug.Log(state);
        //Debug.Log(state.normalizedTime);
        //Observable.EveryUpdate().Subscribe(_ => Debug.Log(state.normalizedTime));
        await UniTask.WaitUntil(() => {
            var state = _animator.GetCurrentAnimatorStateInfo(1);
            return state.normalizedTime > .95f;
        }).AsTask();
        return true;
    }

    public void SetLevel(int level) {
        _level = level;
        string param = string.Format(LEVEL + "_OK", _level);
        _animator.SetTrigger(param);
    }
}
