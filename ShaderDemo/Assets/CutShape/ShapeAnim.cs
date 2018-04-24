using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeAnim : MonoBehaviour 
{
	private int state;
	private Vector3 targetPos;
	private Vector3 tempPos;

	void Awake () 
	{
		state = 0;
	}
	
	void Update () 
	{
		if (state == 1) updateMoveBy ();
	}

	public void moveBy(Vector3 targetPos)
	{
		this.targetPos = targetPos;
		tempPos = transform.position;
		state = 1;
	}

	private void updateMoveBy()
	{
		float time = 1.5f;
		float speed = targetPos.magnitude / time;

		transform.position += targetPos * speed * Time.deltaTime;

		if (Vector3.Distance (transform.position, tempPos) > .4f) {
			state = 0;
			Destroy (this);
		}
	}

}
