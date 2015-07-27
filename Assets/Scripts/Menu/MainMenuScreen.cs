using UnityEngine;
using System.Collections;

public class MainMenuScreen : MonoBehaviour 
{
	private enum Screen
	{
		None,
		Main,
		Options,
		Credits,
		Achievements,
	}

	private Screen currentScreen = Screen.None;

	public GameObject placa;

	public GameObject main;
	public GameObject options;
	public GameObject credits;
	public GameObject achievements;

	private static MainMenuScreen instance;
	public static MainMenuScreen Instance 
	{ 
		get { return instance; } 
	}

	void Start()
	{
		instance = this;

		StartCoroutine (EnsureSoundControllerExist ());
	}

	IEnumerator EnsureSoundControllerExist()
	{
		yield return SoundController.Instance == null;

		SoundController.Instance.PlayMusic (SoundController.Musics.MainMenuTheme);
	}

	void Update()
	{
		if(currentScreen != Screen.None) return;

		if(Input.GetMouseButtonDown(0))
		{
			placa.SetActive(true);
			currentScreen = Screen.Main;
		}
	}

	public void OnPlayClicked()
	{
		Application.LoadLevel ("Level Select");
	}

	public void OnOptionsClicked()
	{
		if(currentScreen != Screen.Main) return;
		
		main.SetActive (false);
		options.SetActive (true);

		currentScreen = Screen.Options;

		options.transform.FindChild ("options").GetComponent<Animation>().Play ("OptionsIn");
	}

	//finished animation, show menu
	public void OnTweenFinished()
	{
		if(currentScreen == Screen.Options)
		{
			credits.SetActive(false);
			options.SetActive(true);
			options.transform.FindChild ("Menu").gameObject.SetActive (true);

			options.transform.FindChild("Menu").FindChild("Music").GetComponent<UISlider>().value = SoundController.musicVolume;
			options.transform.FindChild("Menu").FindChild("SoundFX").GetComponent<UISlider>().value = SoundController.soundFXVolume;
		}

		if(currentScreen == Screen.Main)
		{
			main.SetActive (true);
			options.SetActive (false);
		}

		if(currentScreen == Screen.Credits)
			credits.transform.FindChild ("Menu").gameObject.SetActive (true);
	}

	public void OnCreditsClicked()
	{
		options.transform.FindChild ("Menu").gameObject.SetActive (false);
		options.SetActive (false);

		credits.SetActive (true);

		currentScreen = Screen.Credits;

		credits.transform.FindChild ("credits").GetComponent<Animation>().Play ("CreditsIn");
	}

	public void OnAchievementsClicked()
	{

	}

	public void OnVoltarClicked()
	{
		if(currentScreen == Screen.Options)
		{
			currentScreen = Screen.Main;

			options.transform.FindChild ("options").GetComponent<Animation>().Play ("OptionsOut");

			options.transform.FindChild ("Menu").gameObject.SetActive (false);
		}

		else if(currentScreen == Screen.Credits)
		{
			currentScreen = Screen.Options;

			credits.transform.FindChild ("credits").GetComponent<Animation>().Play ("CreditsOut");
			
			credits.transform.FindChild ("Menu").gameObject.SetActive (false);
		}
	}

	public void ResetData()
	{
		//TODO: confirmation popup
		Global.ClearData ();
	}

	public void OnSoundFXChanged()
	{
		SoundController.Instance.SetSoundFXVolume ();
	}

	public void OnMusicChanged()
	{
		SoundController.Instance.SetMusicVolume ();
	}
}
