using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour 
{
	private GameObject lockedSprite;
	private GameObject unlockedSprite;
	private Transform coins;

	public bool unlocked = false;

	// Use this for initialization
	void OnEnable () 
	{
		coins = transform.FindChild ("Coins");

		lockedSprite = transform.FindChild ("Locked").gameObject;
		unlockedSprite = transform.FindChild ("Unlocked").gameObject;

		//verify if it is unlocked
		string worldName = transform.parent.parent.name;
		int numberLength = worldName.Length - ("World ").Length;
		int world = int.Parse(worldName.Substring (worldName.Length - numberLength, numberLength));

		string levelName = gameObject.name;
		numberLength = levelName.Length - ("Level ").Length;
		int level = int.Parse(levelName.Substring (levelName.Length - numberLength, numberLength));

		//level unlocked
		if(Global.GetLevelUnlocked (world, level))
		{
			unlocked = true;

			//activate unlock sprite
			lockedSprite.SetActive(false);
			unlockedSprite.SetActive(true);

			int coinsUnlocked = Global.GetCoinsUnlocked(world, level);

			for(int i = 0; i < coinsUnlocked; i++)
			{
				coins.GetChild(i).gameObject.SetActive(true);
			}
		}
	}

	public void Select()
	{
		if(LevelSelectController.IsNavioMoving) return;
		if(lockedSprite.activeSelf) return;

		string levelName = gameObject.name;
		int numberLength = levelName.Length - ("Level ").Length;
		int level = int.Parse(levelName.Substring (levelName.Length - numberLength, numberLength));

		//if selection a level that ship is current in
		if((Global.Levels)level == Global.CurrentLevel)
		{
			LevelSelectController.Instance.LoadLevel();
		}
		else
		{
			Global.CurrentWorld = Global.Worlds.World1;
			Global.CurrentLevel = (Global.Levels)level;

			LevelSelectController.Instance.TweenNavio ();
		}
	}
}
