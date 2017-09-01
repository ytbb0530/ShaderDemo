using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class TexAnim : MonoBehaviour 
{
	public GameObject template;
	public Text textCount;
	public List<Texture> anims = new List<Texture>();

	private bool inCreate;
	private int count = 0;
	private int width = 50;
	private int height = 10;
	private List<Material> mats = new List<Material>();

	void Start () 
	{
		inCreate = false;
		template.SetActive (false);

		mats.Clear ();
		Material matTemple = template.GetComponent<MeshRenderer> ().sharedMaterial;
		foreach (Texture tex in anims) {
			Material mat = new Material(Shader.Find("Unlit/TexAnim"));
			mat.SetTexture ("_MainTex", matTemple.GetTexture("_MainTex"));
			mat.SetTexture ("_AnimTex", tex);
			mat.enableInstancing = true;
			mats.Add (mat);
		}
	}

	void Update () 
	{
		if (!inCreate) return;

		int c = (int)(count * .1f);
		c = c < 1 ? 1 : c;
		c = c > 100 ? 100 : c;
		for (int i = 0; i < c; i++) {
			createRole ();
		}
	}

	void createRole () 
	{
		int id = ((int)(Random.value * 1000)) % anims.Count;

		GameObject gc = Instantiate (template) as GameObject;
		gc.name = template.name + "_" + id;
		gc.SetActive (true);
		gc.transform.parent = template.transform.parent;
		gc.transform.position = new Vector3 ((count % width - width / 2)*2, 0, (count / width)*2);
		gc.GetComponent<MeshRenderer> ().material = mats[id];

		count++;
		textCount.text = "" + count;
	}

	public void startCreting()
	{
		inCreate = true;
	}

	public void stopCreating()
	{
		inCreate = false;
	}

}
