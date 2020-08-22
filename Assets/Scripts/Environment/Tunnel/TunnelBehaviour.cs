using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelBehaviour : MonoBehaviour {

	public Vector3 distance;
	public float speed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(0, 0, -speed * Time.deltaTime);

		distance.z = transform.position.z;

	}
}
