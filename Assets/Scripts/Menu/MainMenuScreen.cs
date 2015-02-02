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

	}

	public void OnCreditsClicked()
	{

	}

	public void OnAchievementsClicked()
	{

	}
}
