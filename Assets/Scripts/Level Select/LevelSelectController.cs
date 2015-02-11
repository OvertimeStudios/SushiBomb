using UnityEngine;
using System.Collections;

public class LevelSelectController : MonoBehaviour 
{
	private Transform navio;

	private GameObject currentWorld;

	public static bool moveToNextLevel;

	private static bool navioMoving;

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

		if(Global.currentWorld == 0)
		{
			Global.currentWorld = Global.Worlds.World1;
			Global.currentLevel = Global.Levels.Level1;
		}

		currentWorld = GameObject.Find ("World " + (int)Global.currentWorld);

		currentWorld.SetActive (true);

		navio = GameObject.Find("Navio").transform;

		Transform currentLevel = currentWorld.transform.FindChild("Levels").FindChild("Level " + (int)Global.currentLevel);

		navio.transform.position = currentLevel.FindChild ("Waypoint").position;

		if(moveToNextLevel)
		{
			moveToNextLevel = false;

			int nextLevel = (int)Global.currentLevel + 1;

			if(nextLevel <= currentWorld.transform.FindChild("Levels").childCount)
			{
				Global.currentLevel = (Global.Levels)(nextLevel);

				TweenNavio();
			}
		}
	}

	public void TweenNavio()
	{
		navioMoving = true;

		//tween
		TweenPosition navioTween = navio.GetComponent<TweenPosition> ();

		navioTween.from = navio.localPosition;

		Transform levelTo = currentWorld.transform.FindChild("Levels").FindChild("Level " + (int)Global.currentLevel);

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
		Application.LoadLevel ("World " + (int)Global.currentWorld);
	}

}
