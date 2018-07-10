using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Polygon {

	public GameObject gameObject;

	public List<Vector3> vertices;
	public List<int> indexes;

	public static int counter = 0;

	protected Polygon(GameObject gameObject, Material material) {
		gameObject.AddComponent<MeshFilter>();
		
		var renderer = gameObject.AddComponent<MeshRenderer>();
		renderer.material = material;
		renderer.receiveShadows = true;
		
		var collider = gameObject.AddComponent<MeshCollider>();
		collider.convex = true;

		gameObject.layer = LayerMask.NameToLayer ("Stone");
		this.gameObject = gameObject;
	}

	public Polygon(GameObject gameObject, List<Vector3> vertices, Material material) : this(gameObject, material) {
		// todo: check a number of vertices is multiple of 3
		this.vertices = vertices;
		indexes = Enumerable.Range(0, vertices.Count).ToList<int>();
	}

	protected Vector3 GetCenterOfTriangle(int firstIndexOfTriangle) {
		var i = firstIndexOfTriangle;
		return (vertices[i] + vertices[i + 1] + vertices[i + 2]) / 3;
	}
	
	protected Vector3 GetNormalOfTriangle(int firstIndexOfTriangle) {
		var i = firstIndexOfTriangle;
		return Vector3.Cross(vertices[i + 1] - vertices[i], vertices[i + 2] - vertices[i]);
	}

	protected void ReverseSurface(int firstIndexOfTriangle) {
		var i = firstIndexOfTriangle;
		int temp = indexes[i + 1];
		indexes[i + 1] = indexes[i + 2];
		indexes[i + 2] = temp;
	}

	protected abstract void OptimizeIndexes();
	
	protected void BuildMesh() {
		var mesh = gameObject.GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indexes.ToArray();
		mesh.uv = new Vector2[mesh.vertices.Length];
		mesh.Optimize();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		var m = new Mesh();
		m.vertices = vertices.ToArray();
		m.triangles = indexes.ToArray();
		gameObject.GetComponent<MeshCollider>().sharedMesh = m;
	}

	public Polygon Build() {
		OptimizeIndexes();
		BuildMesh();

		OptimizeMesh ();
		return this;
	}

	public Polygon Rigidize() {
		gameObject.AddComponent<Rigidbody>();
		return this;
	}

	private HashSet<Triangle> GetTriangles() {
	
		HashSet<Triangle> temp = new HashSet<Triangle> ();

		for (int i = 0; i < vertices.ToArray ().Length; i = i + 3) {
		
			temp.Add(new Triangle(vertices[i], vertices[i + 1], vertices[i + 2]));
		}

		return temp;
	}

	public void setColorsAboveCenter(Color baseC, Color highlightC) {

		Mesh mesh = this.gameObject.GetComponent<MeshFilter> ().mesh;

		Vector3[] vertices = mesh.vertices;

		Color[] colors = new Color[vertices.Length];

		Vector3 center = gameObject.GetComponent<MeshCollider>().bounds.center;

		for (int i = 0; i < colors.Length; i++) {
			if(vertices[i].y > center.y) {
				colors [i] = highlightC;	
			}
			else {
				colors [i] = baseC;	
			}
		}

		mesh.colors = colors;

		/*
		for (int i = 0; i < colors.Length; i++) {
		
			colors [i] = Mathf.Lerp (baseC, highlightC, colors [i].y - center.y);
		}
		*/
	}
		
	public bool IsPointInside (Vector3 point, Vector3 normal) {

		counter++;
		Debug.Log (normal);
		Vector3 tempPoint = point + normal * 2f;
		float threshold = 0.01f;

		RaycastHit hit;
		if (Physics.Raycast (tempPoint, -normal, out hit, Mathf.Infinity)) {

			float temp = (hit.point - point).magnitude;

			Debug.DrawLine (point, (point + new Vector3(0.01f, 0f, 0f)), Color.white);

			Debug.DrawLine (tempPoint, hit.point, Color.red);

			if (temp <= threshold) {
				Debug.DrawLine (tempPoint, hit.point, Color.green);
				return false;
			}
			else
				Debug.DrawLine (point, (point + new Vector3(0.01f, 0f, 0f)), Color.white);

			return true;
		}

		// inversed normal, repeat raycast try backwards
		else {
			tempPoint = point - normal * 2f;

			if (Physics.Raycast (tempPoint, normal, out hit, Mathf.Infinity)) {

				float temp = (hit.point - point).magnitude;

				Debug.DrawLine (tempPoint, hit.point, Color.magenta);

				if (temp <= threshold) {
					Debug.DrawLine (tempPoint, hit.point, Color.green);
					return false;
				}
				else
					Debug.DrawLine (point, (point + new Vector3(0.01f, 0f, 0f)), Color.white);

				return true;
			}
			else {
				//Debug.Log ("???");
				return false;
			}
		}
	}

	public void OptimizeMesh() {

		HashSet<Triangle> triangles = GetTriangles ();

		Debug.Log ("Triangles: " + triangles.Count + ", Vertices: " + vertices.ToArray().Length);
		triangles.RemoveWhere (t => IsPointInside (t.getCenter(), t.getNormal()));

		vertices = Function.ConvertTrianglesToList(triangles.ToList ());

		Debug.Log ("Triangles: " + triangles.Count + ", Vertices: " + vertices.ToArray().Length);

		indexes = Enumerable.Range(0, vertices.Count).ToList<int>();

		Debug.Log (counter);
		OptimizeIndexes ();
		BuildMesh ();
	}
}

