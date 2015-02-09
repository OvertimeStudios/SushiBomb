using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour 
{
	#region events
	public static event Action OnGameComplete;
	public static event Action OnGameOver;
	public static event Action OnAllCharactersStopMoving;
	public static event Action OnReset;
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
	#endregion

	// Use this for initialization
	void Start () 
	{
		instance = this;

		sushisMoving = 0;
		sushisFallen = 0;

		//TODO: this variables must be setted on Level Select scene
		Global.currentWorld = Global.Worlds.World1;
		Global.currentLevel = Global.Levels.Level1;

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
			sushisMoving = 0;
			sushisFallen = 0;

			if(OnReset != null)
				OnReset();
		}
	}
}
