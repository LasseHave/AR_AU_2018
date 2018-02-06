using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_SpaceShuttle_EarthDetection : MonoBehaviour {

	private GameObject earth;
	private GameObject nose;
	// Use this for initialization
	void Start () {
		earth = GameObject.Find ("Earth");
		nose = GameObject.Find ("SpaceShuttleNose");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
