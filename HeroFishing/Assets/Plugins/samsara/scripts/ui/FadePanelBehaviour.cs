using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Fade panel behaviour class; makes that panels 
/// (dis)appear on the screen smoothly.
/// </summary>
public class FadePanelBehaviour : MonoBehaviour {
	//VARIABLES
	public 	float 	fadeTime	=	1.0f;
	public	bool	fadeInFlag	=	true;
	private Image 	_image;
    private Text    _text;

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable() 
	{
		_image	=	GetComponent<Image> ();
        _text   =   GetComponent<Text>();
			if(fadeInFlag)
			{
            fadeIn();
			}else{
				StartCoroutine (fadeOut());
			}
	}

	/// <summary>
	/// Fade IN.
	/// </summary>
	public void fadeIn()
	{
        if (_image!=null)
        {
            _image.canvasRenderer.SetAlpha(0.01f);
            _image.CrossFadeAlpha(1.0f, fadeTime, false);
        }
		
        if (_text!=null) {
            _text.canvasRenderer.SetAlpha(0.01f);
            _text.CrossFadeAlpha(1.0f, fadeTime, false);
        }
	}

	/// <summary>
	/// Fade OUT.
	/// </summary>
	IEnumerator fadeOut()
	{
		float alpha = 1.0f;
        if (_image!=null)
        {
            _image.canvasRenderer.SetAlpha(alpha);
        }
        
		while (alpha > 0.1f && fadeTime>0) 
		{
			alpha-=0.1f;
            if (_image != null)
            {
                _image.canvasRenderer.SetAlpha(alpha);
            }
            yield return new WaitForSeconds ((fadeTime/10.0f));
		}
		gameObject.SetActive (false);
	}
}