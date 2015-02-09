using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour 
{
	private GameObject back;
	private GameObject front;
	private GameObject animation;
	private Animator animator;

	// Use this for initialization
	void Start () 
	{
		back = transform.FindChild ("back").gameObject;
		front = transform.FindChild ("front").gameObject;
		animation = transform.FindChild ("animation").gameObject;
		animator = animation.GetComponent<Animator> ();

		back.SetActive (false);
		front.SetActive (false);
		animation.SetActive (true);
	}

	public void WasabiOn()
	{
		back.SetActive (true);
		front.SetActive (true);
		animation.SetActive (false);
	}

	public void WasabiOff()
	{
		back.SetActive (false);
		front.SetActive (false);
		animation.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
