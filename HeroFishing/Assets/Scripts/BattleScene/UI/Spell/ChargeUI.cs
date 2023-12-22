using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeUI : MonoBehaviour {
    [SerializeField]
    private Image[] _images;
    private float _value;
    public float Value => _value;

    public bool IsFullCharge => _value == 1;


    public void Init() {
        //if (maxValue <= 0) {
        //    WriteLog.LogErrorFormat("charge max value cannot be 0");
        //    return;
        //}
        //_maxValue = maxValue;
        SetValue(0);
    }

    public void AddValue(float value) {
        _value = Mathf.Clamp01(_value + value);
        //float normalizedValue = Mathf.Clamp01(_value / _maxValue);
        for (int i = 0; i < _images.Length; i++) {
            var image = _images[i];
            image.gameObject.SetActive(_value < 1);
            image.fillAmount = 1 - _value;
        }
    }

    public void SetValue(float value) {
        _value = Mathf.Clamp01(value);
        //var normalizedValue = Mathf.Clamp01(value / _maxValue);
        for (int i = 0; i < _images.Length; i++) {
            var image = _images[i];
            image.gameObject.SetActive(_value < 1);
            image.fillAmount = 1 - _value;
        }
    }
}
