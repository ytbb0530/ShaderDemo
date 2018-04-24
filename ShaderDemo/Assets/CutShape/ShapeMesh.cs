using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeVertex
{
	public int index;
	public Vector3 position;
	public ShapeMesh mesh;
	public Vector2 uv;
	public List<ShapeTriangle> parentTriangles = new List<ShapeTriangle> ();

	public ShapeVertex ()
	{
	
	}

	public ShapeVertex (int index, Vector3 position, ShapeMesh mesh)
	{
		this.index = index;
		this.position = position;
		this.mesh = mesh;
	}

	public void AddTriangle (ShapeTriangle triangle)
	{
		bool has = false;
		foreach (ShapeTriangle tri in parentTriangles) {
			if (tri == triangle) {
				has = true;
				break;
			}
		}
		if (!has) {
			parentTriangles.Add (triangle);
		}
	}

	public static bool operator == (ShapeVertex a, ShapeVertex b)
	{
		if (object.Equals(a, null) && object.Equals(b, null)) return true; 
		if (object.Equals(a, null) || object.Equals(b, null)) return false; 
		return a.Equals (b);
	}
	public static bool operator != (ShapeVertex a, ShapeVertex b)
	{
		return !(a == b);
	}
	public override bool Equals(object obj)  
	{  
//		return position == ((ShapeVertex)obj).position;
		return index == ((ShapeVertex)obj).index && position == ((ShapeVertex)obj).position;
	} 
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

}

public class ShapeTriangle
{
	public int index;
	public ShapeMesh mesh;
	public ShapeVertex[] vertices = new ShapeVertex[3];
	public int tag;
	public bool valid{get{ return vertices [0] != null && vertices [1] != null && vertices [2] != null;}}

	public ShapeTriangle ()
	{
		
	}

	public ShapeTriangle(int index, ShapeMesh mesh, ShapeVertex v1, ShapeVertex v2, ShapeVertex v3)
	{
		this.index = index;
		this.mesh = mesh;
		this.tag = 0;
		vertices = new ShapeVertex[3]{ v1, v2, v3};
	}

	public void InitWithVertex(ShapeVertex v1, ShapeVertex v2, ShapeVertex v3)
	{
		vertices = new ShapeVertex[3]{ v1, v2, v3};
	}

	public bool Border (ShapeTriangle other)
	{
		foreach (ShapeVertex ver in vertices) {
			foreach (ShapeVertex otherVer in other.vertices) {
				if (ver == otherVer) {
					return true;
				}
			}
		}
		return false;
	}

	public static bool operator == (ShapeTriangle a, ShapeTriangle b)
	{
		if (object.Equals(a, null) && object.Equals(b, null)) return true; 
		if (object.Equals(a, null) || object.Equals(b, null)) return false; 

		return a.Equals (b);
	}
	public static bool operator != (ShapeTriangle a, ShapeTriangle b)
	{
		return !(a == b);
	}
	public override bool Equals(object obj)  
	{
		ShapeTriangle b = (ShapeTriangle)obj;
		if (index != b.index) return false;
		return vertices[0]==b.vertices[0] && vertices[1]==b.vertices[1] && vertices[2]==b.vertices[2];
	}  
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
}

public class ShapeMesh
{
	public string tag{ private set; get; }
	public Mesh mesh;
	public List<ShapeVertex> vertices = new List<ShapeVertex> ();
	public List<ShapeTriangle> triangles = new List<ShapeTriangle> ();

	public ShapeMesh(Mesh _mesh)
	{
		InitByMesh (_mesh);
	}

	public ShapeMesh (List<ShapeTriangle> _triangles)
	{
		List<Vector3> vers = new List<Vector3> ();
		List<int> tris = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();

		for (int i = 0; i < _triangles.Count; i++) {
			ShapeTriangle st = _triangles [i];
			for (int vi = 0; vi < 3; vi++) {
				ShapeVertex sv = st.vertices [vi];
				int index = GetVertexIndex (vers, sv.position);
				if (index >= 0) {
					tris.Add (index);
				} else {
					vers.Add (sv.position);
					uvs.Add (sv.uv);
					tris.Add (vers.Count - 1);
				}
			}
		}

		Mesh mesh = new Mesh ();
		mesh.SetVertices (vers);
		mesh.SetTriangles (tris, 0);
		mesh.SetUVs (0, uvs);

		InitByMesh (mesh);
	}

	private void InitByMesh (Mesh _mesh)
	{
		mesh = _mesh;
		vertices.Clear ();
		triangles.Clear ();

		List<Vector3> vs = new List<Vector3> ();
		List<Vector2> uvs = new List<Vector2> ();
		mesh.GetVertices (vs);
		mesh.GetUVs (0, uvs);
		for (int i = 0; i < vs.Count; i++) {
			ShapeVertex sv = new ShapeVertex (i, vs[i], this);
			sv.uv = uvs [i];
			vertices.Add (sv);
		}

		int[] trangs = mesh.GetTriangles (0);
		int tIndex = 0;
		for (int i = 0; i < trangs.Length; i += 3) {
			ShapeVertex v1 = vertices [trangs[i]];
			ShapeVertex v2 = vertices [trangs[i+1]];
			ShapeVertex v3 = vertices [trangs[i+2]];

			ShapeTriangle st = new ShapeTriangle (tIndex, this, v1, v2, v3);
			triangles.Add (st);

			v1.AddTriangle (st);
			v2.AddTriangle (st);
			v3.AddTriangle (st);

			tIndex++;
		}
	}

	public List<ShapeMesh> GetSubMeshes()
	{
		triangles [0].tag = 1;
		for (int i = 1; i < triangles.Count; i++) {
			triangles [i].tag = 0;
		}

		int maxTag = 1;
		for (int i = 0; i < triangles.Count; i++) {
			ShapeTriangle a = triangles [i];

			for (int j = i + 1; j < triangles.Count; j++) {
				ShapeTriangle b = triangles [j];

				if (a.Border (b)) {
					if (a.tag != 0) {
						maxTag = a.tag > maxTag ? a.tag : maxTag;
						if (b.tag != 0) {
							foreach (ShapeTriangle t in triangles) {
								if (t.tag == b.tag) {
									t.tag = a.tag;
								}
							}
						}
						b.tag = a.tag;
					} else {
						if (b.tag == 0) {
							maxTag++;
							b.tag = maxTag;
						}
						a.tag = b.tag;
					}
				}
			}

		}

		Dictionary<int, List<ShapeTriangle>> resultTris = new Dictionary<int, List<ShapeTriangle>> ();

		for (int i = 0; i < triangles.Count; i++) {
			int tag = triangles [i].tag;
			if (!resultTris.ContainsKey(tag)) {
				resultTris[tag] = new List<ShapeTriangle>();
			}
			resultTris [tag].Add (triangles[i]);
		}


		List<ShapeMesh> meshes = new List<ShapeMesh> ();
		foreach (List<ShapeTriangle> subTris in resultTris.Values) {
			ShapeMesh sm = new ShapeMesh (subTris);
			meshes.Add (sm);
		}
		return meshes;
	}

	private int GetVertexIndex (List<Vector3> list, Vector3 pos)
	{
		for (int i = 0; i < list.Count; i++) {
			if (list [i] == pos) {
				return i;
			}
		}
		return -1;
	}

}