using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiriWave : MonoBehaviour 
{
	public Shader shader;
	public int index;
	public Color color;
	public float startPos;
	public float endPos;
	public float waveHeight;
	public float waveLength;
	public float waveTime;

	[HideInInspector] public bool inUse;

	private const int length = 200;
	private const float step = .1f;
	private const float height = .02f;

	private Material mat;
	private float tempTime;

	public void init () {
		float offsetx = -length * step / 2;
		List<Vector3> verteices = new List<Vector3>();
		int[] indices = new int[length * 6];

		for (int i = 0; i < length; i++) {
			verteices.Add (new Vector3(i * step + 0 + offsetx, -height/2, 0));
			verteices.Add (new Vector3(i * step + 0 + offsetx, height/2, 0));
			verteices.Add (new Vector3(i * step + step + offsetx, height/2, 0));
			verteices.Add (new Vector3(i * step + step + offsetx, -height/2, 0));

			indices [0 + i * 6] = 0 + i * 4;
			indices [1 + i * 6] = 1 + i * 4;
			indices [2 + i * 6] = 2 + i * 4;
			indices [3 + i * 6] = 0 + i * 4;
			indices [4 + i * 6] = 2 + i * 4;
			indices [5 + i * 6] = 3 + i * 4;
		}

		Mesh mesh = new Mesh ();
		mesh.Clear();
		mesh.SetVertices(verteices);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0 );

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		mat = new Material (shader);

		MeshRenderer meshRender = gameObject.AddComponent<MeshRenderer>();
		meshRender.material = mat;

		inUse = false;
	}

	public void show(float height, Color _color, float localPos)
	{
		inUse = true;

		float l = length * step * .02f;
		startPos = localPos * length * step + Random.value * l - l / 2;
		endPos = startPos + Random.value * 4f - 2f;

		waveHeight = Mathf.Sqrt(height) / 2f;
		waveLength = Random.value * .8f + .3f;
		waveTime = Random.value * 2f + .6f;
		tempTime = Time.time;
		color = _color;
	}
	
	void Update () {
		if (inUse) {
			float temp = Time.time - tempTime;
			float timeRate = temp / waveTime;

			float height = 0;

			if (timeRate < .5f) {
				height = waveHeight * timeRate;
			} else {
				height = waveHeight * (1 - timeRate); 
			}

			float pos = Mathf.Lerp (startPos, endPos, timeRate);

			mat.SetInt ("_Index", index);
			mat.SetColor ("_Color", color);
			mat.SetFloat ("_Pos", pos);
			mat.SetFloat ("_Height", height);
			mat.SetFloat ("_WaveLength", waveLength);

			if (timeRate > .9f) {
				waveHeight = 0;
				inUse = false;
			}
		} else {
			mat.SetFloat ("_Height", 0);
		}
	}

}
