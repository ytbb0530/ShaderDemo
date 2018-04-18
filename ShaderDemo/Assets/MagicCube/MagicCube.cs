using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MagicCube : MonoBehaviour 
{
	public enum FRAGMENT_INDEX
	{
		NULL = 0,
		FORWARD = 1,	// white
		BACK = 2,		// yellow
		LEFT  = 3,		// red
		RIGHT = 4,		// orange
		TOP = 5,		// blue
		BOTTOM = 6,		// green
	}

	public enum ROTATE_AXIS
	{
		X,
		Y,
		Z,
	}

	public enum ROTATE_DIR
	{
		POSITIVE = 90,
		NAGATIVE = -90,
	}

	public GameObject cubeTemplate;

	public GameObject frameLine;

	public GameObject btnRoot;


	private const float gapRate = 1.02f;

	private const float resistance = 0.001f;


	private List<MagicCubePiece> pieceList = new List<MagicCubePiece> ();

	private List<Vector2> inertiaList = new List<Vector2> ();

	private bool inDrag;

	private bool inInertia;

	private Vector2 vInertia;

	private Vector2 vResitance;


	private FRAGMENT_INDEX focusFragmentIndex;

	private List<MagicCubePiece> focusCubeList = new List<MagicCubePiece>();

	private ROTATE_AXIS focusAxis;

	private GameObject universalAxis;

	private bool inRotating;


	void Start () 
	{
		inDrag = false;

		inertiaList.Clear ();

		inInertia = false;

		pieceList.Clear ();

		inRotating = false;

		for(int x = -1; x <= 1; x++)
		{
			for(int y = -1; y <= 1; y++)
			{
				for(int z = -1; z <= 1; z++)
				{
					if (x == 0 && y == 0 && z == 0) {

						universalAxis = new GameObject ();

						universalAxis.name = "universalAxis";

						universalAxis.transform.parent = transform;

						universalAxis.transform.localPosition = Vector3.zero;

						universalAxis.transform.localEulerAngles = Vector3.zero;

						universalAxis.transform.localScale = new Vector3 (1, 1, 1);

						continue;
					}

					GameObject gc = Instantiate (cubeTemplate);

					gc.transform.parent = transform;

					gc.transform.localScale = new Vector3 (1, 1, 1);
					
					MagicCubePiece piece = gc.AddComponent<MagicCubePiece> ();

					piece.setValue (x, y, z, gapRate);

					pieceList.Add (piece);

					addColiider (piece, x, y, z);
				}
			}
		}

		setFocusFragment (FRAGMENT_INDEX.NULL);
	}

	private void addColiider(MagicCubePiece piece, int x, int y, int z)
	{
		
		if (x == -1) { // red
			addColiider(piece, FRAGMENT_INDEX.LEFT);
		} else if (x == 1) { // orange
			addColiider(piece, FRAGMENT_INDEX.RIGHT);
		}

		if (y == -1) { // green
			addColiider(piece, FRAGMENT_INDEX.BOTTOM);
		} else if (y == 1) { // blue
			addColiider(piece, FRAGMENT_INDEX.TOP);
		}

		if (z == -1) { // white
			addColiider(piece, FRAGMENT_INDEX.FORWARD);
		} else if (z == 1) { // yellow
			addColiider(piece, FRAGMENT_INDEX.BACK);
		}
	}

	private void addColiider(MagicCubePiece piece, FRAGMENT_INDEX index)
	{
		GameObject colliderGc = new GameObject ();
		colliderGc.transform.parent = piece.transform;
		colliderGc.transform.localPosition = Vector3.zero;
		colliderGc.transform.localEulerAngles = Vector3.zero;
		colliderGc.transform.localScale = new Vector3 (1, 1, 1);

		piece.colliderList.Add (colliderGc);

		BoxCollider collider = colliderGc.AddComponent<BoxCollider>();

		if (index == FRAGMENT_INDEX.LEFT) { // red
			colliderGc.name = "3";
			collider.size = new Vector3 ((gapRate - 1) * 2, gapRate, gapRate);
			collider.center = new Vector3 (-0.5f, 0, 0);
		} else if (index == FRAGMENT_INDEX.RIGHT) { // orange
			colliderGc.name = "4";
			collider.size = new Vector3 ((gapRate - 1) * 2, gapRate, gapRate);
			collider.center = new Vector3 (0.5f, 0, 0);
		}else if (index == FRAGMENT_INDEX.BOTTOM) { // green
			colliderGc.name = "6";
			collider.size = new Vector3 (gapRate, (gapRate - 1) * 2, gapRate);
			collider.center = new Vector3 (0, -0.5f, 0);
		} else if (index == FRAGMENT_INDEX.TOP) { // blue
			colliderGc.name = "5";
			collider.size = new Vector3 (gapRate, (gapRate - 1) * 2, gapRate);
			collider.center = new Vector3 (0, 0.5f, 0);
		}else if (index == FRAGMENT_INDEX.FORWARD) { // white
			colliderGc.name = "1";
			collider.size = new Vector3 (gapRate, gapRate, (gapRate - 1) * 2);
			collider.center = new Vector3 (0, 0, -0.5f);
		} else if (index == FRAGMENT_INDEX.BACK) { // yellow
			colliderGc.name = "2";
			collider.size = new Vector3 (gapRate, gapRate, (gapRate - 1) * 2);
			collider.center = new Vector3 (0, 0, 0.5f);
		}
	}

	void Update () 
	{
		addDragData (Vector2.zero);

		updateInertia ();

		updateFocusFragment ();
	}

	private void addDragData(Vector2 data)
	{
		if (!inDrag) return;
			
		if (inertiaList.Count < 20) {
			inertiaList.Add (data);
		} else {

			for(int i = 0; i < inertiaList.Count - 1; i++)
			{
				inertiaList [i] = inertiaList [i + 1];
			}

			inertiaList [inertiaList.Count - 1] = data;
		}
	}

	private void updateInertia()
	{
		if (!inInertia) return; 

		vInertia -= vResitance;

		if(vInertia.magnitude < resistance)
		{
			inInertia = false;

			return;
		}

		rotateRootByDelta (vInertia);
	}

	public void beginDrag()
	{
		inDrag = true;

		inertiaList.Clear ();

		inInertia = false;

		setFocusFragment (FRAGMENT_INDEX.NULL);
	}

	public void drag(Vector2 delta)
	{
		rotateRootByDelta (delta);

		addDragData (delta);
	}

	public void endDrag()
	{
		inDrag = false;

		inInertia = true;

		vInertia = Vector2.zero;

		foreach(Vector2 v in inertiaList)
		{
			vInertia += v;
		}

		vInertia = vInertia / inertiaList.Count;

		vResitance = vInertia.normalized * resistance;
	}

	private void rotateRootByDelta(Vector2 delta)
	{
		rotateRoot (delta.y, -delta.x);
	}

	private void rotateRoot(float x, float y)
	{
		Vector3 axis = new Vector3(x, y, 0);

		transform.rotation = Quaternion.AngleAxis (axis.magnitude * 100, axis.normalized) * transform.rotation;
	}

	private void updateFocusFragment()
	{
		if (inDrag) return;
		if (inInertia) return;
		if (focusFragmentIndex != FRAGMENT_INDEX.NULL) return;

		Ray ray = Camera.main.ViewportPointToRay (new Vector3(.5f, .5f, 0));

		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000f)) 
		{
			string strName = hit.collider.gameObject.name;

			int index = int.Parse (strName);

			setFocusFragment ((FRAGMENT_INDEX)index);
		}
	}

	private void setFocusFragment(FRAGMENT_INDEX index)
	{
		focusFragmentIndex = index;

		frameLine.SetActive (focusFragmentIndex != FRAGMENT_INDEX.NULL && !inRotating);

		btnRoot.SetActive (frameLine.activeSelf && !inRotating);

		if(index == FRAGMENT_INDEX.NULL) return;

		Vector3 pos = Vector3.zero;
		Vector3 scale = Vector3.zero;
		focusAxis = ROTATE_AXIS.X;
		int axisValue = 0;

		if (index == FRAGMENT_INDEX.FORWARD) {
			pos = new Vector3 (0, 0, -1.5f);
			scale = new Vector3 (3.1f, 3.1f, 0.2f);
			focusAxis = ROTATE_AXIS.Z;
			axisValue = -1;
		} else if (index == FRAGMENT_INDEX.BACK) {
			pos = new Vector3 (0, 0, 1.5f);
			scale = new Vector3 (3.1f, 3.1f, 0.2f);
			focusAxis = ROTATE_AXIS.Z;
			axisValue = 1;
		} else if (index == FRAGMENT_INDEX.LEFT) {
			pos = new Vector3 (-1.5f, 0, 0);
			scale = new Vector3 (0.2f, 3.1f, 3.1f);
			focusAxis = ROTATE_AXIS.X;
			axisValue = -1;
		} else if (index == FRAGMENT_INDEX.RIGHT) {
			pos = new Vector3 (1.5f, 0, 0);
			scale = new Vector3 (0.2f, 3.1f, 3.1f);
			focusAxis = ROTATE_AXIS.X;
			axisValue = 1;
		} else if (index == FRAGMENT_INDEX.TOP) {
			pos = new Vector3 (0, 1.5f, 0);
			scale = new Vector3 (3.1f, 0.2f, 3.1f);
			focusAxis = ROTATE_AXIS.Y;
			axisValue = 1;
		} else if (index == FRAGMENT_INDEX.BOTTOM) {
			pos = new Vector3 (0, -1.5f, 0);
			scale = new Vector3 (3.1f, 0.2f, 3.1f);
			focusAxis = ROTATE_AXIS.Y;
			axisValue = -1;
		}

		frameLine.transform.localPosition = pos;
		frameLine.transform.localScale = scale;

		focusCubeList = getCubeList (focusAxis, axisValue);
	}

	private List<MagicCubePiece> getCubeList(ROTATE_AXIS axis, int value)
	{
		List<MagicCubePiece> list = new List<MagicCubePiece> ();

		foreach(MagicCubePiece piece in pieceList)
		{
			if (piece.equalValue (axis, value)) {
				list.Add (piece);
			}
		}

		return list;
	}

	public void click(Vector2 viewPos)
	{
		bool debug = true;
		if (debug) return;

		if (inDrag) return;
		if (inInertia) return;
		if (focusFragmentIndex == FRAGMENT_INDEX.NULL) return;

		Ray ray = Camera.main.ViewportPointToRay (new Vector3(viewPos.x, viewPos.y, 0));

		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000f)) 
		{
			MagicCubePiece touchPiece = hit.collider.transform.parent.GetComponent<MagicCubePiece> ();

			if(focusCubeList.Contains(touchPiece))
			{
				ROTATE_AXIS axis = ROTATE_AXIS.X;
				int index = 0;
				ROTATE_DIR dir = ROTATE_DIR.NAGATIVE;

				int x = (int)touchPiece.transform.localPosition.x;
				int y = (int)touchPiece.transform.localPosition.y;
				int z = (int)touchPiece.transform.localPosition.z;

				int xyz = 1;// 1:yz 2:xz 3:xy

				if (focusAxis == ROTATE_AXIS.X) {
					xyz = 1;
					if (y == 0 && z == 0) return;
				} else if (focusAxis == ROTATE_AXIS.Y) {
					xyz = 2;
					if (x == 0 && z == 0) return;
				} else if (focusAxis == ROTATE_AXIS.Z) {
					xyz = 3;
					if (x == 0 && y == 0) return;
				}

				if (xyz == 1) {
					axis = ROTATE_AXIS.X;
					if (y < 0) {
						dir = ROTATE_DIR.POSITIVE;
					}
					index = (int)touchPiece.transform.localPosition.x;
				} else if (xyz == 2) {
					axis = ROTATE_AXIS.Y;
					index = (int)touchPiece.transform.localPosition.y;
				}else if(xyz == 3) {
					axis = ROTATE_AXIS.Z;
					index = (int)touchPiece.transform.localPosition.z;
				}

				StartCoroutine(rotateCubePiece (axis, index, dir));
			}
		}
	}

	private IEnumerator rotateCubePiece(ROTATE_AXIS axis, int index, ROTATE_DIR dir)
	{
		inRotating = true;

		frameLine.SetActive (false);

		btnRoot.SetActive (false);

		List<MagicCubePiece> list = getCubeList (axis, index);

		foreach(MagicCubePiece piece in list)
		{
			piece.transform.parent = universalAxis.transform;
		}

		Vector3 av = new Vector3 (0, 0, 0);
		if (axis == ROTATE_AXIS.X) {
			av.x = 1;
		} else if (axis == ROTATE_AXIS.Y) {
			av.y = 1;
		} else if (axis == ROTATE_AXIS.Z) {
			av.z = 1;
		}

		for(float i = 0; Mathf.Abs(i) < Mathf.Abs((float)dir); i += (float)dir / 30f)
		{
			universalAxis.transform.rotation *= Quaternion.AngleAxis ((float)dir / 30f, av);

			yield return new WaitForEndOfFrame ();
		}

		foreach(MagicCubePiece piece in list)
		{
			piece.transform.parent = transform;
			piece.refreshValue ((int)axis, (int)dir);
		}

		universalAxis.transform.localEulerAngles = Vector3.zero;

		inRotating = false;

		frameLine.SetActive (focusFragmentIndex != FRAGMENT_INDEX.NULL && !inRotating);

		btnRoot.SetActive (frameLine.activeSelf && !inRotating);
	}

	public void click_L_1_N()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, -1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, -1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, -1, ROTATE_DIR.NAGATIVE));
		}
	}

	public void click_L_1_P()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, -1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, -1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, -1, ROTATE_DIR.POSITIVE));
		}
	}

	public void click_L_2_N()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 0, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 0, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 0, ROTATE_DIR.NAGATIVE));
		}
	}

	public void click_L_2_P()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 0, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 0, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 0, ROTATE_DIR.POSITIVE));
		}
	}

	public void click_L_3_N()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 1, ROTATE_DIR.NAGATIVE));
		}
	}

	public void click_L_3_P()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.X, 1, ROTATE_DIR.POSITIVE));
		}
	}

	public void click_R_1_N()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, -1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, -1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, -1, ROTATE_DIR.NAGATIVE));
		}
	}

	public void click_R_1_P()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, -1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, -1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, -1, ROTATE_DIR.POSITIVE));
		}
	}

	public void click_R_2_N()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 0, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 0, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 0, ROTATE_DIR.NAGATIVE));
		}
	}

	public void click_R_2_P()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 0, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 0, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 0, ROTATE_DIR.POSITIVE));
		}
	}

	public void click_R_3_N()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 1, ROTATE_DIR.NAGATIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 1, ROTATE_DIR.NAGATIVE));
		}
	}

	public void click_R_3_P()
	{
		if (focusAxis == ROTATE_AXIS.X) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Y) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Z, 1, ROTATE_DIR.POSITIVE));
		} else if (focusAxis == ROTATE_AXIS.Z) {
			StartCoroutine (rotateCubePiece(ROTATE_AXIS.Y, 1, ROTATE_DIR.POSITIVE));
		}
	}

}
