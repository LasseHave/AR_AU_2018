using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShuttle : MonoBehaviour {

	public GameObject landingStrip;
	public GameObject quad;

	private Renderer quadRenderer;

	private Color red = new Color(255, 0, 0);
	private Color green = new Color(0, 255, 0);

	// Use this for initialization
	void Start () {
        landingStrip = GameObject.Find("LandingStrip");
        quad = GameObject.Find("Quad");
        quadRenderer = quad.GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {
		var forwardDot = Vector3.Dot(transform.forward, landingStrip.transform.forward);
		var rightDot = Vector3.Dot(transform.right, landingStrip.transform.right);

		var t = Mathf.Clamp01 (forwardDot * rightDot);

		var color = new Color ((1 - t), t, 0);
		quadRenderer.material.color = color;

		Debug.Log (t);
		Debug.Log ("255 * (1 - " + t + "), 255 * t, 0");
		Debug.Log (color);
        Debug.Log("forwardDot: " + forwardDot);
        Debug.Log("rightDow: " + rightDot);
	}
}
