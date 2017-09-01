using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class WaterPiece
{
	public Vector3 pos;
	public int index;
	public float speed = 0;

	private float g = .005f;

	public WaterPiece(int _index, Vector3 _pos)
	{
		index = _index;
		pos = _pos;
	}

	public void addSpeed(float _speed)
	{
		speed = speed * .4f + _speed;
		speed = speed > .1f ? .1f : speed;
		speed = speed < -.1f ? -.1f : speed;
	}

	public void updatePos()
	{
		float ly = pos.y;

		pos.y += speed;

		if((ly > 0 && pos.y < 0) || (ly < 0 && pos.y > 0))
		{
			if (Mathf.Abs(speed) < .02f )
			{
				speed = 0;
				pos.y = 0;
				return;
			}

			speed = speed / 2;
			return;
		}

		if (pos.y > 0) speed -= g;
		else if(pos.y < 0) speed += g;
	}
}

public class WaterSurface : MonoBehaviour 
{
	public GameObject waterMesh;

	private Dictionary<int, WaterPiece> dictWaterPiece = new Dictionary<int, WaterPiece> ();
	private Dictionary<int, Mesh> dictMesh = new Dictionary<int, Mesh> ();
	private LineRenderer line;
	private float lineStep = .2f;
	private float[] heavyRate = {.5f, .35f, .15f};

	void Start ()
	{
		dictWaterPiece.Clear ();
		dictMesh.Clear ();

		line = GetComponent<LineRenderer> ();

		int count = (int)(15 / lineStep);

		line.SetVertexCount (count);

		float x = -lineStep * count / 2;

		for(int i = 0; i < count; i++)
		{
			WaterPiece wp = new WaterPiece (i, new Vector3(x, 0, 0));
			dictWaterPiece.Add (i, wp);
			line.SetPosition (i, wp.pos);
			x += lineStep;

			Vector3[] Vertices = new Vector3[4];
			Vertices[0] = new Vector3(x - lineStep, 0, 0);
            Vertices[1] = new Vector3(x, 0, 0);
			Vertices[2] = new Vector3(x - lineStep, -5, 0);
            Vertices[3] = new Vector3(x, -5, 0);

			Vector2[] Uvs = new Vector2[4];
			Uvs [0] = new Vector2 (0, 1);
			Uvs [1] = new Vector2 (1, 1);
			Uvs [2] = new Vector2 (0, 0);
			Uvs [3] = new Vector2 (1, 0);

			int[] tris = new int[6]{0, 1, 3, 3, 2, 0};

			GameObject gc = Instantiate (waterMesh, transform) as GameObject;
			MeshFilter mf = gc.GetComponent<MeshFilter> ();
			mf.mesh = new Mesh ();
			mf.mesh.vertices = Vertices;
			mf.mesh.uv = Uvs;
			mf.mesh.triangles = tris;

			dictMesh.Add (i, mf.mesh);
		}
	}
	
	void Update () 
	{
		foreach (WaterPiece wp in dictWaterPiece.Values) {
			wp.updatePos ();
		}

		Dictionary<int, Vector3> dictRes = new Dictionary<int, Vector3> ();

		foreach(WaterPiece wp in dictWaterPiece.Values) 
		{
			Vector3 result = new Vector3(wp.pos.x, 0, 0);

			for(int i = -2; i <= 2; i++)
			{
				int index = wp.index + i;

				if(dictWaterPiece.ContainsKey(index))
				{
					float rate = heavyRate[Mathf.Abs(i)];
					result.y += dictWaterPiece [index].pos.y * rate;
				}
			}

			line.SetPosition (wp.index, result);

			dictRes.Add (wp.index, result);
		}

		foreach(int index in dictRes.Keys)
		{
			Vector3 result = dictRes [index];

			if(dictRes.ContainsKey(index + 1))
			{
				Vector3 nextPos = dictRes[index + 1];

				Vector3[] Vertices = new Vector3[4];
				Vertices[0] = new Vector3(result.x, result.y, 0);
				Vertices[1] = new Vector3(nextPos.x, nextPos.y, 0);
				Vertices[2] = new Vector3(result.x, -5, 0);
				Vertices[3] = new Vector3(nextPos.x, -5, 0);

				Mesh mesh = dictMesh [index];
				mesh.vertices = Vertices;
			}
		}
	}

	public void setRipple(Vector3 position)
	{
		float length = 100;
		WaterPiece focus = null;

		foreach(WaterPiece wp in dictWaterPiece.Values)
		{
			float l = Mathf.Abs (wp.pos.x - position.x);

			if(l < length)
			{
				length = l;
				focus = wp;
			}
		}

		if (focus == null) return;

		float force = -.1f;

		if(position.y < focus.pos.y)
		{
			force *= -1;
		}

		StartCoroutine (wave(focus, force));
	}

	IEnumerator wave(WaterPiece focus, float force)
	{
		focus.addSpeed (force);

		for(int i = 1; ; i++)
		{
			yield return new WaitForEndOfFrame ();

			float reduction = 1 - (i - 1) / 100f;
			reduction = reduction < 0 ? 0 : reduction;

			int index_1 = focus.index - i;
			WaterPiece neighbor_1;
			dictWaterPiece.TryGetValue (index_1, out neighbor_1);
			if (neighbor_1 != null){
				neighbor_1.addSpeed (force * reduction);
			}

			int index_2 = focus.index + i;
			WaterPiece neighbor_2;
			dictWaterPiece.TryGetValue (index_2, out neighbor_2);
			if (neighbor_2 != null) {
				neighbor_2.addSpeed (force * reduction);
			}

			if(neighbor_1 == null && neighbor_2 == null){
				break;
			}
		}
	}

}
