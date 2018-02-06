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
		var dot = Vector3.Dot(landingStrip.transform.position.normalized, this.transform.position.normalized);
		var normalizedDot = (dot + 1) / 2;

		Color color = Color.Lerp (green, red, dot);

		quadRenderer.material.color = color;
        Debug.Log("Shuttle pos: " + this.transform.position.normalized);
        Debug.Log("Landing Pos: " + landingStrip.transform.position.normalized);
        Debug.Log("Dot product: " + normalizedDot);
		Debug.Log ("color: " + color);
	}
}
