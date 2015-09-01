using UnityEngine;
using System.Collections;

public class ChangeWorld : MonoBehaviour 
{
	public Level levelDependency;
	public Global.Worlds worldToLoad;

	public void Load()
	{
		if(levelDependency != null && !levelDependency.unlocked) return;

		LevelSelectController.Instance.LoadWorld(worldToLoad);
	}
}
