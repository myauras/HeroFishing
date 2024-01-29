using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class musicButtonIconBehaviour : MonoBehaviour
{
    private bool volumeFlag = false;
    public Sprite iconOn;
    public Sprite iconOff;
    private Image image;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void OnEnable()
    {
        volumeFlag = SettingsManager.music;
        setIcon();
    }

    /// <summary>
    /// Action this instance.
    /// </summary>
    public void action()
    {
        volumeFlag = !volumeFlag;
        SettingsManager.music = volumeFlag;
        setIcon();
    }

    /// <summary>
    /// Sets the icon according to the volumen level.
    /// </summary>
    public void setIcon()
    {
        if (image == null)
        {
            Transform child = transform.Find("icon");
            image = child.GetComponent<Image>();
        }
        if (image != null)
        {
            image.sprite = (volumeFlag) ? iconOn : iconOff;
        }
    }
}
