using UnityEngine;
using System;
using System.Collections;

public class ExplodableObject : MonoBehaviour 
{
	#region events
	public static event Action<Vector3, float> OnExplode;
	#endregion
	
	private Transform myTransform;

	public float force = 12f;

	//Destructable 2D parameters
	public LayerMask Layers = -1;
	
	public Texture2D StampTex;
	
	public Vector2 Size = Vector2.one;
	
	public float Angle;
	
	public float Hardness = 1.0f;

	void Start()
	{
		myTransform = transform;
	}

	void OnDestructibleStamped()
	{
		if(enabled)
			StartCoroutine(Explode ());
	}

	public IEnumerator Explode()
	{
		yield return new WaitForEndOfFrame();

		var ray      = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint((Vector2)myTransform.position));
		var distance = D2D_Helper.Divide(ray.origin.z, ray.direction.z);
		var point    = ray.origin - ray.direction * distance;
		
		D2D_Destructible.StampAll(point, Size, Angle, StampTex, Hardness, Layers);
		
		//call delegate
		if(OnExplode != null)
			OnExplode(myTransform.position, force);
		/*
		//TODO: play sound
		myAudioSource.clip = explosionFX;
		myAudioSource.loop = false;
		myAudioSource.Stop ();
		myAudioSource.Play ();
		*/

		Destroy(gameObject);
	}
}
