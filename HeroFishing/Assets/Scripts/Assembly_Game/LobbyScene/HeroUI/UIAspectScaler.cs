using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAspectScaler : MonoBehaviour {
    public enum RefVector { Horizontal, Vertical }
    [SerializeField]
    private RefVector _refVector;
    [SerializeField]
    private float _refValue = 100;
    [SerializeField]
    private float _originScaleFactor = 1;
    private RectTransform _rectTransform;

    private void Start() {
        _rectTransform = GetComponent<RectTransform>();
        Scale();
    }

    [ContextMenu("Scale")]
    public void Scale() {
        _rectTransform = GetComponent<RectTransform>();
        Rect parentRect = _rectTransform.parent.GetComponent<RectTransform>().rect;
        if (_refVector == RefVector.Horizontal) {
            var factor = parentRect.width / _refValue;
            _rectTransform.localScale = Vector3.one * factor * _originScaleFactor;
        }
        else {
            var factor = parentRect.height / _refValue;
            _rectTransform.localScale = Vector3.one * factor * _originScaleFactor;
        }
    }
}
