using UnityEngine;
using System.Collections;

public class AnimationEvents : MonoBehaviour 
{
	//called in the end of Chest Closing animation as an event
	public void ChestClosed()
	{
		GetComponentInParent<Chest> ().ChestClosed ();
	}

	public void VictoryFinished()
	{
		HUD.Instance.PlayPlacaAnimation (false);
	}
}
