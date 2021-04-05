using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnotGenerator : MonoBehaviour {
	private List<Vector3> normals = new List<Vector3>();
	private List<Vector3> points = new List<Vector3>();
	private List<Vector3> tangents = new List<Vector3>();
	private List<Vector3> vertices = new List<Vector3>();
	private List<Triangle> triangles = new List<Triangle>();
	private List<Vector3> triangleNormals = new List<Vector3>();
	public float betaIncrement = .04f;
	public GameObject sphere;
	public float t;
	public float normalSize = 1;
	public float tangentSize = 1;
	public float r = 1;
	public GameObject marker;
	public int circleVertices = 8;
	private List<int> triangleIndices = new List<int>();
	int k = 8;
	private struct Triangle
	{
		public Vector3[] points;
		public Triangle(Vector3 a, Vector3 b, Vector3 c)
		{
			points = new Vector3[] { a, b, c };
		}

		public Vector3 CalculateNormal()
		{
			Vector3 p1 = points[0] - points[1];
			Vector3 p2 = points[0] - points[2];
			Vector3 normal = Vector3.Cross(p1, p2).normalized;
			return normal;
		}
	}

	// Use this for initialization
	void Start () {
		int iters = 0;
		Vector3 firstPos = Vector3.zero;
		float beta = 0;
		float shortestDistance = float.PositiveInfinity;
		int shortestIndex = 0;
		while(beta < (4*k+2)*Mathf.PI/*Mathf.PI*/)
		{
			iters++;
			beta += betaIncrement;
			iters++;
			t = Time.deltaTime;
			Vector3 currentPos = KnotPoint(beta);
			Vector3 nextPos = KnotPoint(beta + betaIncrement);
			Vector3 T = Vector3.Normalize(nextPos - currentPos);
			Vector3 B = Vector3.Normalize(Vector3.Cross(T, nextPos + currentPos));
			Vector3 N = -Vector3.Normalize(Vector3.Cross(B, T));
			tangents.Add(T);
			points.Add(currentPos);
			normals.Add(N);
			
			//pointMarkers.Add(Instantiate(sphere, currentPos, Quaternion.identity));
		}
		
		for(int i = 0; i < points.Count - 2; i++)
		{
			Vector3 currentPoint = points[i];
			Vector3 nextPoint = points[i + 1];
			Vector3 forwardVector = nextPoint - currentPoint;
			forwardVector.Normalize();
			print(points.Count);
			print(normals.Count);
			Vector3 upVector = normals[i];
			Quaternion pointQuat = Quaternion.LookRotation(forwardVector, upVector);
			Matrix4x4 rotMatrix = Matrix4x4.Rotate(pointQuat);

			for(int z = 0; z < circleVertices; z++)
			{
				CreatePoint(z, circleVertices, rotMatrix, currentPoint);
			}
		}
		GenerateTriangles();
		GenerateNormals();
		CreateMesh();
	}

	/*void OnDrawGizmos()
	{
		for(int i = 0; i < points.Count-1;i++)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(points[i], points[i + 1]);

			Gizmos.color = Color.red;
			Gizmos.DrawLine(points[i], points[i] + normals[i] * normalSize);
		}
		for(int i = 0; i < triangles.Count; i++)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(triangles[i].points[0], triangles[i].points[1]);
			Gizmos.DrawLine(triangles[i].points[1], triangles[i].points[2]);
			Gizmos.DrawLine(triangles[i].points[0], triangles[i].points[2]);        //Instantiate(marker, pivot + posDiff, Quaternion.identity);

		}
	}*/

	private Vector3 KnotPoint(float beta)
	{

		/*float r = 1.2f + 0.6f * Mathf.Sin(0.5f * Mathf.PI + 6 * beta);
		float theta = 4 * beta;
		float phi = 0.2f * Mathf.PI * Mathf.Sin(6 * beta);*/
		/*float r = 0.8f + 1.6f * Mathf.Sin(6 * beta);
		float theta = 2 * beta;
		float phi = 0.6f * Mathf.PI * Mathf.Sin(6 * beta);*/


		/*Vector3 newPos = Vector3.zero;
		newPos.x = r * Mathf.Cos(phi) * Mathf.Cos(theta);
		newPos.y = r * Mathf.Cos(phi) * Mathf.Sin(theta);
		newPos.z = r * Mathf.Sin(phi);
		return newPos;*/

		float x = Mathf.Cos(beta) * (2 - Mathf.Cos(2 * beta / (2 * k + 1)));
		float y = Mathf.Sin(beta) * (2 - Mathf.Cos(2 * beta / (2 * k + 1)));
		float z = -Mathf.Sin(2 * beta / (2 * k + 1));
		return new Vector3(x, y, z);
	}

	private void CreatePoint(int currentIndex, int maxPoints, Matrix4x4 rotationMatrix, Vector3 pivot)
	{
		var theta = ((Mathf.PI * 2) / maxPoints);
		var angle = (theta * currentIndex);

		float xPos = (r * Mathf.Cos(angle));
		float yPos = (r * Mathf.Sin(angle));
		Vector3 posDiff = new Vector3(xPos, yPos, 0);
		posDiff = rotationMatrix.MultiplyPoint3x4(posDiff);
		vertices.Add(pivot + posDiff);
		//Instantiate(marker, pivot + posDiff, Quaternion.identity);
	}

	private void GenerateTriangles()
	{
		print(vertices.Count);
		for(int i = 0; i < vertices.Count; i++)
		{
			int index1 = i + circleVertices + 1;
			int index2 = i + circleVertices;
			int index3 = i + 1;
			index1 %= vertices.Count;
			index2 %= vertices.Count;
			index3 %= vertices.Count;

			Vector3 v0 = vertices[i];
			Vector3 v1 = vertices[index3];
			Vector3 v5 = vertices[index2];
			triangles.Add(new Triangle(v0, v1, v5));
			triangleIndices.Add(i);
			triangleIndices.Add(index3);
			triangleIndices.Add(index2);
			Vector3 v6 = vertices[index1];
			triangles.Add(new Triangle(v1, v5, v6));
			triangleIndices.Add(index2);
			triangleIndices.Add(index3);				
			triangleIndices.Add(index1);
		}
		print(triangles.Count);
	}

	private void GenerateNormals()
	{
		foreach(Triangle t in triangles)
		{
			triangleNormals.Add(t.CalculateNormal());
		}
	}

	private void CreateMesh()
	{
		Mesh m = new Mesh();
		m.vertices = vertices.ToArray();
		m.triangles = triangleIndices.ToArray();
		print(vertices.Count);
		print(triangleNormals.Count);
		print(triangles.Count);
		m.normals = new Vector3[vertices.Count];
		
		m.RecalculateNormals();
		GetComponent<MeshFilter>().sharedMesh = m;
		//UnityEditor.AssetDatabase.CreateAsset(m, "Assets/Knot1.asset");
		//UnityEditor.AssetDatabase.SaveAssets();
	}
}

