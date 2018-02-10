using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D_SpaceShuttle_EarthDetection : MonoBehaviour {
	private GameObject earth;
	private GameObject nose;
	private string testText;
	// Use this for initialization
	void Start () {
		earth = GameObject.Find ("Earth");
		nose = GameObject.Find ("SpaceShuttleNose");
		testText = "Init";
	}

	// Update is called once per frame
	void Update () {
		var nose4 = T(nose.transform.localPosition.x, nose.transform.localPosition.y, nose.transform.localPosition.z);
		var earth4 = T(earth.transform.localPosition.x, earth.transform.localPosition.y, earth.transform.localPosition.z);

		var worldCoords = nose4*earth4; // Multiply as of the slides 

		Debug.Log ("Magnitude: " + earth.transform.localScale.magnitude);
		Debug.Log ("World: " + worldCoords.GetColumn(3));
		Debug.Log ("Nose: " + nose4.GetColumn(3));

		string hemisphere = CalculateHemisphere (worldCoords, nose4);
		testText = hemisphere;
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
