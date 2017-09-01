using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlayer : MonoBehaviour
{
	public JoyStick moveStick;
	public ColorBullet bulletTemplate;
	public GameObject bulletShooter;


	private Animator anim;
	private bool atking;

	void Start ()
	{
		moveStick.setCallback (moveStickCallback);

		anim = GetComponent<Animator> ();
		atking = false;
	}

	void FixedUpdate ()
	{
		Camera.main.transform.position = transform.position + new Vector3(0, 3.51f, -4f);

		if (Input.GetKeyDown (KeyCode.Space) && !atking) {
			Attack ();
		}
	}

	public void moveStickCallback(Vector2 offset)
	{
		if (offset.magnitude < .1f) {
			anim.SetBool ("moving", false);
			return;
		}

		if (atking) return;

		Vector3 moveset = new Vector3 (offset.x, 0, offset.y);

		transform.forward = moveset;
		transform.position += transform.forward * Time.deltaTime * 4f;

		anim.SetBool ("moving", true);
	}

	public void Attack()
	{
		if (atking) return;

		atking = true;
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
