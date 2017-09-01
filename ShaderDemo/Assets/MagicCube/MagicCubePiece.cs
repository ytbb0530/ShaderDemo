using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicCubePiece : MonoBehaviour 
{
	private int x;
	private int y;
	private int z;

	public List<GameObject> colliderList = new List<GameObject> ();

	private float gapRate;

	private int[,,] rotateValue = new int[3,2,6] {
		{	// x
			{ 6, 5, 3, 4, 1, 2 }, // -1
			{ 5, 6, 3, 4, 2, 1 }, // 1
		},

		{	// y
			{ 4, 3, 1, 2, 5, 6 }, // -1
			{ 3, 4, 2, 1, 5, 6 }, // 1
		},

		{	// z
			{ 1, 2, 5, 6, 4, 3 }, // -1
			{ 1, 2, 6, 5, 3, 4 }, // 1
		},
	};

	public void setValue(int _x, int _y, int _z, float _gapRate)
	{
		x = _x;
		y = _y;
		z = _z;
		gapRate = _gapRate;

		gameObject.name = "Cube_" + x + "_" + y + "_" + z;
		gameObject.transform.position = new Vector3 (x * gapRate, y * gapRate, z * gapRate);
	}

	public void refreshValue(int axis, int dir)
	{
		x = (int)transform.localPosition.x;
		y = (int)transform.localPosition.y;
		z = (int)transform.localPosition.z;

		int d = dir / Mathf.Abs (dir);

		d = d == -1 ? 0 : d;

		foreach(GameObject colliderGc in colliderList) {
			int id = int.Parse(colliderGc.name) - 1;

			int newid = rotateValue[axis, d, id ];

			colliderGc.name = newid + "";
		}
	}

	public bool equalValue(MagicCube.ROTATE_AXIS axis, int value)
	{
		if (value == -1) {
			string targetName = "";
			if (axis == MagicCube.ROTATE_AXIS.X) {
				targetName = "3";
			} else if (axis == MagicCube.ROTATE_AXIS.Y) {
				targetName = "6";
			} else if (axis == MagicCube.ROTATE_AXIS.Z) {
				targetName = "1";
			}
			
			foreach (GameObject c in colliderList) {
				if (c.name.Equals (targetName)) {
					return true;
				}
			}
		} else if (value == 1) {
			string targetName = "";
			if (axis == MagicCube.ROTATE_AXIS.X) {
				targetName = "4";
			} else if (axis == MagicCube.ROTATE_AXIS.Y) {
				targetName = "5";
			} else if (axis == MagicCube.ROTATE_AXIS.Z) {
				targetName = "2";
			}

			foreach (GameObject c in colliderList) {
				if (c.name.Equals (targetName)) {
					return true;
				}
			}
		} else if (value == 0) {
			string targetName_1 = "";
			string targetName_2 = "";
			if (axis == MagicCube.ROTATE_AXIS.X) {
				targetName_1 = "3";
				targetName_2 = "4";
			} else if (axis == MagicCube.ROTATE_AXIS.Y) {
				targetName_1 = "6";
				targetName_2 = "5";
			} else if (axis == MagicCube.ROTATE_AXIS.Z) {
				targetName_1 = "1";
				targetName_2 = "2";
			}

			foreach (GameObject c in colliderList) {
				if (c.name.Equals (targetName_1) || c.name.Equals (targetName_2)) {
					return false;
				}
			}

			return true;
		}

		return false;
	}

	public void log ()
	{
		Debug.Log ("MagicCubePiece: " + x + ", " + y + ", " + z);
	}

}
