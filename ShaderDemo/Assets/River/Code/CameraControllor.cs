using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraControllor : MonoBehaviour 
{
	public Camera targetCamera;
	public Camera mirroCamera;
	public MeshRenderer river;
	public Cubemap cubemap;


	private int firstFlag = 0;

	private Vector3 offset = Vector3.zero;
	private float minX = 124;
	private float maxX = 197;
	private float minY = 1;
	private float maxY = 40;
	private float minZ = 85;
	private float maxZ = 190;


	private Vector3 dirOffset = Vector3.zero;
	private float dirMinX = 1;
	private float dirMaxX = 45;
	private float dirMinY = 120;
	private float dirMaxY = 250;

	private float cubeTime = 0;
	private Vector3 cubeOffset = Vector3.zero;


	public void goForward()
	{
		offset += new Vector3 (0, 0, -1);
	}

	public void goBack()
	{
		offset += new Vector3 (0, 0, 1);
	}

	public void goUp()
	{
		offset += new Vector3 (0, 1, 0);
	}

	public void goDown()
	{
		offset += new Vector3 (0, -1, 0);
	}

	public void goLeft()
	{
		offset += new Vector3 (1, 0, 0);
	}

	public void goRight()
	{
		offset += new Vector3 (-1, 0, 0);
	}

	public void stop()
	{
		offset = Vector3.zero;
	}

	public void dirUp()
	{
		dirOffset += new Vector3 (-1, 0, 0);
	}

	public void dirDowm()
	{
		dirOffset += new Vector3 (1, 0, 0);
	}

	public void dirLeft()
	{
		dirOffset += new Vector3 (0, -1, 0);
	}

	public void dirRight()
	{
		dirOffset += new Vector3 (0, 1, 0);
	}

	public void dirStop()
	{
		dirOffset = Vector3.zero;
	}

	void Update()
	{
		Vector3 targetPos = UpdatePosition ();

		Vector3 targetAngle = UpdateRotation ();

		bool able = controlMirroActive (targetPos, targetAngle);
		bool debug = false;

		if(able && debug)
		{
			UpdateMirroCam ();

			UpdateMirroCube ();

			river.material.SetVector ("_Offset", new Vector4(cubeOffset.x, cubeOffset.y, cubeOffset.z, 1));
		}
	}

	Vector3 UpdatePosition()
	{
		Vector3 targetPos = targetCamera.transform.position + offset * Time.deltaTime * 4;

		if(targetPos.x < minX)
		{
			targetPos = new Vector3 (minX, targetPos.y, targetPos.z);
		}

		if(targetPos.x > maxX)
		{
			targetPos = new Vector3 (maxX, targetPos.y, targetPos.z);
		}

		if(targetPos.y < minY)
		{
			targetPos = new Vector3 (targetPos.x, minY, targetPos.z);
		}

		if(targetPos.y > maxY)
		{
			targetPos = new Vector3 (targetPos.x, maxX, targetPos.z);
		}

		if(targetPos.z < minZ)
		{
			targetPos = new Vector3 (targetPos.x, targetPos.y, minZ);
		}

		if(targetPos.z > maxZ)
		{
			targetPos = new Vector3 (targetPos.x, targetPos.y, maxZ);
		}

		//targetCamera.transform.position = targetPos;

		return targetPos;
	}

	Vector3 UpdateRotation()
	{
		Vector3 targetAngle = targetCamera.transform.eulerAngles + dirOffset * Time.deltaTime * 5;

		if(targetAngle.x < dirMinX)
		{
			targetAngle = new Vector3 (dirMinX, targetAngle.y, targetAngle.z);
		}

		if(targetAngle.x > dirMaxX)
		{
			targetAngle = new Vector3 (dirMaxX, targetAngle.y, targetAngle.z);
		}

		if(targetAngle.y < dirMinY)
		{
			targetAngle = new Vector3 (targetAngle.x, dirMinY, targetAngle.z);
		}

		if(targetAngle.y > dirMaxY)
		{
			targetAngle = new Vector3 (targetAngle.x, dirMaxY, targetAngle.z);
		}

		//targetCamera.transform.eulerAngles = targetAngle;

		return targetAngle;
	}

	private bool controlMirroActive(Vector3 targetPos, Vector3 targetAngle)
	{
		if (firstFlag < 10) 
		{
			mirroCamera.gameObject.SetActive (true);

			firstFlag ++;
		}
		else
		{
			mirroCamera.gameObject.SetActive (targetCamera.transform.position != targetPos || targetCamera.transform.eulerAngles != targetAngle);
		}

		targetCamera.transform.position = targetPos;

		targetCamera.transform.eulerAngles = targetAngle;

		return mirroCamera.gameObject.activeSelf;
	}

	void UpdateMirroCam()
	{
		mirroCamera.fieldOfView = targetCamera.fieldOfView;

		Vector3 tp = new Vector3(transform.position.x, river.transform.position.y - transform.position.y + river.transform.position.y, transform.position.z);

		cubeOffset += tp - mirroCamera.transform.position;

		mirroCamera.transform.position = tp;

		Vector3 shadowPoint = new Vector3 (transform.position.x, river.transform.position.y, transform.position.z);

		float lenthY = Vector3.Distance (transform.position, shadowPoint);

		float angle = Vector3.Angle (transform.forward, new Vector3(0, -1, 0));

		float lenth = lenthY / Mathf.Cos (angle * Mathf.Deg2Rad);

		Vector3 tagetPosition = transform.position + transform.forward * lenth;

		mirroCamera.transform.forward = tagetPosition - mirroCamera.transform.position;
	}

	void UpdateMirroCube()
	{
		float now = Time.realtimeSinceStartup;

		if (now - cubeTime < 1f && cubeTime > 0){

			//return;
		}

		cubeTime = now;

		mirroCamera.RenderToCubemap (cubemap);

		cubeOffset = Vector3.zero;
	}

}