using UnityEngine;
using System.Collections;

public class WorldUnlockAnimation : MonoBehaviour 
{
	public TweenPosition[] animations;
	public GameObject[] objectsToActive;

	public void Play()
	{
		if(Global.GetUnlockAnimationPlayer(Global.CurrentWorld))
			GoToEndAnimation();
		else
		{
			foreach(TweenPosition tween in animations)
				tween.PlayForward();

			foreach(GameObject obj in objectsToActive)
				obj.SetActive(true);

			Global.UnlockAnimationPlayed(Global.CurrentWorld);
		}
	}

	public void GoToEndAnimation()
	{
		foreach(TweenPosition tween in animations)
			tween.transform.localPosition = tween.to;
	}
}
