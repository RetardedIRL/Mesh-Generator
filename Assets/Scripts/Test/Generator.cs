using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour {

	[Range(10, 40)]
	public int vertNum = 30;

	[Range(0.1f, 5f)]
	public float radius = 1f;

	public int amount = 1;

	Polygon CreateSphericalPolygon(int numVertices, float radius) {
		
		List<Vector3> vertices = new List<Vector3>();

		Vector3 center = new Vector3(0, 0, 0);

		for (int i = 0; numVertices > i; i++) {
			vertices.Add(Random.onUnitSphere * radius + center);
		}

		return Factory.Create("Stone", vertices);
	}
		
	void Start() {
		CreateMultiple ();
	}

	void Create() {
		Polygon p = CreateSphericalPolygon (vertNum, radius);

		p.gameObject.transform.position = Vector3.zero;
	}

	void CreateMultiple() {

		for (int i = 0; i < amount; i++) {

			Polygon p = CreateSphericalPolygon (vertNum, radius);

			p.gameObject.transform.position = new Vector3 (((i + radius) * 2) - radius, 0, 0);

		}
	}
}
