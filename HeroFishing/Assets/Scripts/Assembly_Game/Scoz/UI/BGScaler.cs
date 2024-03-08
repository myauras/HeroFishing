using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;

public class StageBGScaler : MonoBehaviour
{
    [SerializeField] Vector2 RefScreenSize = new Vector2(2026, 936);
    [SerializeField] float OriginScaleFactor = 1;
    void Start() {
        ScaleImgSizeByScreenSize(transform, RefScreenSize);
    }
    void ScaleImgSizeByScreenSize(Transform _trans, Vector2 _refSize) {
        float scaleRatio = ((float)Screen.width / (float)Screen.height) / (_refSize.x / _refSize.y);
        if (scaleRatio < 1) {
            _trans.localScale = Vector3.one * OriginScaleFactor;
            return;
        }
        _trans.localScale = Vector3.one * scaleRatio * OriginScaleFactor;
    }
}
