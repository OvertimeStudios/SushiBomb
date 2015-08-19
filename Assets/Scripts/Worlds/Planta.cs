using UnityEngine;
using System.Collections;

public class Planta : MonoBehaviour 
{
	public bool swallow;

	void OnTriggerEnter2D(Collider2D col)
	{
		GetComponent<Animator>().SetBool("IsEating", true);

		StartCoroutine(DisableCollider());
	}

	private IEnumerator DisableCollider()
	{
		yield return new WaitForEndOfFrame();

		GetComponent<Collider2D>().enabled = false;

		if(swallow)
			StartCoroutine(RestoreCollider());
	}

	private IEnumerator RestoreCollider()
	{
		yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

		GetComponent<Collider2D>().enabled = true;

		GetComponent<Animator>().SetBool("IsEating", false);
	}
}
