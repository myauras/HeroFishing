using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculates the ortographicCamera size
/// </summary>
public class orthographicCameraBehaviour : MonoBehaviour
{

    /// <summary>
    /// 
    /// </summary>
    public float pixelsPerUnit      = 64.0f;
    /// <summary>
    /// 
    /// </summary>
    public float screenTargetWidth  = 0.0f;

    /// <summary>
    /// 
    /// </summary>
    public float screenTargetHeight = 0.0f;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public float getOrthographicCameraSize(float width,float height,float pixelsPerUnit) {

        return (width / ( ((width / height) * 2.0f) * pixelsPerUnit));
    }
    /// <summary>
    /// Uses this for initialization
    /// </summary>
    void OnEnable()
    {
        screenTargetHeight  = (screenTargetHeight > 0.0f) ?  screenTargetHeight  :   Screen.height;
        screenTargetWidth   = (screenTargetWidth >0.0f) ?    screenTargetWidth   :   Screen.width;
        Debug.Log ("OrthoGraphic Size: "+getOrthographicCameraSize(screenTargetWidth,screenTargetHeight,pixelsPerUnit) );
        Camera.main.orthographicSize = getOrthographicCameraSize(screenTargetWidth, screenTargetHeight, pixelsPerUnit);
        GameObject cManager = GameObject.Find("CanvasManager");
        if (cManager!=null)
        {
            cManager.GetComponent<UnityEngine.UI.CanvasScaler>().referenceResolution = new Vector2(screenTargetWidth, screenTargetHeight);
            cManager.GetComponent<UnityEngine.UI.CanvasScaler>().referencePixelsPerUnit = pixelsPerUnit;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
