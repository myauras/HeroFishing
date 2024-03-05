using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GlassController : MonoBehaviour {
    private static GlassController _instance;
    public static GlassController Instance => _instance;
    [SerializeField]
    private GameObject _wholeGlass;
    [SerializeField]
    private Animator _breakGlassAnimator;
    [SerializeField]
    private Camera _camera;

    private void Awake() {
        _instance = this;
    }

    private void OnDestroy() {
        if (_camera != null)
            UICam.Instance.ClearCameraStack();
    }

    public void Init() {
        UICam.Instance.AddCameraStack(_camera);
    }

    public void Select(bool active) {
        _wholeGlass.SetActive(active);
    }

    [ContextMenu("Play")]
    public void Play() {
        _wholeGlass.SetActive(false);
        _breakGlassAnimator.gameObject.SetActive(true);
        _breakGlassAnimator.SetTrigger("Play");
        Observable.EveryUpdate().Select(_ => _breakGlassAnimator.GetCurrentAnimatorStateInfo(0))
            .SkipWhile(stateInfo => stateInfo.normalizedTime < 0.95f).First().Subscribe(_ => {
            _breakGlassAnimator.gameObject.SetActive(false);
        });
    }
}
