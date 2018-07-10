using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorTest2 : MonoBehaviour {


	public int vertexNum = 5;
	public Color baseC = Color.white;
	public Color highlight = Color.cyan;

	Polygon p;
	Polygon CreateSphericalPolygon(int vtxNm, float radius) {

		List<Vector3> vertices = new List<Vector3>();

		Vector3 center = new Vector3(0, 0, 0);

		for (int i = 0; vtxNm > i; i++) {
			vertices.Add(Random.onUnitSphere * radius + center);
		}

		return Factory.Create("Mesh", vertices, new Material(Shader.Find("Vertex Color")));
	}

	void Start() {
		p = CreateSphericalPolygon (vertexNum, 1f);

		p.setColorsAboveCenter (Color.white, Color.cyan);
		//p.setColorOfHighest(Color.white, Color.cyan);
		p.gameObject.transform.position = Vector3.zero;
	}

	/*
	void Update() {
		p.OptimizeMesh ();
	}
	*/
}
