using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ColorSkinnedMeshRenderer : MonoBehaviour 
{
	[HideInInspector] public Mesh colliderMesh;

	private ColorMesh colorMesh;
	private SkinnedMeshRenderer skinnedMeshRenderer;
	private MeshCollider meshCollider;

	void Start()
	{
		colorMesh = GetComponentInChildren<ColorMesh> ();
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

		colliderMesh = Instantiate(skinnedMeshRenderer.sharedMesh) as Mesh;
		meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = colliderMesh;
	}

	public int setValue(MeshFilter newFilter, List<int> indices, List<Vector3> offsets)
	{
		return colorMesh.setValue (newFilter, indices, offsets);
	}

	public void clear()
	{
		colorMesh.clear ();
	}

	void Update ()
	{
		skinnedMeshRenderer.BakeMesh(colliderMesh);
		meshCollider.enabled = false;
		meshCollider.enabled = true;

		colorMesh.refreshMesh (colliderMesh);
	}

}
