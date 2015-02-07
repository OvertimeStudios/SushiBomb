using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	[HideInInspector]
	public Transform currentWorld;

	#region singleton
	private static GameController instance;

	public static GameController Instance
	{
		get { return instance; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		instance = this;

		//TODO: this variables must be setted on Level Select scene
		Global.currentWorld = Global.Worlds.World1;
		Global.currentLevel = Global.Levels.Level1;

		currentWorld = GameObject.Find("Levels").transform.FindChild ("Level " + (int)Global.currentLevel).transform;

		currentWorld.gameObject.SetActive (true);
	}
}
