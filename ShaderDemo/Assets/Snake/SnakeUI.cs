using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeUI : MonoBehaviour 
{
	public SnakeIO io;
	public GameObject cover;
	public GameObject train;
	public GameObject formal;

	void Awake()
	{
		cover.SetActive (true);
		train.SetActive (false);
		formal.SetActive (false);
	}

	public void GotoTrain()
	{
		cover.SetActive (false);
		train.SetActive (true);
		SnakeMap.training = true;
		io.Init ();
	}

	public void GotoFormal()
	{
		cover.SetActive (false);
		formal.SetActive (true);
		SnakeMap.training = false;
		io.Init ();
	}

}
