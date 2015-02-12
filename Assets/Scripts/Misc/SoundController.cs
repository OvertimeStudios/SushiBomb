using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour 
{
	#region enum
	public enum Musics
	{
		None,
		World1Theme,
		World2Theme,
		World3Theme,
		World4Theme,
		MainMenuTheme,
		VictoryTheme,
	}

	public enum AmbientSounds
	{
		None,
		Waves,
	}

	public enum SoundFX
	{
		None,
	}

	#endregion

	private static Musics currentMusic;
	public static float musicVolume = 1f;
	public static float soundFXVolume = 1f;

	#region Music Audioclips
	public AudioClip mainMenuTheme;
	public AudioClip world1Theme;
	public AudioClip victoryTheme;
	#endregion

	#region Ambient Audioclips
	public AudioClip waves;
	#endregion

	#region SoundFX Audioclips

	#endregion

	#region singleton
	private static SoundController instance;
	public static SoundController Instance
	{
		get { return instance; }
	}
	#endregion

	private AudioSource audioSourceMusic;
	private AudioSource audioSourceAmbient;

	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
		}

		DontDestroyOnLoad (gameObject);

		AudioSource[] audios = GetComponents<AudioSource> ();
		audioSourceMusic = audios [0];
		audioSourceAmbient = audios [1];
	}

	// Use this for initialization
	void Start () 
	{

	}

	public void PlayMusic(Musics music)
	{
		PlayMusic (music, true);
	}

	public void PlayMusic(Musics music, bool loop)
	{
		if(music == currentMusic) return;
		
		AudioClip clip = null;
		
		if(music == Musics.MainMenuTheme)
			clip = mainMenuTheme;
		else if(music == Musics.World1Theme)
			clip = world1Theme;
		else if(music == Musics.VictoryTheme)
			clip = victoryTheme;

		audioSourceMusic.loop = loop;
		audioSourceMusic.clip = clip;
		audioSourceMusic.Stop ();
		audioSourceMusic.Play ();
		
		currentMusic = music;
	}

	public void PlayAmbientSound(AmbientSounds ambientSound)
	{
		AudioClip clip = null;
		
		if(ambientSound == AmbientSounds.Waves)
			clip = waves;
		
		audioSourceAmbient.clip = clip;
		audioSourceAmbient.Stop ();
		audioSourceAmbient.Play ();
	}

	public void SetMusicVolume()
	{
		musicVolume = UISlider.current.value;

		audioSourceMusic.volume = musicVolume;
		audioSourceAmbient.volume = musicVolume;
	}

	public void SetSoundFXVolume()
	{
		soundFXVolume = UISlider.current.value;
	}
}
