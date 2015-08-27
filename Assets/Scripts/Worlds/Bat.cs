using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour 
{
	public float vel;
	private const float upForce = 15f;

	private TweenPosition tweenPosition;
	private bool forward;
	private bool changeScale;
	private bool caught;
	private float originalScale;

	private Animator myAnimator;

	// Use this for initialization
	IEnumerator Start () 
	{
		caught = false;

		originalScale = transform.localScale.x;

		myAnimator = GetComponent<Animator>();

		tweenPosition = GetComponent<TweenPosition>();
		tweenPosition.duration = Mathf.Max(Mathf.Abs(tweenPosition.from.y - tweenPosition.to.y), Mathf.Abs(tweenPosition.from.x - tweenPosition.to.x)) / vel;
		changeScale = tweenPosition.from.x != tweenPosition.to.x;

		transform.position = tweenPosition.from;

		yield return new WaitForEndOfFrame();

		forward = false;

		PlayAnimation();
	}

	public void OnTweenFinished()
	{
		PlayAnimation();
	}

	private void PlayAnimation()
	{
		if(forward)
			tweenPosition.PlayReverse();
		else
			tweenPosition.PlayForward();

		forward = !forward;

		//change scale
		if(changeScale)
		{
			int dir = (int)((tweenPosition.from.x - tweenPosition.to.x) / Mathf.Abs(tweenPosition.from.x - tweenPosition.to.x));

			if(!forward)
				dir *= -1;

			Vector3 localScale = transform.localScale;
			localScale.x = originalScale * dir;
			transform.localScale = localScale;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(caught) return;

		myAnimator.SetBool("Up", true);
		caught = true;

		tweenPosition.onFinished.Clear();
		//tweenPosition.SetOnFinished(GoUp);

		StartCoroutine(CaughtSushi(col));
	}

	private IEnumerator CaughtSushi(Collider2D col)
	{
		yield return new WaitForEndOfFrame();

		Transform sushi = col.transform;

		col.enabled = false;
		tweenPosition.enabled = false;

		sushi.transform.parent = transform;
		sushi.transform.localPosition = transform.GetChild(0).localPosition;

		StartCoroutine(GoUp ());
	}

	private IEnumerator GoUp()
	{
		Rigidbody2D myRigidbody2D = GetComponent<Rigidbody2D>();
		myRigidbody2D.gravityScale = 1;

		myRigidbody2D.AddForce(-Vector2.up * upForce * 12f);

		while(transform.position.y < 12f)
		{
			myRigidbody2D.AddForce(Vector2.up * upForce);

			yield return null;
		}

		Destroy(gameObject);
	}
}
