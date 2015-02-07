using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour 
{
	private GameObject locked;
	private GameObject unlocked;
	private Transform coins;

	// Use this for initialization
	void OnEnable () 
	{
		coins = transform.FindChild ("Coins");

		locked = transform.FindChild ("Locked").gameObject;
		unlocked = transform.FindChild ("Unlocked").gameObject;

		//verify if it is unlocked
		string worldName = transform.parent.parent.name;
		int world = int.Parse(worldName.Substring (worldName.Length - 1, 1));

		string levelName = gameObject.name;
		int level = int.Parse(levelName.Substring (levelName.Length - 1, 1));

		//level unlocked
		if(Global.GetLevelUnlocked (world, level))
		{
			//activate unlock sprite
			locked.SetActive(false);
			unlocked.SetActive(true);

			int coinsUnlocked = Global.GetCoinsUnlocked(world, level);

			for(int i = 0; i < coinsUnlocked; i++)
			{
				coins.GetChild(i).gameObject.SetActive(true);
			}
		}
	}

	public void Select()
	{
		Global.currentWorld = Global.Worlds.World1;
		Global.currentLevel = Global.Levels.Level1;

		Application.LoadLevel ("World " + (int)Global.Worlds.World1);
	}
}
