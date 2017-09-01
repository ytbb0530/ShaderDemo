using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorEnemy : MonoBehaviour 
{
	public GameObject player;
	public ColorBullet bulletTemplate;
	public GameObject bulletShooter;

	private Animator anim;
	private bool atking;

	void Start () 
	{
		anim = GetComponent<Animator> ();
		atking = false;
		StartCoroutine (attackClock());
	}
	
	void Update () 
	{
		if (atking) return;

		float length = Vector3.Distance (transform.position, player.transform.position);
		if (length > 5) {
			transform.forward = player.transform.position - transform.position;
			transform.position += transform.forward * Time.deltaTime * 1f;
			anim.SetBool ("moving", true);
		} else {
			anim.SetBool ("moving", false);
		}
	}

	private IEnumerator attackClock()
	{
		for(;;){
			yield return new WaitForSeconds (3 + Random.value * 2);
			Attack ();
		}
	}

	private void Attack()
	{
		if (atking) return;

		transform.forward = player.transform.position - transform.position;
		atking = true;
		anim.SetBool ("moving", false);
		anim.SetTrigger ("atk");
	}

	public void attackFinalFrameCallback()
	{
		atking = false;
	}

	public void attackCallback()
	{
		GameObject bulletGc = Instantiate (bulletTemplate.gameObject) as GameObject;
		bulletGc.transform.position = bulletShooter.transform.position;
		bulletGc.transform.forward = transform.forward + new Vector3(Random.value * .2f - .1f, Random.value * .2f - .1f, 0);
		bulletGc.name = "BULLET";
		bulletGc.SetActive (true);
	}

}
