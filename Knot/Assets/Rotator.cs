using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public float xSpeed = 0;
	public float ySpeed = 0;
	public float zSpeed = 0;

	private Vector3 xAxis = new Vector3(1, 0, 0);
	private Vector3 yAxis = new Vector3(0, 1, 0);
	private Vector3 zAxis = new Vector3(0, 0, 1);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(xAxis, xSpeed * Time.deltaTime);
		transform.Rotate(yAxis, ySpeed * Time.deltaTime);
		transform.Rotate(zAxis, zSpeed * Time.deltaTime);


	}
}
