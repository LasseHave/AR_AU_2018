﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthMotion : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0f, 1f, .5f));
		transform.RotateAround (Vector3.zero, Vector3.up, 60 * Time.deltaTime);
	}
}
