using UnityEngine;
using System.Collections;

/// <summary>
/// Music states.
/// </summary>
enum MusicStates {
    not_initialized =   0,
    music_off       =   1,
    music_on        =   2,
}
/// <summary>
/// Settings manager class; Deals with player settings saved in PlayerPrefs.
/// </summary>
public static class SettingsManager {

	/// <summary>
	/// Gets or sets the selected I.
	/// </summary>
	/// <value>The selected I.</value>
	public static int selectedID
	{
		get{return PlayerPrefs.GetInt("selectedID");}
		set{PlayerPrefs.SetInt("selectedID",value);}
	}

	/// <summary>
	/// Gets or sets the sound.
	/// </summary>
	/// <value>The sound.</value>
	public static float sound
	{
		get{return PlayerPrefs.GetFloat("sound");}
		set{PlayerPrefs.SetFloat("sound",value);}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="game_core.SettingsManager"/> is music.
	///	0 Not initialized.
	/// 1 Music OFF.
	/// 2 Music ON .
	/// </summary>
	/// <value><c>true</c> if music; otherwise, <c>false</c>.</value>
	public static bool music
	{
		get{
				if(PlayerPrefs.GetInt("music")==(int) MusicStates.not_initialized)
				{
					PlayerPrefs.SetInt("music",(int)MusicStates.music_on);
				}
				return (PlayerPrefs.GetInt("music")==(int)MusicStates.music_on);
			}
		set{
				MusicStates val			=	MusicStates.music_off;
				if(value){val	=	MusicStates.music_on;}
				PlayerPrefs.SetInt("music",(int)val);
			}
	}
}
