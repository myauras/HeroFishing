using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
namespace Scoz.Func {
    public class BGScaler : MonoBehaviour {
        [SerializeField] Vector2 RefScreenSize = new Vector2(2026, 936);
        void Start() {
            ScaleImgSizeByScreenSize(transform, RefScreenSize);
        }
        void ScaleImgSizeByScreenSize(Transform _trans, Vector2 _refSize) {
            float scaleRatio = ((float)Screen.width / (float)Screen.height) / (_refSize.x / _refSize.y);
            if (scaleRatio < 1) {
                _trans.localScale = Vector3.one;
                return;
            }
            _trans.localScale = Vector3.one * scaleRatio;
        }
    }
}