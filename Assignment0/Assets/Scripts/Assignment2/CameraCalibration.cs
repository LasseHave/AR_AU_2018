using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCalibration : MonoBehaviour {

    public Matrix4x4 originalProjection;
    Camera cam;
    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        originalProjection = cam.projectionMatrix;
        originalProjection.m00 = 2 * 833.88811F / 640F;
        originalProjection.m11 = 2 * 834.98600F / 480F;
        originalProjection.m02 = (-2 * 319.52686F / 640F) + 1;
        originalProjection.m12 = (-2 * 238.81514F / 480F) + 1;
        originalProjection.m32 = -1;


        cam.projectionMatrix = originalProjection;
    }
}
