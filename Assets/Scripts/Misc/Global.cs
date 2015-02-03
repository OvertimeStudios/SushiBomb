using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Global : MonoBehaviour 
{
	private static Dictionary<string, int> levels;
	private static Dictionary<string, int> coins;

	public static void LoadValues()
	{
		//verify any key on PlayerPrefs for first play
		if(!PlayerPrefs.HasKey("FirstPlay"))
		{
			PlayerPrefs.SetInt("FirstPlay", 1);

			PlayerPrefs.SetInt("level1_1", 1);
			PlayerPrefs.SetInt("coins1_1", 0);

			PlayerPrefs.Save();
		}

		levels = new Dictionary<string, int> ();
		coins = new Dictionary<string, int> ();

		for(byte world = 1; world <= 3; world++)
		{
			for(byte level = 1; level <= 9; level++)
			{
				//get levels unlocked
				string key = "level" + world + "_" + level;
				levels.Add(key, (PlayerPrefs.HasKey(key)) ? PlayerPrefs.GetInt(key) : 0);

				key = "coins" + world + "_" + level;
				coins.Add(key, (PlayerPrefs.HasKey(key)) ? PlayerPrefs.GetInt(key) : 0);

			}
		}
	}

	public static bool GetLevelUnlocked(int world, int level)
	{
		if(levels == null)
			LoadValues();

		string key = "level" + world + "_" + level;

		if (levels.ContainsKey (key)) 
			return levels[key] == 1;
		else
			return false;
	}

	public static int GetCoinsUnlocked(int world, int level)
	{
		if(coins == null)
			LoadValues();

		string key = "coins" + world + "_" + level;
		
		if (coins.ContainsKey (key)) 
			return coins[key];
		else
			return 0;
	}
}
