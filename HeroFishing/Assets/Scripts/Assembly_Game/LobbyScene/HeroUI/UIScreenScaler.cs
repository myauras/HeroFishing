using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenScaler : MonoBehaviour {
    [SerializeField]
    private Vector2 _refScreenSize = new Vector2(2026, 936);
    private void Start() {
        Scale();
    }

    public void Scale() {
        if ((float)Screen.width / Screen.height < _refScreenSize.x / _refScreenSize.y) {
            var rectTransform = GetComponent<RectTransform>();
            var aspectRatio = Screen.width / _refScreenSize.x;
            var height = Screen.height / aspectRatio;
            var width = height / rectTransform.rect.height * rectTransform.rect.width;
            rectTransform.sizeDelta = new Vector2(width, height);
        }
    }
}
