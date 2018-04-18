using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCutable : MonoBehaviour 
{
	private MeshFilter meshFilter;
	private ShapeMesh shapeMesh;

	void Start () 
	{
		meshFilter = GetComponent<MeshFilter> ();
		shapeMesh = new ShapeMesh (meshFilter.mesh);
	}

	public void cutWithPlane (Vector3 worldP1, Vector3 worldP2, Vector3 worldP3)
	{
		Plane plane = new Plane (worldP1, worldP2, worldP3);

		// TODO 没有切到模型的粗粒度剔除

		List<ShapeTriangle> triangleList_1 = new List<ShapeTriangle> ();
		List<ShapeTriangle> triangleList_2 = new List<ShapeTriangle> ();

		for (int i = 0; i < shapeMesh.triangles.Count; i++) {
			ShapeTriangle tri = shapeMesh.triangles [i];

			ShapeTriangle tri_11 = new ShapeTriangle ();
			ShapeTriangle tri_12 = new ShapeTriangle ();
			ShapeTriangle tri_21 = new ShapeTriangle ();
			ShapeTriangle tri_22 = new ShapeTriangle ();

			ShapeVertex p1 = tri.vertices [0];
			ShapeVertex p2 = tri.vertices [1];
			ShapeVertex p3 = tri.vertices [2];

			Vector3 wp1 = transform.TransformPoint (p1.position);
			Vector3 wp2 = transform.TransformPoint (p2.position);
			Vector3 wp3 = transform.TransformPoint (p3.position);

			int side1 = plane.GetSide (wp1) ? 1 : -1;
			int side2 = plane.GetSide (wp2) ? 1 : -1;
			int side3 = plane.GetSide (wp3) ? 1 : -1;

//			float d1 = plane.GetDistanceToPoint (wp1);
//			float d2 = plane.GetDistanceToPoint (wp2);
//			float d3 = plane.GetDistanceToPoint (wp3);
//			int side1 = Mathf.Abs(d1) < .1f ? 0 : (d1 > 0 ? 1 : -1);
//			int side2 = Mathf.Abs(d2) < .1f ? 0 : (d2 > 0 ? 1 : -1);
//			int side3 = Mathf.Abs(d3) < .1f ? 0 : (d3 > 0 ? 1 : -1);

			int cross = 0;
			ShapeVertex p12 = null;
			ShapeVertex p13 = null;
			ShapeVertex p23 = null;
			if (side1 != side2) { p12 = getNewPoint (plane, p1, p2, side1, side2); cross++; }
			if (side1 != side3) { p13 = getNewPoint (plane, p1, p3, side1, side3); cross++; }
			if (side2 != side3) { p23 = getNewPoint (plane, p2, p3, side2, side3); cross++; }

			if (Mathf.Abs (side1) + Mathf.Abs (side2) + Mathf.Abs (side3) <= 1 || cross < 2) {
				if (side1 + side2 + side3 > 0) {
					tri_11.InitWithVertex (p1, p2, p3);
				} else {
					tri_21.InitWithVertex (p1, p2, p3);
				}
			} else if (cross == 2) {
				if (p13 != null && p23 != null) {
					if (side3 == 1) {
						tri_11.InitWithVertex (p3, p13, p23);
						tri_21.InitWithVertex (p1, p2, p13);
						tri_22.InitWithVertex (p2, p23, p13);
					} else {
						tri_21.InitWithVertex (p3, p13, p23);
						tri_11.InitWithVertex (p1, p2, p13);
						tri_12.InitWithVertex (p2, p23, p13);
					}
				} else if (p12 != null && p13 != null) {
					if (side1 == 1) {
						tri_11.InitWithVertex (p1, p12, p13);
						tri_21.InitWithVertex (p2, p3, p12);
						tri_22.InitWithVertex (p3, p13, p12);	
					} else {
						tri_21.InitWithVertex (p1, p12, p13);
						tri_11.InitWithVertex (p2, p3, p12);
						tri_12.InitWithVertex (p3, p13, p12);	
					}
				} else if (p12 != null && p23 != null) {
					if (side2 == 1) {
						tri_11.InitWithVertex (p2, p23, p12);
						tri_21.InitWithVertex (p3, p1, p23);
						tri_22.InitWithVertex (p1, p12, p23);
					} else {
						tri_21.InitWithVertex (p2, p23, p12);
						tri_11.InitWithVertex (p3, p1, p23);
						tri_12.InitWithVertex (p1, p12, p23);
					}
				}
			} else if (cross == 3) {
				if (side1 == 0) {
					if (side2 == 1) {
						tri_11.InitWithVertex (p1, p2, p23);
						tri_21.InitWithVertex (p3, p1, p23);
					} else {
						tri_21.InitWithVertex (p1, p2, p23);
						tri_11.InitWithVertex (p3, p1, p23);
					}
				} else if (side2 == 0) {
					if (side3 == 1) {
						tri_11.InitWithVertex (p2, p3, p13);
						tri_21.InitWithVertex (p1, p2, p13);
					} else {
						tri_21.InitWithVertex (p2, p3, p13);
						tri_11.InitWithVertex (p1, p2, p13);
					}
				} else if (side3 == 0) {
					if (side1 == 1) {
						tri_11.InitWithVertex (p3, p1, p12);
						tri_21.InitWithVertex (p2, p3, p12);
					} else {
						tri_21.InitWithVertex (p3, p1, p12);
						tri_11.InitWithVertex (p2, p3, p12);
					}
				}
			}

			if (tri_11.valid) triangleList_1.Add (tri_11);
			if (tri_12.valid) triangleList_1.Add (tri_12);
			if (tri_21.valid) triangleList_2.Add (tri_21);
			if (tri_22.valid) triangleList_2.Add (tri_22);
		}

		createGameObjectByTriangles (triangleList_1);
		createGameObjectByTriangles (triangleList_2);
		GameObject.Destroy (gameObject);
	}

	private ShapeVertex getNewPoint (Plane plane, ShapeVertex p1, ShapeVertex p2, int side1, int side2)
	{
		Vector3 pos = Vector3.zero;
		Vector2 uv = Vector2.zero;
		if (side1 == 0) {
			pos = p2.position;
			uv = p2.uv;
		} else if (side2 == 0) {
			pos = p1.position;
			uv = p1.uv;
		} else {
			Vector3 pos1 = transform.TransformPoint (p1.position);
			Vector3 pos2 = transform.TransformPoint (p2.position);

			Vector3 shadow1 = plane.ClosestPointOnPlane (pos1);
			Vector3 shadow2 = plane.ClosestPointOnPlane (pos2);

			float d1 = Vector3.Distance (pos1, shadow1);
			float d2 = Vector3.Distance (pos2, shadow2);

			float rate = d1 / (d1 + d2);
			pos = Vector3.Lerp (p1.position, p2.position, rate);
			uv = Vector2.Lerp (p1.uv, p2.uv, rate);
		}
		ShapeVertex value = new ShapeVertex (0, pos, null);
		value.uv = uv;
		return value;
	}

	private void createGameObjectByTriangles (List<ShapeTriangle> triangleList)
	{
		if (triangleList.Count == 0) return;

		ShapeMesh sm = new ShapeMesh (triangleList);
		GameObject obj = GameObject.Instantiate (gameObject, transform.parent);
		MeshFilter filter = obj.GetComponent<MeshFilter> ();
		filter.sharedMesh = sm.mesh;
	}

}