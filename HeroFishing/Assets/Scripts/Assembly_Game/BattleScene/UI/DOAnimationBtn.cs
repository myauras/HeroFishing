using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DOAnimationBtn : MonoBehaviour {
    [SerializeField]
    private Button _button;
    [SerializeField]
    private DOTweenAnimation[] _animations;
    private bool _isActive;

    private void Reset() {
        _button = GetComponent<Button>();
        _animations = GetComponentsInChildren<DOTweenAnimation>();
    }

    private void Start() {
        _button.onClick.AddListener(Play);
    }

    private void Play() {
        _isActive = !_isActive;

        for (int i = 0; i < _animations.Length; i++) {
            if (_isActive) {
                _animations[i].DORestart();
            }
            else {
                _animations[i].DOPlayBackwards();
            }
        }

    }
}
