using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class DrawButton : MonoBehaviour, IVirtualButtonEventHandler {

	GameObject cam;

	// Use this for initialization
	void Start () {

		cam = GameObject.Find ("ARCamera");

		GameObject virtualButtonObject = GameObject.Find ("drawButton");
		virtualButtonObject.GetComponent<VirtualButtonBehaviour> ().RegisterEventHandler (this);
		
	}

	/// <summary>
	/// Called when the virtual button has just been pressed:
	/// </summary>
	public void OnButtonPressed(VirtualButtonBehaviour vb) {
		Debug.Log("button pressed");

		cam.GetComponent<Assignment3_e> ().drawButtonState = true;
	}

	/// <summary>
	/// Called when the virtual button has just been released:
	/// </summary>
	public void OnButtonReleased(VirtualButtonBehaviour vb) { 
		Debug.Log("button released");
		cam.GetComponent<Assignment3_e> ().drawButtonState = false;
		cam.GetComponent<Assignment3_e> ().foundFingerHSV = false;
	}
}
