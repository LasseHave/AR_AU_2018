using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCalibration : MonoBehaviour {

    public Matrix4x4 originalProjection;
    float nearField = 0.05F, farField = 2F, width = 640F, height = 480F, 
        fx = 833.88811F, fy = 834.98600F, cx = 319.52686F, cy = 238.81514F;
    Camera cam;
    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        originalProjection = cam.projectionMatrix; 
        
        // Set the projection matrix properties
        originalProjection.m00 = 2 * fx / width;
        originalProjection.m11 = 2 * fy / height;
        originalProjection.m02 = (-2 * cx / width) + 1;
        originalProjection.m12 = (-2 * cy / height) + 1;
        originalProjection.m32 = -1;
        originalProjection.m22 = -((farField + nearField) / (farField - nearField));
        originalProjection.m23 = -((2 * farField * nearField) / (farField - nearField));
        Debug.Log("First coordinate: " + originalProjection.m22 + " Second coordinate: " + originalProjection.m23);
        // Update camera projection matrix
        cam.projectionMatrix = originalProjection;
    }
}
