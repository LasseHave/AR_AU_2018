using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_SpaceShuttle_EarthDetection : MonoBehaviour {
	private GameObject earth, nose;
	private string hemisphereText, coordinateText;
	// Use this for initialization
	void Start () {
		earth = GameObject.Find ("Earth");
		nose = GameObject.Find ("SpaceShuttle");
        hemisphereText = "Init";
	}

	// Update is called once per frame
	void Update () {
        // Part 1)
        // From shuttle to world
        Matrix4x4 Shuttle_M = transform.localToWorldMatrix;
        // From Earth to world
        Matrix4x4 Earth_M = earth.transform.localToWorldMatrix;
        // Combine the transformation from Shuttle --> World --> Earth
        Matrix4x4 Shuttle_Earth_Relation = Earth_M.inverse * Shuttle_M;
        // Calculate the shuttle nose position in the earth coordinate system
        Vector3 ShuttleNosePos_Earth = Shuttle_Earth_Relation.MultiplyPoint3x4(nose.transform.localPosition);
        coordinateText = "Local Position: " + ShuttleNosePos_Earth.x + ", " + ShuttleNosePos_Earth.y + ", " + ShuttleNosePos_Earth.z;
        // Part 2)
        // Calculate the length of the vector drawn by x,z direction
        var shuttleNose_dist = Mathf.Sqrt(Mathf.Pow(ShuttleNosePos_Earth.x,2) + Mathf.Pow(ShuttleNosePos_Earth.z, 2));
        // Calculate the earth radius
        var r_earth = earth.transform.localScale.x / 2;
        // If the distance is within radius of the earth and shuttle is within 10 cm range we are howevering across earth
        // Due to scale of the Earth we need to include this in the calculations
        if (shuttleNose_dist * earth.transform.localScale.x < r_earth  && ShuttleNosePos_Earth.y * earth.transform.localScale.x < 0.10 )
        {
            // If Z value is positive we are in the northen hemisphere
            if (ShuttleNosePos_Earth.z > 0)
            {
                hemisphereText = "Hemisphere: North";
            }
            else // Otherwise south
            {
                hemisphereText = "Hemisphere: South";
            } 
        } // Clear text if we are not howevering earth
        else
        {
            hemisphereText = "Hemisphere: N/A";
        }

    }
		
    private void OnGUI()
	{
		GUI.color = Color.red;
		GUI.Label(new Rect(10, 10, 500, 100), coordinateText);
        GUI.Label(new Rect(10, 40, 500,100), hemisphereText);
    }
}
