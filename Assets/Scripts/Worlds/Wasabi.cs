using UnityEngine;
using System;
using System.Collections;

public class Wasabi : MonoBehaviour 
{
	#region events
	public static event Action OnClick;
	public static event Action OnRelease;
	public static event Action<Vector3, float> OnMoving;
	public static event Action<Vector3, float> OnExplode;
	#endregion

	public float force = 10f;
	public float minForce = 4f;

	//optimizations
	private SpriteRenderer spRenderer;
	private Animator myAnimator;
	private Transform myTransform;

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
	}
	
	void Update()
	{
		#region input
		if(Input.GetMouseButtonDown(0))
			StartWasabi();
		
		if(Input.GetMouseButtonUp(0))
			StartExplosion();
		#endregion
	}

	void FixedUpdate () 
	{
		if(spRenderer.enabled && !myAnimator.GetBool("explode"))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			pos.z = 0;
			myTransform.position = pos;

			//call delegate
			if(OnMoving != null)
				OnMoving(pos, force);
		}
	}

	void StartWasabi()
	{
		if(GameController.gameOver) return;
		if(spRenderer.enabled) return;
		if(GameController.AreSushisMoving) return;

		//call delegate
		if(OnClick != null)
			OnClick ();

		spRenderer.enabled = true;

		myAnimator.Play ("Aparecendo");
	}

	void StartExplosion()
	{		
		if(!spRenderer.enabled) return;
		if(GameController.AreSushisMoving) return;

		//call delegate
		if(OnRelease != null)
			OnRelease();

		myAnimator.SetBool ("explode", true);
	}

	public void Explode()
	{
		//call delegate
		if(OnExplode != null)
			OnExplode(myTransform.position, force);
	}

	public void FinishExplosion()
	{
		myAnimator.SetBool ("explode", false);
		spRenderer.enabled = false;
	}
}