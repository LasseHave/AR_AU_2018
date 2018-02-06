using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShuttle : MonoBehaviour {

	public GameObject landingStrip;
	public GameObject quad;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var dist = Vector3.Dot(landingStrip.transform.position, this.transform.position);


	}
}
