using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

public class TexAnimCheckout : MonoBehaviour 
{
	public Animator anim;

	private SkinnedMeshRenderer skin;
	private Mesh skinMesh;
	private Dictionary<string, List<List<Vector3>>> data = new Dictionary<string, List<List<Vector3>>>();
	private int doneAnimCount;


	void Start () {
		data.Clear ();
		doneAnimCount = 0;
		anim.enabled = false;
		skin = anim.gameObject.GetComponentInChildren<SkinnedMeshRenderer> ();
		skinMesh = Instantiate(skin.sharedMesh) as Mesh;

		StartCoroutine (record());
	}

	IEnumerator record(){
		yield return new WaitForSeconds (2);
		anim.enabled = true;

		while(anim.enabled){
			yield return new WaitForEndOfFrame ();
			recordUpdate ();
		}
	}

//	void Update () 
	void recordUpdate () 
	{
		if (!anim.enabled) return;

		string playing = anim.GetCurrentAnimatorClipInfo (0)[0].clip.name;

		List<List<Vector3>> frameData;
		bool contains = data.ContainsKey (playing);
		if (!contains) {
			frameData = new List<List<Vector3>> ();
			data.Add (playing, frameData);
		} else {
			frameData = data[playing];
		}

		skin.BakeMesh (skinMesh);
		List<Vector3> vertexData = new List<Vector3> ();
		foreach (Vector3 pos in skinMesh.vertices) {
			vertexData.Add (pos);
		}

		frameData.Add (vertexData);

		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
		// 判断动画是否播放完成
		if (info.normalizedTime >= 1.0f)
		{
			doneAnimCount++;

			if (doneAnimCount >= anim.GetCurrentAnimatorClipInfo (0).Length) {
				stop ();
			}
		}
	}

	void stop() {
		anim.enabled = false;

		Debug.Log ("data.Count: " + data.Count);
		foreach (string datakey in data.Keys) {
			writePic (datakey);
		}

		AssetDatabase.Refresh ();
	}

	private void writePic(string key){
		List<List<Vector3>> frameData = data [key];

		int width = frameData[0].Count;
		int height = frameData.Count;
		Texture2D tex = new Texture2D (width, height, TextureFormat.RGBAHalf, false);
		Debug.Log ("texture size: " + width + " - " + height);

		for (int frame = 0; frame < frameData.Count; frame++) {
			List<Vector3> vertexData = frameData [frame];

			for (int vertexId = 0; vertexId < vertexData.Count; vertexId++) {
				Vector3 pos = vertexData [vertexId];

//				if (vertexId == 1045) {
//					Debug.Log ("pos: " + pos);
//				}

				tex.SetPixel (vertexId, frame, new Color(pos.x, pos.y, pos.z));
			}
		}
		tex.Apply ();

		AssetDatabase.CreateAsset(tex, "Assets/TextureAnimation/"+key+".animTex.asset");
	}

}
