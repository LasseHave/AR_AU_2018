using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonRotation : MonoBehaviour {

	private GameObject earth;

	// Use this for initialization
	void Start () {
		earth = GameObject.Find ("Earth");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up);
        transform.RotateAround (earth.transform.position, Vector3.up, 20 * Time.deltaTime);
    }
}
