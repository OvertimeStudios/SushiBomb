using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Global : MonoBehaviour 
{
	public const string CURRENT_WORLD = "currentWorld";
	public const string CURRENT_LEVEL = "currentLevel";
	public const string UNLOCK_ANIMATION = "unlockAnimationWorld";

	public const int maxWorld = 3;
	public const int maxLevel = 12;
	
	public enum Worlds
	{
		World1 = 1,
		World2,
		World3,
	}
	
	public enum Levels
	{
		Level1 = 1,
		Level2,
		Level3,
		Level4,
		Level5,
		Level6,
		Level7,
		Level8,
		Level9,
		Level10,
		Level11,
		Level12,
	}

	private static Dictionary<string, int> levels;
	private static Dictionary<string, int> coins;

	private static Worlds currentWorld;
	private static Levels currentLevel;

	public static List<bool> unlockAnimationPlayed;
	private static bool isLoaded = false;

	#region get / set
	public static Worlds CurrentWorld
	{
		get 
		{
			if(!isLoaded)
				LoadValues();

			return currentWorld;
		}
		set
		{
			currentWorld = value;

			PlayerPrefs.SetInt(CURRENT_WORLD, (int)value);
			PlayerPrefs.Save();

		}
	}

	public static Levels CurrentLevel
	{
		get 
		{
			if(!isLoaded)
				LoadValues();
			
			return currentLevel;
		}
		set
		{
			currentLevel = value;

			PlayerPrefs.SetInt(CURRENT_LEVEL, (int)value);
			PlayerPrefs.Save();
		}
	}
	#endregion

	public static void LoadValues()
	{
		//verify any key on PlayerPrefs for first play
		if(!PlayerPrefs.HasKey("FirstPlay"))
		{
			PlayerPrefs.SetInt("FirstPlay", 1);

			PlayerPrefs.SetInt("level1_1", 1);
			PlayerPrefs.SetInt("coins1_1", 0);

			CurrentWorld = Worlds.World1;
			CurrentLevel = Levels.Level1;

			PlayerPrefs.Save();
		}

		unlockAnimationPlayed = new List<bool>();
		levels = new Dictionary<string, int> ();
		coins = new Dictionary<string, int> ();

		for(byte world = 1; world <= 3; world++)
		{
			unlockAnimationPlayed.Add(PlayerPrefs.HasKey(UNLOCK_ANIMATION + world) ? PlayerPrefs.GetInt(UNLOCK_ANIMATION + world) == 1 : false);

			Debug.Log(unlockAnimationPlayed[unlockAnimationPlayed.Count - 1]);

			for(byte level = 1; level <= 12; level++)
			{
				//get levels unlocked
				string key = "level" + world + "_" + level;
				levels.Add(key, (PlayerPrefs.HasKey(key)) ? PlayerPrefs.GetInt(key) : 0);

				key = "coins" + world + "_" + level;
				coins.Add(key, (PlayerPrefs.HasKey(key)) ? PlayerPrefs.GetInt(key) : 0);

			}
		}

		CurrentWorld = (Worlds)PlayerPrefs.GetInt(CURRENT_WORLD);
		CurrentLevel = (Levels)PlayerPrefs.GetInt(CURRENT_LEVEL);

		isLoaded = true;
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

	public static void LevelComplete(int world, int level, int coins)
	{
		if(levels == null)
			LoadValues();

		//unlock next level
		string key;
		if(level == maxLevel)
			key = "level" + (world + 1) + "_1";
		else
			key = "level" + world + "_" + (level + 1);

		PlayerPrefs.SetInt (key, 1);
		Global.levels [key] = 1;

		//unlock level coins
		key = "coins" + world + "_" + level;

		PlayerPrefs.SetInt (key, Mathf.Max (PlayerPrefs.GetInt (key), coins));
		Global.coins [key] = Mathf.Max (PlayerPrefs.GetInt (key), coins);
	}

	public static bool GetUnlockAnimationPlayer(Worlds world)
	{
		return unlockAnimationPlayed[(int)world - 1];
	}

	public static void UnlockAnimationPlayed(Worlds world)
	{
		unlockAnimationPlayed[(int)world - 1] = true;

		PlayerPrefs.SetInt(UNLOCK_ANIMATION + (int)world, 1);
		PlayerPrefs.Save();
	}

	public static void ClearData()
	{
		PlayerPrefs.DeleteAll ();

		if(levels != null)
			levels.Clear ();

		if(coins != null)
		coins.Clear ();

		isLoaded = false;
		LoadValues ();
	}
}
