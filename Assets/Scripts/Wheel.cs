using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wheel : MonoBehaviour {

	List<Vector3> verts;
	List<Vector2> uvs;
	List<int> tris;

	// Use this for initialization
	void Start () {
		verts = new List<Vector3>();
		uvs = new List<Vector2>();
		tris = new List<int>();

		float thick = 0.3f;
		float radius = 0.345f;
		float halfThick = thick * 0.5f;

		int numSides = 8;
		float ang = 1f / numSides * Mathf.PI*2f;

		// right
		for (int i = 0; i < numSides; i++)
		{
			verts.Add(new Vector3(halfThick, 0f, 0f));
			verts.Add(new Vector3(halfThick, Mathf.Sin(ang*(i+1))*radius, Mathf.Cos(ang*(i+1))*radius));
			verts.Add(new Vector3(halfThick, Mathf.Sin(ang*i)*radius, Mathf.Cos(ang*i)*radius));

			uvs.Add(new Vector2(0f, 0f));
			uvs.Add(new Vector2(1f, 0f));
			uvs.Add(new Vector2(1f, 1f));

			tris.Add(tris.Count);
			tris.Add(tris.Count);
			tris.Add(tris.Count);
		}

		// left
		for (int i = 0; i < numSides; i++)
		{
			verts.Add(new Vector3(-halfThick, 0f, 0f));
			verts.Add(new Vector3(-halfThick, Mathf.Sin(ang*i)*radius, Mathf.Cos(ang*i)*radius));
			verts.Add(new Vector3(-halfThick, Mathf.Sin(ang*(i+1))*radius, Mathf.Cos(ang*(i+1))*radius));

			uvs.Add(new Vector2(0f, 0f));
			uvs.Add(new Vector2(1f, 0f));
			uvs.Add(new Vector2(1f, 1f));

			tris.Add(tris.Count);
			tris.Add(tris.Count);
			tris.Add(tris.Count);
		}

		// sides
		int k = tris.Count;
		for (int i = 0; i < numSides; i++)
		{
			verts.Add(new Vector3(halfThick, Mathf.Sin(ang*i)*radius, Mathf.Cos(ang*i)*radius));
			verts.Add(new Vector3(halfThick, Mathf.Sin(ang*(i+1))*radius, Mathf.Cos(ang*(i+1))*radius));
			verts.Add(new Vector3(-halfThick, Mathf.Sin(ang*(i+1))*radius, Mathf.Cos(ang*(i+1))*radius));
			verts.Add(new Vector3(-halfThick, Mathf.Sin(ang*i)*radius, Mathf.Cos(ang*i)*radius));

			uvs.Add(new Vector2(0f, 0f));
			uvs.Add(new Vector2(0f, 1f));
			uvs.Add(new Vector2(1f, 1f));
			uvs.Add(new Vector2(1f, 0f));

			tris.Add(k);
			tris.Add(k+1);
			tris.Add(k+2);
			tris.Add(k+2);
			tris.Add(k+3);
			tris.Add(k);

			k += 4;
		}


		Mesh mesh = new Mesh();
		mesh.vertices = verts.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.Optimize();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshRenderer>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
