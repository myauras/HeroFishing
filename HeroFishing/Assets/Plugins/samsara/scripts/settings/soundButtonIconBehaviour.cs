using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Switches between ON/OFF states the sound of game.
/// Changes the button icon depending on the sound state.
/// </summary>
public class soundButtonIconBehaviour : MonoBehaviour
{
    private bool    volumeFlag  =   false;
    public  Sprite  iconOn;
    public  Sprite  iconOff;
    private Image   image;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void OnEnable()
    {
        SettingsManager.sound = volume;
        volumeFlag  = (volume > 0);
        setIcon();
    }

    /// <summary>
    /// Action this instance.
    /// </summary>
    public void action()
    {
        
        volumeFlag  = !volumeFlag;
        volume      = (volumeFlag) ? 1 : 0;
        SettingsManager.sound = volume;
        setIcon();
    }

    /// <summary>
    /// Sets the icon according to the volumen level.
    /// </summary>
    public void setIcon() {
        if (image==null)
        {
            Transform child = transform.Find("icon");
            image=child.GetComponent<Image>();
        }
        if (image!=null)
        {
            image.sprite = (volume > 0) ? iconOn : iconOff;
        }
    }
   
    /// <summary>
    /// Gets or sets the volume.
    /// </summary>
    /// <value>The volume.</value>
    private float volume
    {
        get {
            return AudioListener.volume;
        }
        set {
            AudioListener.volume = value;
        }
    }
}
