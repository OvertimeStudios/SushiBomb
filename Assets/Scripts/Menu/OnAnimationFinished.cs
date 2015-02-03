using UnityEngine;
using System.Collections;

public class OnAnimationFinished : MonoBehaviour 
{

	public void OnTweenFinished()
	{
		MainMenuScreen.Instance.OnTweenFinished ();
	}
}
