using UnityEngine;
using System.Collections;

public class MagicCubeUIController : MonoBehaviour 
{
	public MagicCube magicCube;


	private Vector2 dragStartPos;


	public void beginDrag()
	{
		dragStartPos = getCurTouchPosiiton ();

		magicCube.beginDrag ();
	}

	public void drag()
	{
		Vector2 curPos = getCurTouchPosiiton ();

		magicCube.drag (curPos - dragStartPos);

		dragStartPos = curPos;
	}

	public void endDrag()
	{
		magicCube.endDrag ();
	}

	public void click()
	{
		magicCube.click (getCurTouchPosiiton());
	}

	private Vector2 getCurTouchPosiiton()
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

		return new Vector2 (xPos / Screen.width, yPos / Screen.height);
	}

}
