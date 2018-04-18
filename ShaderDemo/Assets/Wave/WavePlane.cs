using UnityEngine;
using System.Collections;

public class WavePlane : MonoBehaviour 
{

	private Material material;

//	private float waveScale;

	private float step = .04f;

	private int waveIndex;

	private Color colorX;

	private Color colorY;

	private Color colorS;


	void Start () 
	{
		waveIndex = 0;

		material = gameObject.GetComponent<MeshRenderer> ().material;

		colorX = new Color (0, 0, 0, 0);

		colorY = new Color (0, 0, 0, 0);

		colorS = new Color (0, 0, 0, 0);
	}
	
	public void touch() 
	{
		float xPos, yPos;

		if (Input.touchCount > 0) 
		{
			xPos = Input.touches [0].position.x;
			yPos = Input.touches [0].position.y;
		}
		else
		{
			xPos = Input.mousePosition.x;
			yPos = Input.mousePosition.y;
		}

		Ray ray = Camera.main.ScreenPointToRay(new Vector3(xPos, yPos, 0));

		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000f)) 
		{
			if (hit.collider.gameObject.Equals(gameObject)) 
			{
				Vector3 hitPos = (hit.point - gameObject.transform.position) / 10f;

				startWave(hitPos.x, hitPos.y);
			}
		}
	}

	void startWave(float x, float y)
	{
//		waveScale = 0;

		int curIndex = waveIndex;

		if (curIndex == 0) {
			colorX.r = x;
			colorY.r = y;
			colorS.r = .01f;
		}else if(curIndex == 1) {
			colorX.g = x;
			colorY.g = y;
			colorS.g = .01f;
		}else if(curIndex == 2) {
			colorX.b = x;
			colorY.b = y;
			colorS.b = .01f;
		}else if(curIndex == 3) {
			colorX.a = x;
			colorY.a = y;
			colorS.a = .01f;
		}

		material.SetColor ("_PosX", colorX);
		
		material.SetColor ("_PosY", colorY);

		waveIndex++;

		waveIndex = waveIndex >= 4 ? 0 : waveIndex;
	}

	void Update()
	{
		if(colorS.r > 0)
		{
			colorS.r += step;

			if (colorS.r > 3) 
			{
				colorS.r = -.01f;
			}
		}

		if(colorS.g > 0)
		{
			colorS.g += step;

			if (colorS.g > 3) 
			{
				colorS.g = -.01f;
			}
		}

		if(colorS.b > 0)
		{
			colorS.b += step;

			if (colorS.b > 3) 
			{
				colorS.b = -.01f;
			}
		}

		if(colorS.a > 0)
		{
			colorS.a += step;

			if (colorS.a > 3) 
			{
				colorS.a = -.01f;
			}
		}

		material.SetColor ("_Scale", colorS);
	}

}
