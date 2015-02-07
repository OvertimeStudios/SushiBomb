using UnityEngine;
using System.Collections;

public class Wasabi : MonoBehaviour 
{
	public float force = 10f;
	public float minForce = 4f;

	//optimizations
	private SpriteRenderer spRenderer;
	private Animator myAnimator;
	private Transform myTransform;

	private Character[] charactersInGame;

	#region singleton
	private static Wasabi instance;
	public static Wasabi Instance
	{
		get { return instance; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		instance = this;

		spRenderer = GetComponent<SpriteRenderer>();
		myAnimator = GetComponent<Animator> ();
		myTransform = transform;

		spRenderer.enabled = false;

		charactersInGame = GameController.Instance.currentWorld.GetComponentsInChildren<Character> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		#region input
		if(Input.GetMouseButtonDown(0))
			OnClick();

		if(Input.GetMouseButtonUp(0))
			StartExplosion();
		#endregion

		if(spRenderer.enabled && !myAnimator.GetBool("explode"))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			pos.z = 0;
			myTransform.position = pos;

			foreach(Character character in charactersInGame)
			{
				character.CalculateAim(myTransform.position, force);
			}
		}
	}

	void OnClick()
	{
		if(spRenderer.enabled) return;

		spRenderer.enabled = true;

		myAnimator.Play ("Aparecendo");

		foreach(Character character in charactersInGame)
		{
			character.ShowAim();
		}
	}

	void StartExplosion()
	{
		foreach(Character character in charactersInGame)
		{
			character.HideAim();
		}

		myAnimator.SetBool ("explode", true);
	}

	public void Explode()
	{
		myAnimator.Play ("Explodindo");
	}

	public void FinishExplosion()
	{
		myAnimator.SetBool ("explode", false);
		spRenderer.enabled = false;
	}
}