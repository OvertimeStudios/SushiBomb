using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	private Transform trajectoryHolder;
	
	private Vector2 velocity;
	private Vector2 position;
	
	private bool launched = false;
	
	public byte spacing = 6;
	private LayerMask layerMask;

	//optimizations
	private Transform myTransform;

	// Use this for initialization
	protected virtual void Start () 
	{
		velocity = new Vector2 ();

		myTransform = transform;

		trajectoryHolder = transform.FindChild ("Trajectory");
		layerMask = LayerMask.NameToLayer ("Characters");

		HideAim ();
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
		float angle = Mathf.Atan2 (myTransform.position.y - wasabiPosition.y, myTransform.position.x - wasabiPosition.x);
		float distance = Vector2.Distance (myTransform.position, wasabiPosition);

		float relativeForce = force / (distance * 0.3f);

		if(relativeForce < Wasabi.Instance.minForce)
			HideAim();
		else
			ShowAim();

		relativeForce = Mathf.Clamp (relativeForce, 0f, force);

		velocity.x = Mathf.Cos(angle) * relativeForce;
		velocity.y = Mathf.Sin(angle) * relativeForce;

		position = myTransform.position;
		
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
				for(byte j = 0; j < spacing; j++)
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

	public void Explode()
	{

	}
}
