using UnityEngine;
using System.Collections;

public class LevelGameplay : MonoBehaviour 
{
	#region singleton
	private static LevelGameplay instance;
	public static LevelGameplay Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<LevelGameplay>();

			return instance;
		}
	}
	#endregion

	public int sushisToWin;

	void Start()
	{
		instance = this;
	}
}
