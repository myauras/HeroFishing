using DG.Tweening;
using MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DOAnimationBtn : MonoBehaviour {
    [SerializeField]
    private Button _button;
    [SerializeField]
    private DOTweenView _view;
    private bool _isActive;
    public bool IsActive => _isActive;
    public Action<bool> OnClick;

    private void Reset() {
        _button = GetComponent<Button>();
        _view = GetComponent<DOTweenView>();
    }

    private void Start() {
        _button.onClick.AddListener(Play);
    }

    private void Play() {
        _isActive = !_isActive;
        _view.SetToggle(_isActive);
        OnClick?.Invoke(_isActive);
    }
}
