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
		testText = "Whaaat!?";
	}

	// Update is called once per frame
	void Update () {
		var nose4 = T(nose.transform.localPosition.x, nose.transform.localPosition.y, nose.transform.localPosition.z);
		var earth4 = T(earth.transform.localPosition.x, earth.transform.localPosition.y, earth.transform.localPosition.z);

		var test = nose4 * earth4;

		if (test.GetColumn (3) [0] > 0) {
			testText = "North";
		}
		Debug.Log (test.GetColumn(3));
	}

	private void OnGUI()
	{
		GUI.color = Color.red;
		GUI.Label(new Rect(10, 10, 500, 100), testText);
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
