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

	[HideInInspector]
	public Transform currentWorld;

	/// <summary>
	/// How much sushis player need to put inside the chest?
	/// </summary>
	public int sushisToWin = 0;

	private static int sushisMoving;
	private static int sushisFallen;
	private static Character[] charactersInGame;
	public static bool gameOver;
	public static bool paused;

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
	void Awake () 
	{
		instance = this;

		sushisMoving = 0;
		sushisFallen = 0;
		gameOver = false;
		paused = false;

		Debug.Log (Global.currentWorld);
		Debug.Log (Global.currentLevel);

		if (Global.currentWorld == 0)
		{
			Global.currentWorld = Global.Worlds.World1;
			Global.currentLevel = Global.Levels.Level1;
		}
		
		currentWorld = GameObject.Find("Levels").transform.FindChild ("Level " + (int)Global.currentLevel).transform;

		currentWorld.gameObject.SetActive (true);

		charactersInGame = currentWorld.GetComponentsInChildren<Character> ();
	}

	void OnEnable()
	{
		Character.OnCharacterStartMoving += SushiExploded;
		Character.OnCharacterStopMoving += SushiStopped;
		Character.OnOutOfScreen += SushiOutOfScreen;
	}

	void OnDisable()
	{
		Character.OnCharacterStartMoving -= SushiExploded;
		Character.OnCharacterStopMoving -= SushiStopped;
		Character.OnOutOfScreen -= SushiOutOfScreen;
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

	public void SushiOutOfScreen()
	{
		sushisFallen++;
		sushisMoving--;

		if(RemainingSushis < sushisToWin)
		{
			Reset ();
		}
	}

	public void Reset()
	{
		sushisMoving = 0;
		sushisFallen = 0;
		paused = false;

		gameOver = false;

		Application.LoadLevel (Application.loadedLevel);

		/*if(OnGameOver != null)
			OnGameOver();*/
	}
}
