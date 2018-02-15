using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Tracking : MonoBehaviour {

	public GameObject ruedingerHead;
	public GameObject ruedingerSpine;
	public GameObject falcon;
	public GameObject camera;

	private Quaternion initialRot;
	// Use this for initialization
	private float speed = 4f;

	/*
	Rotate the head so that the penguin always looks at the beer image target (the head GameObject
	is a child of the rigged model). 
	You can use Quaternion.LookRotation (use the up-vector of the penguin’s spine for better results). 
	Hint: For a correct rotation you have to save the initial rotation of the head during startup to 
	concatenate with your calculated rotation at runtime. 
	 */

	void Start () {
		ruedingerHead = GameObject.Find ("head");
		ruedingerSpine = GameObject.Find ("spine");
		falcon = GameObject.Find ("Falcon");
		camera = GameObject.Find ("ARCamera");

		initialRot = ruedingerHead.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		//hvis falcon ikke er fundet af vuforia, kig ind i kameraet

		TrackableBehaviour trackable = falcon.GetComponent<TrackableBehaviour> ();
		bool falconIsDetected = trackable.CurrentStatus == TrackableBehaviour.Status.DETECTED || trackable.CurrentStatus ==  TrackableBehaviour.Status.TRACKED;

		GameObject target = falconIsDetected ? falcon : camera;

		Quaternion currentRotation = ruedingerHead.transform.rotation;
		Vector3 relativePos = target.transform.position - ruedingerHead.transform.position;
		Quaternion newRot = Quaternion.LookRotation (relativePos, ruedingerSpine.transform.up) * initialRot;

		ruedingerHead.transform.rotation = Quaternion.Lerp (currentRotation, newRot, Time.deltaTime * speed );

	}
}
