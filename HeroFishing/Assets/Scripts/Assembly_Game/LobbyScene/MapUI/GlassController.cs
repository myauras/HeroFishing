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
        //Debug.Log(UICam.Instance == null);
        UICam.Instance.AddCameraStack(_camera);
    }

    [ContextMenu("Play")]
    public void Play() {
        _wholeGlass.SetActive(true);
        Observable.Timer(TimeSpan.FromMilliseconds(50)).Subscribe(_ => {
            _wholeGlass.SetActive(false);
            _breakGlassAnimator.gameObject.SetActive(true);
            _breakGlassAnimator.SetTrigger("Play");
        });
    }
}
