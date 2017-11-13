using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class RealtimeVideo : MonoBehaviour 
{
	public Material mat;
	public Text textWH;
	public Slider slider;
	public RawImage image;

	private VideoPlayer vplayer;
	private int mx;
	private int my;


	void Start()
	{
		slider.minValue = .1f;
		slider.maxValue = .4f;
		slider.value = .25f;
		vplayer = GetComponent<VideoPlayer> ();
		valueChange ();
	}

	public void createMesh () 
	{
		slider.transform.parent.gameObject.SetActive (false);
		RenderTexture renderTex = new RenderTexture (mx, my, 1);
		vplayer.targetTexture = renderTex;
		image.texture = renderTex;

		int w = renderTex.width;
		int h = renderTex.height;

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int> ();
		List<Vector2> uv_0 = new List<Vector2> ();
		List<Vector2> uv_1 = new List<Vector2> ();

		for (int x = 0; x < w - 1; x++) {
			for (int y = 0; y < h - 1; y++) {

				Vector3 p0 = new Vector3 (x, y, 0);
				Vector3 p1 = new Vector3 (x, y+1, 0);
				Vector3 p2 = new Vector3 (x+1, y+1, 0);
				Vector3 p3 = new Vector3 (x+1, y, 0);

				vertices.Add (p0 + new Vector3(-(w-1)/2f, -(h-1)/2f, 0));
				vertices.Add (p1 + new Vector3(-(w-1)/2f, -(h-1)/2f, 0));
				vertices.Add (p2 + new Vector3(-(w-1)/2f, -(h-1)/2f, 0));
				vertices.Add (p3 + new Vector3(-(w-1)/2f, -(h-1)/2f, 0));

				triangles.Add (vertices.Count - 4);
				triangles.Add (vertices.Count - 3);
				triangles.Add (vertices.Count - 2);
				triangles.Add (vertices.Count - 1);

				Vector2 uv = new Vector2 (((p0.x+p3.x)/2)/w, ((p0.y+p1.y)/2)/h);

				uv_0.Add (uv);
				uv_0.Add (uv);
				uv_0.Add (uv);
				uv_0.Add (uv);

				uv_1.Add (new Vector2(0, 0));
				uv_1.Add (new Vector2(0, 1));
				uv_1.Add (new Vector2(1, 1));
				uv_1.Add (new Vector2(1, 0));
			}
		}

		Mesh mesh = new Mesh ();
		mesh.Clear ();
		mesh.SetVertices(vertices);
		mesh.SetIndices(triangles.ToArray(), MeshTopology.Quads, 0 );
		mesh.SetUVs (0, uv_0);
		mesh.SetUVs (1, uv_1);

		GameObject gc = new GameObject ();

		MeshFilter meshFilter = gc.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = mesh;

		MeshRenderer meshRender = gc.AddComponent<MeshRenderer> ();
		meshRender.material = mat;
		meshRender.material.SetTexture ("_MainTex", renderTex);

		float camSize = h / 2;
		if (w * 1f / h > Camera.main.aspect) {
			camSize *= (w * 1f / h) - Camera.main.aspect + 1;
		}
		Camera.main.orthographicSize = (int)camSize + 4;

		vplayer.Play ();
	}

	public void PR()
	{
		if (vplayer.isPlaying) {
			vplayer.Pause ();
		} else {
			vplayer.Play ();
		}
	}

	public void valueChange () 
	{
		mx = (int)(426 * slider.value);
		my = (int)(240 * slider.value);
		textWH.text = mx + "/" + my;
	}
}
