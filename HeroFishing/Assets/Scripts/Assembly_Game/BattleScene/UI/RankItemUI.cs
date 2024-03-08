using Cysharp.Threading.Tasks;
using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankItemUI : MonoBehaviour {
    [SerializeField]
    private Color _playerColor;
    [SerializeField]
    private Color _otherColor;
    [SerializeField]
    private GameObject _activeGO;
    [SerializeField]
    private Image _imgHeroIcon;
    [SerializeField]
    private Animation[] _animations;
    [SerializeField]
    private GameObject[] _playerGO;
    [SerializeField]
    private GameObject[] _otherGO;
    [SerializeField]
    private Graphic[] _graphics;

    private int _heroID;
    public int HeroID => _heroID;

    private int _playerIdx;
    public int PlayerIdx => _playerIdx;

    private bool _isPlayer;
    public bool IsPlayer => _isPlayer;

    private static bool s_isChanging;
    public static bool s_IsChanging => s_isChanging;

    public void Init() {
        ResetContent();
    }

    public void PlayAnimation() {
        s_isChanging = false;
        for (int i = 0; i < _animations.Length; i++) {
            var a = _animations[i];
            a.Play();
        }
    }

    public void SetContent(RankItemUI other) {
        SetContent(other.HeroID, other.PlayerIdx, other.IsPlayer);
    }

    public void SetContent(int heroID, int playerIdx, bool isPlayer) {
        //Debug.Log($"old player index: {_playerIdx} new player index {playerIdx}");
        AddressablesLoader.GetSpriteAtlas("HeroIcon", atlas => {
            var heroData = HeroJsonData.GetData(heroID);
            _imgHeroIcon.sprite = atlas.GetSprite(heroData.Ref);
        });
        SetGOActive(isPlayer);
        SetGraphicColor(isPlayer ? _playerColor : _otherColor);
        //_imgBackground.color = isPlayer ? _playerColor : _otherColor;

        _heroID = heroID;
        _playerIdx = playerIdx;
        _isPlayer = isPlayer;
    }

    public void ResetContent() {
        _imgHeroIcon.sprite = null;
        SetGOActive(false);
        SetGraphicColor(_otherColor);
        //_imgBackground.color = _otherColor;
        _playerIdx = -1;
    }

    public void SetActive(bool active) {
        gameObject.SetActive(active);
        _activeGO.SetActive(active);
    }

    public void OnIconChange() {
        s_isChanging = true;
    }

    private void SetGOActive(bool isPlayer) {
        for (int i = 0; i < _playerGO.Length; i++) {
            _playerGO[i].SetActive(isPlayer);
        }

        for (int i = 0; i < _otherGO.Length; i++) {
            _otherGO[i].SetActive(!isPlayer);
        }
    }

    private void SetGraphicColor(Color color) {
        for (int i = 0; i < _graphics.Length; i++) {
            _graphics[i].color = color;
        }
    }
}
