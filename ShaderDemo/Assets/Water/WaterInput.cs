using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WaterInput : EventTrigger
{
	private WaterSurface surface;

	public void pointerClick()
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

		if(surface == null)
		{
			surface = GameObject.Find ("Water").GetComponent<WaterSurface> ();
		}

		Vector3 castPosition = Camera.main.ScreenToWorldPoint (new Vector3(xPos, yPos, 10));

		surface.setRipple (castPosition);
	}

}
