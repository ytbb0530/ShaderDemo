using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ColorAIController : MonoBehaviour {
	
	public Camera rayCamera;
	public MeshFilter meshRoot;
	public Text textVertexCount;
	public List<Color> colorTemplates = new List<Color> ();
	public List<ColorSkinnedMeshRenderer> roles = new List<ColorSkinnedMeshRenderer> ();

	private int shootCount = 1;
	private int vertexCount = 0;

	public void shoot(GameObject src, int colorIndex)
	{
		rayCamera.transform.position = src.transform.position - src.transform.forward * .1f;
		rayCamera.transform.forward = src.transform.forward;
		rayCamera.orthographicSize = .2f + Random.value * .3f;

		Color vertexColor = colorTemplates[colorIndex];

		List<Vector3> vertices = new List<Vector3> ();
		List<Vector2> uvs = new List<Vector2> ();
		List<int> triangles = new List<int> ();
		List<Color> colors = new List<Color> ();
		List<int> indices = new List<int> ();
		List<Vector3> offsets = new List<Vector3> ();
		ColorSkinnedMeshRenderer skinned = null;

		for (float i = 0; i <= 1; i += .05f) {
			for (float j = 0; j <= 1; j += .05f) {
				bool ret = getRayCastVertex (i, j, vertices, uvs, indices, offsets, ref skinned);
				if (ret) {
					triangles.Add (vertices.Count - 3);
					triangles.Add (vertices.Count - 2);
					triangles.Add (vertices.Count - 1);

					colors.Add (vertexColor);
					colors.Add (vertexColor);
					colors.Add (vertexColor);
				}
			}
		}

		if (vertices.Count == 0) {
			return;
		}

		Mesh mesh = new Mesh ();
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0 );
		mesh.SetUVs (0, uvs);
		mesh.SetColors (colors);

		shootCount++;

		GameObject gc = new GameObject ();
		gc.transform.parent = meshRoot.transform;
		gc.name = "ColoredMesh_Hit";

		MeshFilter meshFilter = gc.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = mesh;

		if (skinned == null) {
			combine (meshFilter);
		} else {
			vertexCount += skinned.setValue (meshFilter, indices, offsets);
			textVertexCount.text = vertexCount + "";
		}
	}

	private bool getRayCastVertex(float u, float v, List<Vector3> vertices, List<Vector2> uvs, List<int> indices, List<Vector3> offsets, ref ColorSkinnedMeshRenderer skinned)
	{
		Vector2 pos = new Vector2 (Screen.width * u, Screen.height * v);
		Ray ray = rayCamera.ScreenPointToRay (pos);

		RaycastHit hit;
		bool ret = Physics.Raycast (ray, out hit, rayCamera.farClipPlane);

		if (!ret || hit.transform == null || hit.collider.GetType() != typeof(MeshCollider)) {
			return false;
		}

		MeshFilter filter = hit.transform.GetComponent<MeshFilter> ();
		ColorSkinnedMeshRenderer skin = filter == null ? hit.transform.GetComponentInChildren<ColorSkinnedMeshRenderer> () : null;

		Mesh mesh = filter == null ? skin.colliderMesh : filter.mesh;

		int vertexIndex_1 = mesh.triangles[hit.triangleIndex * 3];
		int vertexIndex_2 = mesh.triangles[hit.triangleIndex * 3 + 1];
		int vertexIndex_3 = mesh.triangles[hit.triangleIndex * 3 + 2];

		Vector3 offset_1;
		Vector3 offset_2;
		Vector3 offset_3;

		Vector3 vertex_1 = transferPoint (hit.transform, mesh.vertices [vertexIndex_1], out offset_1);
		Vector3 vertex_2 = transferPoint (hit.transform, mesh.vertices [vertexIndex_2], out offset_2);
		Vector3 vertex_3 = transferPoint (hit.transform, mesh.vertices [vertexIndex_3], out offset_3);

		bool b1 = false;
		bool b2 = false;
		bool b3 = false;

		foreach (Vector3 vec in vertices) {
			if (Vector3.Distance(vec, vertex_1) < 0.01f) {
				b1 = true;
			} else if (Vector3.Distance(vec, vertex_2) < 0.01f) {
				b2 = true;
			} else if (Vector3.Distance(vec, vertex_3) < 0.01f) {
				b3 = true;
			}
		}

		if (b1 && b2 && b3) {
			return false;
		}

		vertices.Add (vertex_1);
		vertices.Add (vertex_2);
		vertices.Add (vertex_3);

		uvs.Add(rayCamera.WorldToViewportPoint (vertex_1));
		uvs.Add(rayCamera.WorldToViewportPoint (vertex_2));
		uvs.Add(rayCamera.WorldToViewportPoint (vertex_3));

		indices.Add (vertexIndex_1);
		indices.Add (vertexIndex_2);
		indices.Add (vertexIndex_3);

		offsets.Add (offset_1);
		offsets.Add (offset_2);
		offsets.Add (offset_3);

		if (skinned == null && skin != null) {
			skinned = skin;
		}

		return true;
	}

	public void combine(MeshFilter childFilter)
	{
		Matrix4x4 matrix = meshRoot.transform.worldToLocalMatrix;

		CombineInstance selfCombine = new CombineInstance();
		selfCombine.mesh = meshRoot.mesh;
		selfCombine.transform = meshRoot.transform.localToWorldMatrix * matrix;

		CombineInstance childCombine = new CombineInstance();
		childCombine.mesh = childFilter.mesh;
		childCombine.transform = childFilter.transform.localToWorldMatrix * matrix;
		DestroyObject(childFilter.gameObject);

		Mesh mesh = new Mesh ();
		mesh.name = "Combined";

		mesh.CombineMeshes (new CombineInstance[]{selfCombine, childCombine}, true, true);
		meshRoot.mesh = mesh;
		mesh.MarkDynamic ();

		vertexCount += mesh.vertexCount;
		textVertexCount.text = vertexCount + "";
	}

	public void clear()
	{
		foreach(ColorSkinnedMeshRenderer csmr in roles) {
			csmr.clear ();
		}
		meshRoot.sharedMesh = null;
		vertexCount = 0;

		textVertexCount.text = vertexCount + "";
	}

	private Vector3 transferPoint(Transform parent, Vector3 localPos, out Vector3 _offset){
		Vector3 offset = rayCamera.transform.forward * 0.001f * shootCount;
		_offset = offset;

		Vector3 dst = parent.TransformPoint (localPos);
		dst -= offset;

		return dst;
	}

}