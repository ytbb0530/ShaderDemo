using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBullet : MonoBehaviour 
{
	public ColorAIController controller;

	private int colorIndex;
	private bool moving;

	void Start () {
		moving = true;
		colorIndex = (int)(Random.value * 1000) % controller.colorTemplates.Count;
		MeshRenderer render = GetComponentInChildren<MeshRenderer> ();
		render.material.SetColor ("_Color", controller.colorTemplates[colorIndex]);
	}
	
	void Update () {
		if (moving) {
			transform.position += transform.forward * 15f * Time.deltaTime;
		}
	}

	public void OnTriggerEnter(Collider other){
		if (!moving) return;
		moving = false;

		if (!other.name.Equals ("StageCollision")) {
			controller.shoot (gameObject, colorIndex);
		}
		DestroyObject (gameObject);
	}
}
