using HeroFishing.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicConverter : MonoBehaviour
{
    [SerializeField, Range(0, 1)]
    private float _value;
    [SerializeField]
    private Graphic[] _originGraphics;
    [SerializeField]
    private Graphic[] _destGraphics;

    private void OnValidate() {
        ChangeValue();
    }

    private void Start() {
        var cam = BattleManager.Instance.BattleCam;
        GetComponent<Canvas>().worldCamera = cam;
    }

    public void ChangeValue(float value) {
        _value = value;
        ChangeValue();
    }

    private void ChangeValue() {
        for (int i = 0; i < _originGraphics.Length; i++) {
            var color = _originGraphics[i].color;
            color.a = 1 - _value;
            _originGraphics[i].color = color;
        }

        for (int i = 0; i < _destGraphics.Length; i++) {
            var color = _destGraphics[i].color;
            color.a = _value;
            _destGraphics[i].color = color;
        }
    }
}
