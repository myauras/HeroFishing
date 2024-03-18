using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassScaler : MonoBehaviour
{
    //[SerializeField]
    //private float _originZ;
    [SerializeField]
    private float _originScale;

    public void Scale() {
        var screenAspectRatio = (float)Screen.width / Screen.height;
        float canvasAspectRatio = 2026.0f / 936;
        var scale = screenAspectRatio / canvasAspectRatio;
        transform.localScale = new Vector3(_originScale * scale, _originScale * scale, _originScale);
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _originZ * scale);
    }
}
