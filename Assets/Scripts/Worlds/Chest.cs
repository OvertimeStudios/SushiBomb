using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chest : MonoBehaviour 
{
	private GameObject back;
	private GameObject front;
	private GameObject anim;
	private Animator animator;

	private Vector3 initialPosition;
	
	private static List<GameObject> sushisInside;

	#region get/set
	public static int SushisInside
	{
		get { return sushisInside.Count; }
	}
	#endregion

	// Use this for initialization
	void Awake () 
	{
		sushisInside = new List<GameObject> ();

		back = transform.FindChild ("back").gameObject;
		front = transform.FindChild ("front").gameObject;
		anim = transform.FindChild ("animation").gameObject;
		animator = anim.GetComponent<Animator> ();

		initialPosition = transform.position;

		back.SetActive (false);
		front.SetActive (false);
		anim.SetActive (true);
	}

	void OnEnable()
	{
		Character.OnCharacterEnter += OnSushiInside;
		Character.OnCharacterExit += OnSushiOutside;
		
		GameController.OnGameComplete += Close;
		GameController.OnAllCharactersStopMoving += PlayIdleAnimation;
		GameController.OnGameOver += Reset;
		
		Wasabi.OnClick += ShowChest;
	}
	
	void OnDisable()
	{
		Character.OnCharacterEnter -= OnSushiInside;
		Character.OnCharacterExit -= OnSushiOutside;
		
		GameController.OnGameComplete -= Close;
		GameController.OnAllCharactersStopMoving -= PlayIdleAnimation;
		GameController.OnGameOver -= Reset;
		
		Wasabi.OnClick -= ShowChest;
	}

	/// <summary>
	/// Stop idle animation and show static chest with alpha
	/// </summary>
	public void ShowChest()
	{
		back.SetActive (true);
		front.SetActive (true);
		anim.SetActive (false);
	}

	/// <summary>
	/// Play idle animation if no sushis are inside
	/// </summary>
	public void PlayIdleAnimation()
	{
		if(SushisInside > 0) return;

		back.SetActive (false);
		front.SetActive (false);
		anim.SetActive (true);
	}

	public void OnSushiInside(GameObject sushi)
	{
		sushisInside.Add (sushi);
	}

	public void OnSushiOutside(GameObject sushi)
	{
		sushisInside.Remove (sushi);
	}

	/// <summary>
	/// Run animation to close the chest
	/// </summary>
	public void Close()
	{
		back.SetActive (false);
		front.SetActive (false);
		anim.SetActive (true);

		animator.SetBool ("CanClose", true);
	}

	//called in the end of Chest Closing animation as an event
	public void ChestClosed()
	{
		//TODO: Call end game popup
		Debug.Log ("Call end game popup");

		HUD.Instance.PlayVictoryAnimation ();
	}

	private void Reset()
	{
		animator.SetBool ("CanClose", false);

		back.SetActive (false);
		front.SetActive (false);
		anim.SetActive (true);

		transform.position = initialPosition;
	}
}
