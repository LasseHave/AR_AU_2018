using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontCannons : MonoBehaviour
{

    public GameObject Falcon_Object, FrontCannon1, FrontCannon2, MainCannon, MainCannonTop;

    // Use this for initialization
    void Start()
    {
        Falcon_Object = GameObject.Find("Falcon");
        FrontCannon1 = GameObject.Find("FrontCannon1");
        FrontCannon2 = GameObject.Find("FrontCannon2");
        MainCannon = GameObject.Find("MainCannon");
        MainCannonTop = GameObject.Find("MainCannonTop");

    }

    // Update is called once per frame
    void Update()
    {
        // METHOD 1 - WORKS
        // Transform the local displacement with the world coordinates with respect to the falcon (W  = M * L)
        //   /*   
        Matrix4x4 FinaleLocal1 = Falcon_Object.transform.localToWorldMatrix
               * T(0.004F * (1F / (Falcon_Object.transform.localScale.x)),
               0,
               0.048F * (1F / (Falcon_Object.transform.localScale.z)));
        Matrix4x4 FinaleLocal2 = Falcon_Object.transform.localToWorldMatrix
            * T(-0.004F * (1F / (Falcon_Object.transform.localScale.x)),
            0,
            0.049F * (1F / (Falcon_Object.transform.localScale.z)));
        // Apply the new world position to the cannons
        FrontCannon1.transform.position = FinaleLocal1.GetColumn(3);
        FrontCannon2.transform.position = FinaleLocal2.GetColumn(3);
        //   */

        // METHOD 2  - WORKS BEST
        FrontCannon1.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
            0.004F * (1F / (Falcon_Object.transform.localScale.x)),
            0,
            0.048F * (1F / (Falcon_Object.transform.localScale.z))));
        FrontCannon2.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
            -0.004F * (1F / (Falcon_Object.transform.localScale.x)),
            0,
            0.048F * (1F / (Falcon_Object.transform.localScale.z))));

        MainCannon.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
             0,
             0.025F / 2 * (1F / (Falcon_Object.transform.localScale.y)),
             -0.009F * (1F / (Falcon_Object.transform.localScale.z))));

        MainCannonTop.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
      0,
      0.025F * (1F / (Falcon_Object.transform.localScale.y)),
      -0.009F * (1F / (Falcon_Object.transform.localScale.z))));

        /*
         // METHOD 2  - WORKS BEST
         FrontCannon1.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
             0.004F * (1F / (Falcon_Object.transform.localScale.x)),
             0,
             0.048F * (1F / (Falcon_Object.transform.localScale.z))));
         FrontCannon2.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
             -0.004F*(1F / (Falcon_Object.transform.localScale.x)),
             0,
             0.048F*(1F / (Falcon_Object.transform.localScale.z))));
             */
        /*
 /// METHOD 3 - DOESN'T WORK
 FrontCannon1.transform.position = Falcon_Object.transform.position + new Vector3(
     0.004F * (1F / (Falcon_Object.transform.localScale.x)),
     0,
     0.048F * (1F / (Falcon_Object.transform.localScale.z)));
 FrontCannon2.transform.position = Falcon_Object.transform.position + new Vector3(
     -0.004F * (1F / (Falcon_Object.transform.localScale.x)),
     0,
     0.048F * (1F / (Falcon_Object.transform.localScale.z)));
     */
        // Set cannons rotation
        FrontCannon1.transform.rotation = Falcon_Object.transform.rotation;
        FrontCannon2.transform.rotation = Falcon_Object.transform.rotation;
        MainCannon.transform.rotation = Falcon_Object.transform.rotation;
        MainCannonTop.transform.rotation = Falcon_Object.transform.rotation;
        //  Debug.Log("Cannon1Pos: " + FrontCannon1.transform.position);
        //  Debug.Log("Cannon1_TPos: " + (Falcon_Object.transform.position + new Vector3(0.04F, 0, 0.007F)));
        //  Debug.Log("Cannon1_RPos: " + (Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0.048F, 0, -0.004F))));

    }
    public static Matrix4x4 T(float x, float y, float z)
    {
        Matrix4x4 m = new Matrix4x4();

        m.SetRow(0, new Vector4(1, 0, 0, x));
        m.SetRow(1, new Vector4(0, 1, 0, y));
        m.SetRow(2, new Vector4(0, 0, 1, z));
        m.SetRow(3, new Vector4(0, 0, 0, 1));

        return m;
    }

    public static Matrix4x4 S(float sx, float sy, float sz)
    {
        Matrix4x4 m = new Matrix4x4();

        m.SetRow(0, new Vector4(sx, 0, 0, 0));
        m.SetRow(1, new Vector4(0, sy, 0, 0));
        m.SetRow(2, new Vector4(0, 0, sz, 0));
        m.SetRow(3, new Vector4(0, 0, 0, 1));

        return m;
    }
}
