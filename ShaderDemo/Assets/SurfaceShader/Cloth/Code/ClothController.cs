using UnityEngine;
using System.Collections;

public class ClothController : MonoBehaviour 
{
	public GameObject mainLight;

	public GameObject mainCamera;


	private float turnSpeed = 0;

	private float moveSpeed = 0;


	public void addTurnSpeed()
	{
		turnSpeed += 10;
	}

	public void subTurnSpeed()
	{
		turnSpeed -= 10;
	}

	public void resetTurnSpeed()
	{
		turnSpeed = 0;
	}

	public void addMoveSpeed()
	{
		moveSpeed += .2f;
	}

	public void subMoveSpeed()
	{
		moveSpeed -= .2f;
	}

	public void resetMoveSpeed()
	{
		moveSpeed = 0;
	}

	void Update () 
	{
		mainLight.transform.localEulerAngles += new Vector3 (0, turnSpeed * Time.deltaTime, 0);

		mainCamera.transform.position += new Vector3 (0, moveSpeed * Time.deltaTime, 0);

		if(mainCamera.transform.position.y > 1.5f)
		{
			mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x, 1.5f, mainCamera.transform.position.z);
		}

		if(mainCamera.transform.position.y < .2f)
		{
			mainCamera.transform.position = new Vector3 (mainCamera.transform.position.x, .2f, mainCamera.transform.position.z);
		}
	}
}
