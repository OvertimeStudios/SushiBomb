using UnityEngine;
using System;
using System.Collections;

public class Character : MonoBehaviour 
{
	#region Events
	public static event Action<GameObject> OnCharacterEnter;
	public static event Action<GameObject> OnCharacterExit;
	public static event Action OnCharacterStartMoving;
	public static event Action OnCharacterStopMoving;
	public static event Action OnOutOfScreen;
	#endregion

	private Transform trajectoryHolder;
	
	private bool manualRotation = false;
	private float initialRotation = -90f;
	private bool moving = false;
	
	public byte trajectorySpacing = 6;
	private LayerMask layerMask;

	//optimizations
	private Transform myTransform;
	private Rigidbody2D myRigidbody2D;

	//Reset
	private Vector3 initialPosition;

	// Use this for initialization
	protected virtual void Start () 
	{
		myTransform = transform;
		myRigidbody2D = rigidbody2D;

		initialPosition = myTransform.position;

		trajectoryHolder = transform.FindChild ("Trajectory");
		layerMask = LayerMask.NameToLayer ("Characters");

		HideAim ();
	}

	void OnEnable()
	{
		Wasabi.OnClick += ShowAim;
		Wasabi.OnRelease += HideAim;
		Wasabi.OnMoving += CalculateAim;
		Wasabi.OnExplode += Explode;

		GameController.OnReset += Reset;
	}

	void OnDisable()
	{
		Wasabi.OnClick -= ShowAim;
		Wasabi.OnRelease -= HideAim;
		Wasabi.OnMoving -= CalculateAim;
		Wasabi.OnExplode -= Explode;

		GameController.OnReset += Reset;
	}

	void Update()
	{
		if(manualRotation)
		{
			Vector3 euler = new Vector3(0f, 0f, (Mathf.Atan2(myRigidbody2D.velocity.y, myRigidbody2D.velocity.x) + initialRotation) * Mathf.Rad2Deg);
			myTransform.rotation = Quaternion.Euler(euler);
		}

		if(moving)
		{
			//OnCharacterStopMoving
			if(myRigidbody2D.velocity.magnitude == 0)
			{
				moving = false;

				initialRotation = myTransform.rotation.eulerAngles.z;

				//call delegate
				if(OnCharacterStopMoving != null)
					OnCharacterStopMoving();
			}

			//OnOutOfScreen
			Vector3 screenPos = Camera.main.WorldToScreenPoint(myTransform.position);
			if(screenPos.x < -20f || screenPos.y < -20f)
			{
				moving = false;

				gameObject.SetActive(false);

				//call delegate
				if(OnOutOfScreen != null)
					OnOutOfScreen();
			}
		}
	}

	public void HideAim()
	{
		trajectoryHolder.gameObject.SetActive (false);
	}

	public void ShowAim()
	{
		trajectoryHolder.gameObject.SetActive (true);
	}

	public void CalculateAim(Vector3 wasabiPosition, float force)
	{
		Vector2 velocity = ApplyVelocity (wasabiPosition, force);

		if(velocity.magnitude < Wasabi.Instance.minForce)
			HideAim();
		else
			ShowAim();

		Vector2 position = myTransform.position;
		
		bool hit = false;//did my trajectory hit something?
		bool thisTransformHit = false;//true when this transform hit something
		
		for(byte i = 0; i < trajectoryHolder.childCount; i++)
		{
			Transform t = trajectoryHolder.GetChild(i);
			thisTransformHit = false;
			
			if(!t.gameObject.activeSelf)
				t.gameObject.SetActive(true);
			
			if(!hit)
			{
				for(byte j = 0; j < trajectorySpacing; j++)
				{
					Vector2 prePosition = position;
					
					//S = So + Vot + 1/2at² (sorvetão)
					position += (velocity * Time.deltaTime) + (0.5f * (Vector2)Physics2D.gravity * Time.deltaTime * Time.deltaTime);
					
					//V = Vo + at (vovô ateu)
					velocity += (Vector2)Physics2D.gravity * Time.deltaTime;
					
					//Verifica se, entre os passos, ele colide. 
					//Se colidir, posiciona a mira na colisão e desativa as demais trajetórias
					Vector2 direction = position - prePosition;
					float aimDistance = Vector2.Distance(prePosition, position);
					RaycastHit2D hit2D = Physics2D.Raycast(prePosition, direction, aimDistance, ~(1 << layerMask));
					
					if(hit2D.collider != null)
					{
						//Debug.Log(hit2D.collider.name);
						
						thisTransformHit = true;
						hit = true;
						t.position = (Vector3)position;
						
						break;
					}
				}
			}
			
			//se não colidiu com nada, posiciona a trajetória no lugar
			if(!hit)
				t.position = (Vector3)position;
			else//se colidiu, desativa a trajetória para não mostrar o resto da trajetória
			{
				if(!thisTransformHit && t.gameObject.activeSelf)
					t.gameObject.SetActive(false);
			}
		}
	}

	//called on Wasabi.cs
	public void Explode(Vector3 wasabiPosition, float force)
	{
		manualRotation = true;
		moving = true;

		myRigidbody2D.velocity = ApplyVelocity (wasabiPosition, force);

		//call delegate
		if(OnCharacterStartMoving != null)
			OnCharacterStartMoving();
	}

	//when colliding to cenario, deactivate manual rotation
	void OnCollisionEnter2D(Collision2D col)
	{
		manualRotation = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.name == "Chest")
		{
			//call delegate
			if(OnCharacterEnter != null)
				OnCharacterEnter(gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.gameObject.name == "Chest")
		{
			//call delegate
			if(OnCharacterExit != null)
				OnCharacterExit(gameObject);
		}
	}

	private Vector2 ApplyVelocity(Vector3 wasabiPosition, float force)
	{
		float angle = Mathf.Atan2 (myTransform.position.y - wasabiPosition.y, myTransform.position.x - wasabiPosition.x);
		float distance = Vector2.Distance (myTransform.position, wasabiPosition);
		
		float relativeForce = force / (distance * 0.4f);

		relativeForce = Mathf.Clamp (relativeForce, 0f, force);

		return new Vector2 (Mathf.Cos (angle) * relativeForce, Mathf.Sin (angle) * relativeForce);
	}

	private void Reset()
	{
		myTransform.position = initialPosition;
		myTransform.rotation = Quaternion.identity;
		initialRotation = -90f;
		moving = false;
		manualRotation = false;

		gameObject.SetActive (true);
	}
}
