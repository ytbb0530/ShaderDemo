using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeKnife : MonoBehaviour 
{
	public GameObject objectRoot;
	public Transform p1;
	public Transform p2;
	public Transform p3;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	public void Cut()
	{
		ShapeCutable[] objects = objectRoot.GetComponentsInChildren<ShapeCutable> ();
		foreach (ShapeCutable cutter in objects) {
			cutter.cutWithPlane (p1.position, p2.position, p3.position);
		}
	}

}