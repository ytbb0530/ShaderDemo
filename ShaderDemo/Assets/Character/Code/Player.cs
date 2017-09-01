using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public JoyStick moveStick;

	public JoyStick dirStick;

	public GameObject body;


	private Animator anim;


	void Start () 
	{
		moveStick.setCallback (moveStickCallback);

		dirStick.setCallback (dirStickCallback);

		anim = body.GetComponent<Animator> ();
	}
	
	void Update () 
	{
		
	}

	public void moveStickCallback(Vector2 offset)
	{
		if (offset.magnitude < .1f) {
			anim.SetInteger ("speed", 0);
			return;
		}

		int i_speed = 1;
		float f_speed = 1.6f;

		if (offset.magnitude > 70) {
			i_speed = 2;
			f_speed = 4f;
		}
		
		Vector3 moveset = new Vector3 (offset.x, 0, offset.y);

		Vector3 translation = moveset.normalized * Time.deltaTime * f_speed;

		Vector3 tempPos = transform.position;

		transform.Translate (translation);

		body.transform.forward = transform.position - tempPos;

		body.transform.localEulerAngles += new Vector3 (-body.transform.localEulerAngles.x, 0, 0);

		anim.SetInteger ("speed", i_speed);
	}

	public void dirStickCallback(Vector2 offset)
	{
		if(offset.magnitude < .1f) return;

		Vector3 tempForward = body.transform.forward;

		transform.rotation = Quaternion.AngleAxis (offset.normalized.x, Vector3.up) * transform.rotation;

		body.transform.forward = tempForward;
	}

}