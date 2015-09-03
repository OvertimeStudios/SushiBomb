using UnityEngine;
using System.Collections;

public class LevelSelectController : MonoBehaviour 
{
	private Transform navio;

	private GameObject currentWorld;

	public static bool moveToNextLevel;

	private static bool navioMoving;

	private Transform worlds;

	#region get/set
	public static bool IsNavioMoving
	{
		get { return navioMoving; }
	}
	#endregion

	#region singleton
	private static LevelSelectController instance;
	public static LevelSelectController Instance
	{
		get { return instance; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		instance = this;
		navioMoving = false;

		if(Global.CurrentWorld == 0)
		{
			Global.CurrentWorld = Global.Worlds.World1;
			Global.CurrentLevel = Global.Levels.Level1;
		}

		worlds = GameObject.Find("Worlds").transform;

		foreach(Transform world in worlds)
			world.gameObject.SetActive(false);

		currentWorld = worlds.GetChild((int)Global.CurrentWorld - 1).gameObject;

		currentWorld.SetActive (true);

		navio = GameObject.Find("Navio").transform;

		Transform currentLevel = currentWorld.transform.FindChild("Levels").FindChild("Level " + (int)Global.CurrentLevel);

		navio.transform.position = currentLevel.FindChild ("Waypoint").position;

		if(moveToNextLevel)
		{
			moveToNextLevel = false;

			int nextLevel = (int)Global.CurrentLevel + 1;

			if(nextLevel <= currentWorld.transform.FindChild("Levels").childCount)
			{
				Global.CurrentLevel = (Global.Levels)(nextLevel);

				TweenNavio();
			}
			else
			{
				//no more levels
				currentWorld.GetComponent<WorldUnlockAnimation>().Play();
			}
		}
		else
		{
			//last level unlocked
			if(Global.GetLevelUnlocked((int)Global.CurrentWorld, Global.maxLevel))
				currentWorld.GetComponent<WorldUnlockAnimation>().Play();

			SoundController.Instance.PlayMusic (SoundController.Musics.MainMenuTheme);
		}

	}

	public void TweenNavio()
	{
		navioMoving = true;

		//tween
		TweenPosition navioTween = navio.GetComponent<TweenPosition> ();

		navioTween.from = navio.localPosition;

		Transform levelTo = currentWorld.transform.FindChild("Levels").FindChild("Level " + (int)Global.CurrentLevel);

		navioTween.to = levelTo.localPosition + levelTo.FindChild ("Waypoint").localScale;

		navioTween.enabled = true;

		//scale
		float scaleX = (navioTween.to.x - navioTween.from.x) / Mathf.Abs (navioTween.to.x - navioTween.from.x);

		Vector3 scale = navio.localScale;
		scale.x *= scaleX;
		navio.localScale = scale;
	}

	public void LoadLevel()
	{
		Application.LoadLevel ("World " + (int)Global.CurrentWorld);
	}

	public void LoadWorld(Global.Worlds world)
	{
		Global.CurrentWorld = world;
		Global.CurrentLevel = Global.Levels.Level1;

		Application.LoadLevel(Application.loadedLevel);
	}

}
