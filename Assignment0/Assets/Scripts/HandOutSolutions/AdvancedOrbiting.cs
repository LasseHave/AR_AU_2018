using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedOrbiting : MonoBehaviour {
	
	private float speed = 100f;
	private float earthDistance = 15f;
	private float moonDistance = 5f;
	private float sunYOffset = 6f;

    private GameObject sun;
    private GameObject earth;
    private GameObject moon;

    void Start () {
		sun = GameObject.Find("Sun");
		earth = GameObject.Find("Earth");
		moon = GameObject.Find("Moon");
	}	

	void Update ()
	{
		Quaternion localYRotation = Quaternion.Euler(new Vector3(0, 0, speed * Time.time));
		sun.transform.rotation = localYRotation;
		earth.transform.rotation = localYRotation;
		moon.transform.rotation = localYRotation;

		Matrix4x4 sunMatrix = T(0, sunYOffset, 0);

		Matrix4x4 earthMatrix = sunMatrix								 
								 * Ry(Mathf.Deg2Rad * speed * Time.time)
								 * S(1f, 1f, 2f)
								 * T(earthDistance, 0, 0);

		Matrix4x4 moonMatrix = earthMatrix 
								* Ry(Mathf.Deg2Rad * speed * Time.time)
							    * T(moonDistance, 0, 0);

		sun.transform.position = sunMatrix.GetColumn(3);
		earth.transform.position = earthMatrix.GetColumn(3);
		moon.transform.position = moonMatrix.GetColumn(3);
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
