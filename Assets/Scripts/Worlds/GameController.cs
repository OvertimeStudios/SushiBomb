using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour 
{
	#region events
	public static event Action OnGameStarted;
	public static event Action OnGameComplete;
	public static event Action OnGameOver;
	public static event Action OnAllCharactersStopMoving;
	#endregion
	
	public Transform CurrentLevel;

	/// <summary>
	/// How much sushis player need to put inside the chest?
	/// </summary>
	public int sushisToWin = 0;
	public float secondsToRestart = 2f;

	private static int sushisMoving;
	private static int sushisFallen;
	private static Character[] charactersInGame;
	public static bool gameOver;
	public static bool paused;

	public bool isEditor = false;
	public int startLevel = 1;

	#region singleton
	private static GameController instance;

	public static GameController Instance
	{
		get { return instance; }
	}
	#endregion

	#region get/set
	public static bool AreSushisMoving
	{
		get { return sushisMoving > 0; }
	}

	public static int RemainingSushis
	{
		get { return  charactersInGame.Length - sushisFallen; }
	}

	public static bool IsPaused
	{
		get { return paused; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		instance = this;

		sushisMoving = 0;
		sushisFallen = 0;
		gameOver = false;
		paused = false;

		Global.CurrentWorld = (Global.Worlds)int.Parse(Application.loadedLevelName.Substring(Application.loadedLevelName.Length - 1, 1));

		if(isEditor)
			Global.CurrentLevel = (Global.Levels)startLevel;
		else if(Global.CurrentLevel != 0)
			Global.CurrentLevel = (isEditor) ? (Global.Levels)startLevel : Global.CurrentLevel;
		else
			Global.CurrentLevel = Global.Levels.Level1;

		CurrentLevel = GameObject.Find("Levels").transform.FindChild ("Level " + (int)Global.CurrentLevel).transform;

		CurrentLevel.gameObject.SetActive (true);
		sushisToWin = CurrentLevel.GetComponent<LevelGameplay> ().sushisToWin;

		charactersInGame = CurrentLevel.GetComponentsInChildren<Character> ();

		SoundController.Instance.PlayMusic ((SoundController.Musics)((int)Global.CurrentWorld));
		SoundController.Instance.PlayAmbientSound ((SoundController.AmbientSounds)((int)Global.CurrentWorld));

		if(OnGameStarted != null)
			OnGameStarted();
	}

	void OnEnable()
	{
		Character.OnCharacterStartMoving += SushiExploded;
		Character.OnCharacterStopMoving += SushiStopped;
		Character.OnOutOfScreen += SushiOutOfGame;
		Character.OnEaten += SushiOutOfGame;
		Character.OnCaught += SushiOutOfGame;
	}

	void OnDisable()
	{
		Character.OnCharacterStartMoving -= SushiExploded;
		Character.OnCharacterStopMoving -= SushiStopped;
		Character.OnOutOfScreen -= SushiOutOfGame;
		Character.OnEaten -= SushiOutOfGame;
		Character.OnCaught -= SushiOutOfGame;
	}

	public void SushiStopped()
	{
		sushisMoving--;

		if(sushisMoving == 0)
		{
			if(OnAllCharactersStopMoving != null)
				OnAllCharactersStopMoving();

			if(Chest.SushisInside >= sushisToWin)
			{
				gameOver = true;

				//TODO: coins conditions for each level
				Global.LevelComplete((int)Global.CurrentWorld, (int)Global.CurrentLevel, 1);

				if(OnGameComplete != null)
					OnGameComplete();
			}
		}
	}

	public void SushiExploded()
	{
		sushisMoving++;
	}

	public void SushiOutOfGame()
	{
		sushisFallen++;
		sushisMoving--;

		if(RemainingSushis < sushisToWin)
		{
			StartCoroutine(Reset ());
		}
	}

	public IEnumerator Reset()
	{
		yield return new WaitForSeconds(secondsToRestart);

		sushisMoving = 0;
		sushisFallen = 0;
		paused = false;

		gameOver = false;

		Application.LoadLevel (Application.loadedLevel);

		if(OnGameOver != null)
			OnGameOver();

		if(OnGameStarted != null)
			OnGameStarted();
	}
}
