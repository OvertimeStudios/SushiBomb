using UnityEngine;
using System.Collections;

public class Yolo : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () 
	{
		yield return new WaitForEndOfFrame();

		Application.LoadLevel ("World " + (int)Global.CurrentWorld);
	}
}
