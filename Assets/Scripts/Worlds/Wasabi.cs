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
	private AudioSource myAudioSource;

	//Destructable 2D parameters
	public LayerMask Layers = -1;
	
	public Texture2D StampTex;
	
	public Vector2 Size = Vector2.one;
	
	public float Angle;
	
	public float Hardness = 1.0f;

	//audio
	public AudioClip loopFX;
	public AudioClip explosionFX;


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
		myAudioSource = GetComponent<AudioSource> ();

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
		if(GameController.IsPaused) return;

		//play sound
		myAudioSource.clip = loopFX;
		myAudioSource.loop = true;
		myAudioSource.Stop ();
		myAudioSource.Play ();

		//call delegate
		if(OnClick != null)
			OnClick ();

		spRenderer.enabled = true;

		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		pos.z = 0;
		myTransform.position = pos;

		GetComponent<AudioSource> ().volume = SoundController.soundFXVolume;

		myAnimator.Play ("Aparecendo");
	}

	void StartExplosion()
	{
		if(!spRenderer.enabled) return;
		if(GameController.AreSushisMoving) return;
		if(GameController.IsPaused) return;

		//call delegate
		if(OnRelease != null)
			OnRelease();

		myAnimator.SetBool ("explode", true);
	}

	public void Stop()
	{
		FinishExplosion ();
	}

	public void Explode()
	{
		var ray      = Camera.main.ScreenPointToRay(Input.mousePosition);
		var distance = D2D_Helper.Divide(ray.origin.z, ray.direction.z);
		var point    = ray.origin - ray.direction * distance;
		
		D2D_Destructible.StampAll(point, Size, Angle, StampTex, Hardness, Layers);

		//call delegate
		if(OnExplode != null)
			OnExplode(myTransform.position, force);

		//play sound
		myAudioSource.clip = explosionFX;
		myAudioSource.loop = false;
		myAudioSource.Stop ();
		myAudioSource.Play ();
	}

	public void FinishExplosion()
	{
		myAnimator.SetBool ("explode", false);
		spRenderer.enabled = false;

		myAnimator.Play ("Aparecendo");
	}
}