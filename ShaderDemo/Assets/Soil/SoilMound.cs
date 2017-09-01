using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dust{

	public static float gravity = 10f;

	private int __x;
	private int __y;

	public Vector2 vec;

	public int x {
		set{
			__x = value;
			vec.x = value; 
		}
		get{ return __x;}
	}
	public int y{
		set{ 
			__y = value;
			vec.y = value;
		}
		get{return __y;}

	}
	public float support;
	public bool able = true;


	public Dust(int _x, int _y) {
		x = _x;
		y = _y;
		support = 0;
		able = true;
	}

}

public class SoilMound : MonoBehaviour 
{
	private const float radios = 10f;

	private Material mat;

	private Texture2D texHole;

	private List<Dust> dustList = new List<Dust> ();


	void Start () 
	{
		mat = gameObject.GetComponent<MeshRenderer> ().material;

		texHole = new Texture2D (128, 128, TextureFormat.RGBA32, false);//mat.GetTexture ("_HoleTex");

		for(int x = 0; x < texHole.width; x++)
		{
			for(int y = 0; y < texHole.height; y++)
			{

				texHole.SetPixel (x, y, Color.black);
			}
		}

		texHole.wrapMode = TextureWrapMode.Clamp;

		texHole.Apply ();

		mat.SetTexture ("_HoleTex", (Texture)texHole);

		dustList.Clear ();

		for(int x = 0; x < texHole.width; x++)
		{
			for(int y = 0; y < texHole.height; y++)
			{
				Dust dust = new Dust (x, y);
				dustList.Add (dust);
			}
		}
	}

	public void click()
	{
		float xPos, yPos;

		if (Input.touchCount > 0) 
		{
			xPos = Input.touches [0].position.x;
			yPos = Input.touches [0].position.y;
		}
		else
		{
			xPos = Input.mousePosition.x;
			yPos = Input.mousePosition.y;
		}

		Vector3 castPosition = Camera.main.ScreenToWorldPoint (new Vector3(xPos, yPos, 10));

		Vector3 castLocal = (castPosition - transform.position) + transform.localScale / 2;

		Vector2 local = new Vector2 (castLocal.x / transform.localScale.x, castLocal.y / transform.localScale.y);

		local = local * texHole.width;

		float time = Time.realtimeSinceStartup;

		foreach(Dust dust in dustList)
		{
			if (dust.x > local.x - radios && dust.x < local.x + radios && dust.y < local.y + radios && dust.y > local.y - radios) {
				if (Vector2.Distance (dust.vec, local) < radios) {
					dust.able = false;
				}
			}
		}

		float nowTime = Time.realtimeSinceStartup;

		Debug.Log ("Use Time: " + (nowTime - time));

		mat.SetTexture ("_HoleTex", (Texture)texHole);
	}

	private Dust getDustByPosition(int x, int y)
	{
		foreach(Dust dust in dustList)
		{
			if(dust.x == x && dust.y == y)
			{
				return dust;
			}
		}

		return null;
	}

	private void addSupport(Dust dust, float _sp)
	{
		if(dust != null)
		{
			dust.support += _sp;
		}
	}

	List<int> dropX = new List<int>();

	void Update()
	{
		dropX.Clear ();

		foreach (Dust dust in dustList) 
		{
			if (dropX.Contains (dust.x)) {
				dust.y--;

				continue;
			}

			if (dust.y > 0 && dust.able) {
				if( !dropX.Contains(dust.x))
				{
					bool e =  texHole.GetPixel (dust.x, dust.y - 1).r == 1;

					if (e) {
						dropX.Add (dust.x);
					}
				}
			}
		}

		for(int x = 0; x < texHole.width; x++)
		{
			for(int y = 0; y < texHole.height; y++)
			{

				texHole.SetPixel (x, y, Color.white);
			}
		}

		foreach(Dust dust in dustList)
		{
			if (dust.able) {
				texHole.SetPixel (dust.x, dust.y, Color.black);
			} else {
				texHole.SetPixel (dust.x, dust.y, Color.white);
			}
		}

		texHole.Apply();
	}

}
