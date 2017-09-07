using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLNode : MonoBehaviour 
{
	public int type;
	public float speed = 4;
	public MLEnviroment enviroment;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		transform.position += transform.forward * Time.deltaTime * speed;
		if(Vector3.Distance(transform.position, enviroment.transform.position) > 50){
			Remove ();
		}
	}

	public void Remove()
	{
		enviroment.removeNode(this);
	}

}