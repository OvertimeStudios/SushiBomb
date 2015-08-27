using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour 
{
	#region events
	public static event Action OnGameComplete;
	public static event Action OnGameOver;
	public static event Action OnAllCharactersStopMoving;
	#endregion
	
	public Transform currentLevel;

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

		Global.currentWorld = (Global.Worlds)int.Parse(Application.loadedLevelName.Substring(Application.loadedLevelName.Length - 1, 1));

		if(isEditor)
			Global.currentLevel = (Global.Levels)startLevel;
		else if(Global.currentLevel != 0)
			Global.currentLevel = (isEditor) ? (Global.Levels)startLevel : Global.currentLevel;
		else
			Global.currentLevel = Global.Levels.Level1;

		currentLevel = GameObject.Find("Levels").transform.FindChild ("Level " + (int)Global.currentLevel).transform;

		currentLevel.gameObject.SetActive (true);
		sushisToWin = currentLevel.GetComponent<LevelGameplay> ().sushisToWin;

		charactersInGame = currentLevel.GetComponentsInChildren<Character> ();

		SoundController.Instance.PlayMusic ((SoundController.Musics)((int)Global.currentWorld));
		SoundController.Instance.PlayAmbientSound ((SoundController.AmbientSounds)((int)Global.currentWorld));
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
				Global.LevelComplete((int)Global.currentWorld, (int)Global.currentLevel, 1);

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

		/*if(OnGameOver != null)
			OnGameOver();*/
	}
}
