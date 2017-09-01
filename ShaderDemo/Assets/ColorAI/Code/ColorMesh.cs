using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ColorMesh : MonoBehaviour 
{
	private MeshFilter filter;
	private List<int> indices;
	private List<Vector3> offsets;
	private List<Vector3> tempVec;
	private Material mat;
	private List<Vector3> rootVertex = new List<Vector3>();

	void Start()
	{
		filter = GetComponent<MeshFilter> ();
		indices = new List<int> ();
		offsets = new List<Vector3> ();
		tempVec = new List<Vector3> ();
		mat = GetComponent<MeshRenderer> ().material;
	}

	public void clear()
	{
		filter.sharedMesh = null;
		indices.Clear ();
		offsets.Clear ();
	}

	public int setValue(MeshFilter newFilter, List<int> _indices, List<Vector3> _offsets)
	{
		indices.AddRange(_indices);
		offsets.AddRange (_offsets);
		combine (newFilter);

		return filter.sharedMesh.vertexCount;
	}

	private void combine(MeshFilter childFilter)
	{
		Matrix4x4 matrix = transform.worldToLocalMatrix;

		CombineInstance selfCombine = new CombineInstance();
		selfCombine.mesh = filter.sharedMesh;
		selfCombine.transform = transform.localToWorldMatrix * matrix;

		CombineInstance childCombine = new CombineInstance();
		childCombine.mesh = childFilter.mesh;
		childCombine.transform = childFilter.transform.localToWorldMatrix * matrix;
		DestroyObject(childFilter.gameObject);

		Mesh mesh = new Mesh ();
		mesh.name = "Combined";

		mesh.CombineMeshes (new CombineInstance[]{selfCombine, childCombine}, true, true);
		filter.sharedMesh = mesh;
		mesh.MarkDynamic ();
	}

	public void refreshMesh(Mesh rootMesh)
	{
		if (filter.sharedMesh == null) return;

		tempVec.Clear ();
		rootMesh.GetVertices (rootVertex);
		for(int i = 0; i < filter.sharedMesh.vertexCount; i++){
			tempVec.Add(rootVertex [indices [i]] + offsets[i]);
		}
		filter.sharedMesh.SetVertices (tempVec);
	}

}