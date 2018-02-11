using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_SpaceShuttle_EarthDetection : MonoBehaviour {
	private GameObject earth, nose;
	private string testText;
	// Use this for initialization
	void Start () {
		earth = GameObject.Find ("Earth");
		nose = GameObject.Find ("SpaceShuttle");
        testText = "Init";
	}

	// Update is called once per frame
	void Update () {

        /* BY LASSE #####
		var nose4 = T(nose.transform.localPosition.x, nose.transform.localPosition.y, nose.transform.localPosition.z);
		var earth4 = T(earth.transform.localPosition.x, earth.transform.localPosition.y, earth.transform.localPosition.z);

		var worldCoords = nose4*earth4; // Multiply as of the slides 
        
		string hemisphere = CalculateHemisphere (worldCoords, nose4);
		testText = hemisphere;
        */ // #####

        //// Jesper Devs
        // Part 1)
        // From shuttle to world
        Matrix4x4 Shuttle_M = transform.localToWorldMatrix;
        // From Earth to world
        Matrix4x4 Earth_M = earth.transform.localToWorldMatrix;
        // Combine the transformation from Shuttle --> World --> Earth
        Matrix4x4 Shuttle_Earth_Relation = Earth_M.inverse * Shuttle_M;
        // Calculate the shuttle nose position in the earth coordinate system
        Vector3 ShuttleNosePos_Earth = Shuttle_Earth_Relation.MultiplyPoint3x4(nose.transform.localPosition);
     
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
                testText = "North";
            }
            else // Otherwise south
            {
                testText = "South";
            } 
        } // Clear text if we are not howevering earth
        else
        {
            testText = "";
        }

    }
		

	private void OnGUI()
	{
		GUI.color = Color.red;
		GUI.Label(new Rect(10, 10, 500, 100), testText);
	}

	private string CalculateHemisphere(Matrix4x4 worldCoords, Matrix4x4 targetCoords){
		if (worldCoords.GetColumn (3) [1] > 1) {
			return "Y is too big!";
		}

		Debug.Log ("Magnitude: " + earth.transform.localScale.magnitude);
		Debug.Log ("World: " + worldCoords.GetColumn(3)[2]);
		Debug.Log ("Dist: " + GetDistance(worldCoords, targetCoords));

		if (GetDistance(worldCoords, targetCoords) < earth.transform.localScale.magnitude) {
			if (worldCoords.GetColumn (3) [2] >= 0.45) { // Is the Z coordinate larger than zero
				return "North";
			} else {
				return "South";
			}
		}
		return "test";
	}

	private double GetDistance(Matrix4x4 worldCoords, Matrix4x4 targetCoords) 
	{
		var R = earth.transform.localScale.magnitude; // Radius of the earth
		var dLat = ToRadians(targetCoords.GetColumn(3)[0]-worldCoords.GetColumn(3)[0]);  // deg2rad below
		var dLon = ToRadians(targetCoords.GetColumn(3)[2]-worldCoords.GetColumn(3)[2]); 
		var a = 
			Mathf.Sin((float)dLat/2) * Mathf.Sin((float)dLat/2) +
			Mathf.Cos((float)ToRadians(worldCoords.GetColumn(3)[0])) * Mathf.Cos((float)ToRadians(targetCoords.GetColumn(3)[0])) * 
			Mathf.Sin((float)dLon/2f) * Mathf.Sin((float)dLon/2f);

		var c = 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1-a)); 
		var d = R * c; // Distance
		return d;
	}

	private double ToRadians(double deg) 
	{
		return deg * (Mathf.PI / 180);
	}

	/**************************************************************************/
	/************ CONVENIENCE FUNCTIONS FOR AFFINE TRANSFORMATIONS ************/
	/**************************************************************************/

	public static Matrix4x4 T (float x, float y, float z)
	{
		Matrix4x4 m = new Matrix4x4();

		m.SetRow(0, new Vector4(1, 0, 0, x));
		m.SetRow(1, new Vector4(0, 1, 0, y));
		m.SetRow(2, new Vector4(0, 0, 1, z));
		m.SetRow(3, new Vector4(0, 0, 0, 1));

		return m;
	}

	public static Matrix4x4 Rx (float a)
	{
		Matrix4x4 m = new Matrix4x4();

		m.SetRow(0, new Vector4(1, 0, 			 0, 			0));
		m.SetRow(1, new Vector4(0, Mathf.Cos(a), -Mathf.Sin(a), 0));
		m.SetRow(2, new Vector4(0, Mathf.Sin(a), Mathf.Cos(a),	0));
		m.SetRow(3, new Vector4(0, 0, 		 	 0, 			1));

		return m;
	}

	public static Matrix4x4 Ry (float a)
	{
		Matrix4x4 m = new Matrix4x4();

		m.SetRow(0, new Vector4(Mathf.Cos(a), 	0, Mathf.Sin(a), 0));
		m.SetRow(1, new Vector4(0, 			  	1, 0, 			 0));
		m.SetRow(2, new Vector4(-Mathf.Sin(a), 	0, Mathf.Cos(a), 0));
		m.SetRow(3, new Vector4(0, 				0, 0, 			 1));

		return m;
	}

	public static Matrix4x4 Rz (float a)
	{
		Matrix4x4 m = new Matrix4x4();

		m.SetRow(0, new Vector4(Mathf.Cos(a), -Mathf.Sin(a), 0, 0));
		m.SetRow(1, new Vector4(Mathf.Sin(a), Mathf.Cos(a),  0, 0));
		m.SetRow(2, new Vector4(0, 			  0, 			 1, 0));
		m.SetRow(3, new Vector4(0, 			  0, 			 0, 1));

		return m;
	}


	public static Matrix4x4 S(float sx,float sy,float sz)
	{
		Matrix4x4 m = new Matrix4x4();

		m.SetRow(0, new Vector4(sx, 0, 0, 0));
		m.SetRow(1, new Vector4(0, sy, 0, 0));
		m.SetRow(2, new Vector4(0,  0,sz, 0));
		m.SetRow(3, new Vector4(0,  0, 0, 1));

		return m;
	}
}
