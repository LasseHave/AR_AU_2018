using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visual_b_shader : MonoBehaviour {
	public Renderer rend;
	GameObject img;
	Color testColor;

	// Use this for initialization
	void Start () {
		img = GameObject.Find ("ImageTarget");
		testColor = new Color(1, 0, 1, 1);

		MeshRenderer gameObjectRenderer = img.GetComponent<MeshRenderer>();

		Material newMaterial = new Material(Shader.Find("Custom/Gtayscale"));

		newMaterial.color = testColor;
		gameObjectRenderer.material = newMaterial ;

	}
	
	// Update is called once per frame
	void Update () {

	}
}




