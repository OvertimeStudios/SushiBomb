using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	#region singleton
	private static HUD instance;
	public static HUD Instance
	{
		get { return instance; }
	}
	#endregion

	private UILabel wanted;
	private GameObject black;
	private GameObject placa;
	private GameObject pause;
	private GameObject resume;
	private GameObject nextLevel;
	private GameObject victory;

	private GameObject topLeft;
	private GameObject topRight;

	// Use this for initialization
	void Start () 
	{
		instance = this;

		topLeft = transform.FindChild ("TopLeft").gameObject;
		topRight = transform.FindChild ("TopRight").gameObject;

		wanted = topRight.transform.FindChild ("Wanted").FindChild ("Label").GetComponent<UILabel>();
		black = transform.FindChild ("Center").FindChild ("black").gameObject;
		placa = transform.FindChild ("Center").FindChild ("Placa").gameObject;
		pause = placa.transform.FindChild ("Pause").gameObject;
		resume = placa.transform.FindChild ("Resume").gameObject;
		nextLevel = placa.transform.FindChild ("Next Level").gameObject;
		victory = transform.FindChild ("Center").FindChild ("Victory").gameObject;

		UpdateSushisWanted (null);
	}
	
	void OnEnable()
	{
		Character.OnCharacterEnter += UpdateSushisWanted;
		Character.OnCharacterExit += UpdateSushisWanted;
	}

	void OnDisable()
	{
		Character.OnCharacterEnter -= UpdateSushisWanted;
		Character.OnCharacterExit -= UpdateSushisWanted;
	}

	void UpdateSushisWanted(GameObject character)
	{
		//need a delay of one frame to update
		StartCoroutine(WaitOneFrame());
	}
	
	IEnumerator WaitOneFrame()
	{
		while(true)
		{
			yield return null;

			wanted.text = string.Format ("{0:00}/{1:00}", Chest.SushisInside, GameController.Instance.sushisToWin);
		}
	}

	#region buttons methods
	public void OnPause()
	{
		if(GameController.IsPaused) return;

		GameController.paused = true;
		//Time.timeScale = 0;

		PlayPlacaAnimation();
		black.SetActive (true);

		StartCoroutine(StopWasabi ());
	}

	public void OnRestart()
	{
		if(GameController.IsPaused) return;

		Application.LoadLevel (Application.loadedLevel);
		//OnResume ();

		//GameController.Instance.Reset ();
	}

	public void OnResume()
	{
		GameController.paused = false;
		Time.timeScale = 1;

		if(placa.transform.position.y < 1f)
			PlayPlacaAnimation();

		black.SetActive (false);

		StartCoroutine(StopWasabi ());
	}

	public void OnLevelSelect()
	{
		Application.LoadLevel ("Level Select");
	}

	public void OnClose()
	{
		Application.LoadLevel ("MainMenu");
	}

	public void OnNextLevel()
	{
		LevelSelectController.moveToNextLevel = true;

		Application.LoadLevel ("Level Select");
	}
	#endregion

	IEnumerator StopWasabi()
	{
		yield return null;//wait 1 frame

		Wasabi.Instance.Stop ();
	}
	
	public void PlayPlacaAnimation()
	{
		pause.SetActive (true);
		resume.SetActive (true);
		nextLevel.SetActive (false);
		victory.SetActive (false);

		string anim = (placa.transform.position.y > 1f) ? "PlacaIn" : "PlacaOut";
		placa.animation.Play (anim);
	}

	public void PlayPlacaAnimationVictory()
	{
		pause.SetActive (false);
		resume.SetActive (false);
		nextLevel.SetActive (true);
		
		string anim = (placa.transform.position.y > 1f) ? "PlacaVictoryIn" : "PlacaVictoryOut";
		placa.animation.Play (anim);
	}

	public void PlayVictoryAnimation()
	{
		victory.animation.Rewind ();
		victory.animation.Play ("VictoryIn");

		if(!victory.activeSelf)
		{
			//play victory sound
			SoundController.Instance.PlayMusic(SoundController.Musics.VictoryTheme, false);

			topLeft.SetActive(false);
			topRight.SetActive(false);
			victory.SetActive(true);
		}
		else
		{
			topLeft.SetActive(true);
			topRight.SetActive(true);
		}

	}
}
