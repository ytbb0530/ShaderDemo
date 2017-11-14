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
	public List<VideoClip> clips = new List<VideoClip> ();

	private VideoPlayer vplayer;
	private MeshRenderer meshRender;
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

		int sw = (int)(mx * .15f);
		int sh = (int)(my * .15f);

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int> ();
		List<Vector2> uv_0 = new List<Vector2> ();
		List<Vector2> uv_1 = new List<Vector2> ();
		List<Color> colors = new List<Color> ();

		for (int x = 0; x < mx - 1; x++) {
			for (int y = 0; y < my - 1; y++) {

				Vector3 p0 = new Vector3 (x, y, 0);
				Vector3 p1 = new Vector3 (x, y+1, 0);
				Vector3 p2 = new Vector3 (x+1, y+1, 0);
				Vector3 p3 = new Vector3 (x+1, y, 0);

				vertices.Add (p0 + new Vector3(-(mx-1)/2f, -(my-1)/2f, 0));
				vertices.Add (p1 + new Vector3(-(mx-1)/2f, -(my-1)/2f, 0));
				vertices.Add (p2 + new Vector3(-(mx-1)/2f, -(my-1)/2f, 0));
				vertices.Add (p3 + new Vector3(-(mx-1)/2f, -(my-1)/2f, 0));

				triangles.Add (vertices.Count - 4);
				triangles.Add (vertices.Count - 3);
				triangles.Add (vertices.Count - 2);
				triangles.Add (vertices.Count - 1);

				Vector2 uv = new Vector2 (((p0.x+p3.x)/2)/mx, ((p0.y+p1.y)/2)/my);

				uv_0.Add (uv);
				uv_0.Add (uv);
				uv_0.Add (uv);
				uv_0.Add (uv);

				if (p3.x <= sw && p1.y <= sh) {
					uv_1.Add (new Vector2 (p0.x / sw, p0.y / sh));
					uv_1.Add (new Vector2 (p1.x / sw, p1.y / sh));
					uv_1.Add (new Vector2 (p2.x / sw, p2.y / sh));
					uv_1.Add (new Vector2 (p3.x / sw, p3.y / sh));

					colors.Add (new Color(1, 0, 0));
					colors.Add (new Color(1, 0, 0));
					colors.Add (new Color(1, 0, 0));
					colors.Add (new Color(1, 0, 0));
				} else {
					uv_1.Add (new Vector2(0, 0));
					uv_1.Add (new Vector2(0, 1));
					uv_1.Add (new Vector2(1, 1));
					uv_1.Add (new Vector2(1, 0));

					colors.Add (new Color(0, 0, 0));
					colors.Add (new Color(0, 0, 0));
					colors.Add (new Color(0, 0, 0));
					colors.Add (new Color(0, 0, 0));
				}
			}
		}

		Mesh mesh = new Mesh ();
		mesh.Clear ();
		mesh.SetVertices(vertices);
		mesh.SetIndices(triangles.ToArray(), MeshTopology.Quads, 0 );
		mesh.SetUVs (0, uv_0);
		mesh.SetUVs (1, uv_1);
		mesh.SetColors (colors);

		GameObject gc = new GameObject ();

		MeshFilter meshFilter = gc.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = mesh;

		meshRender = gc.AddComponent<MeshRenderer> ();
		meshRender.material = mat;

		float camSize = my / 2;
		if (mx * 1f / my > Camera.main.aspect) {
			camSize *= (mx * 1f / my) - Camera.main.aspect + 1;
		}
		Camera.main.orthographicSize = (int)camSize + 1;

		vplayer.renderMode = VideoRenderMode.MaterialOverride;
		vplayer.targetMaterialRenderer = meshRender;
		vplayer.targetMaterialProperty = "_MainTex";
		vplayer.loopPointReached += EndReached;

		playVideo ();
	}

	void EndReached(UnityEngine.Video.VideoPlayer vp)
	{
		vp.Stop ();
		playVideo ();
	}

	private void playVideo()
	{
		vplayer.clip = clips[Random.Range(0, clips.Count)];
		vplayer.SetTargetAudioSource (0, vplayer.GetComponent<AudioSource>());
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

	public void CF()
	{
		int cf = meshRender.material.GetInt ("_Colorful");
		if (cf == 1) {
			meshRender.material.SetInt ("_Colorful", 0);
		} else {
			meshRender.material.SetInt ("_Colorful", 1);
		}
	}

	public void valueChange () 
	{
		mx = (int)(426 * slider.value);
		my = (int)(240 * slider.value);
		textWH.text = mx + "/" + my;
	}

}