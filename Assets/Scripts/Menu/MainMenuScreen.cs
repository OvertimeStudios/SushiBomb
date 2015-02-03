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

	}

	public void OnOptionsClicked()
	{
		if(currentScreen != Screen.Main) return;
		
		main.SetActive (false);
		options.SetActive (true);

		currentScreen = Screen.Options;

		options.transform.FindChild ("options").animation.Play ("OptionsIn");
	}

	//finished animation, show menu
	public void OnTweenFinished()
	{
		if(currentScreen == Screen.Options)
		{
			credits.SetActive(false);
			options.SetActive(true);
			options.transform.FindChild ("Menu").gameObject.SetActive (true);
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

		credits.transform.FindChild ("credits").animation.Play ("CreditsIn");
	}

	public void OnAchievementsClicked()
	{

	}

	public void OnVoltarClicked()
	{
		if(currentScreen == Screen.Options)
		{
			currentScreen = Screen.Main;

			options.transform.FindChild ("options").animation.Play ("OptionsOut");

			options.transform.FindChild ("Menu").gameObject.SetActive (false);
		}

		else if(currentScreen == Screen.Credits)
		{
			currentScreen = Screen.Options;

			credits.transform.FindChild ("credits").animation.Play ("CreditsOut");
			
			credits.transform.FindChild ("Menu").gameObject.SetActive (false);
		}
	}

	public void OnSoundFXChanged()
	{

	}

	public void OnMusicChanged()
	{

	}
}
