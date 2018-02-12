using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShuttle : MonoBehaviour {

	public GameObject landingStrip;
	public GameObject quad;

	private Renderer quadRenderer;
    
	// Use this for initialization
	void Start () {
        // Find objects
        landingStrip = GameObject.Find("LandingStrip");
        quad = GameObject.Find("Quad");
        quadRenderer = quad.GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {
        // Calculate dot products on forward vectors and right vectors (x,z axies)
		var forwardDot = Vector3.Dot(transform.forward, landingStrip.transform.forward);
		var rightDot = Vector3.Dot(transform.right, landingStrip.transform.right);
        // Combine the dot products for color status
		var t = Mathf.Clamp01 (forwardDot * rightDot);
        // Define the color from red towards green as dot product approach 1
		var color = new Color ((1 - t), t, 0);
        // Set color
		quadRenderer.material.color = color;

	}
}
