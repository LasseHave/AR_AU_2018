using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonRotation : MonoBehaviour {

	private GameObject earth;
	private GameObject particleCollision;

	// Use this for initialization
	void Start () {
		earth = GameObject.Find ("Earth");
		particleCollision = GameObject.Find ("MeteorSystem");
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0f, 1f, .5f));
		transform.RotateAround (earth.transform.position, Vector3.up, 20 * Time.deltaTime);
	}
}
