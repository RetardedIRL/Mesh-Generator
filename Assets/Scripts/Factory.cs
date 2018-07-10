using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Factory {

	public static Polygon Create(GameObject gameObject, List<Vector3> vertices, Material material) {
		return new Polygon3D(
			gameObject,
			Triangulator3D.Triangulate(vertices),
			material
		).Build();
	}

	public static Polygon Create(string name, List<Vector3> vertices, Material material) {
		return Create(new GameObject(name), vertices, material);
	}

	public static Polygon Create(string name, List<Vector3> vertices) {
		return Create(new GameObject(name), vertices, new Material(Shader.Find("Diffuse")));
	}

	public static Polygon Create(List<Vector3> vertices) {
		return Create(new GameObject(), vertices, new Material(Shader.Find("Diffuse")));
	}

}
