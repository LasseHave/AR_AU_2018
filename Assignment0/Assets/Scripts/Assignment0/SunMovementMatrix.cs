using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMovementMatrix : MonoBehaviour {

    float delta_rot = 0F, step_size = ((2 * Mathf.PI) / 360F) * 0.05F;
    private Matrix4x4 M_Matrix;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Rotation speeds (factor of 1year/rotation)
        var speed_rot = step_size * 12F;
        delta_rot = delta_rot + speed_rot;
         // Rotate object around its own axis
        Vector4 vector4_0 = new Vector4(Mathf.Cos(delta_rot), 0, -Mathf.Sin(delta_rot), 0);
        Vector4 vector4_1 = new Vector4(0, 1, 0, 0);
        Vector4 vector4_2 = new Vector4(Mathf.Sin(delta_rot), 0, Mathf.Cos(delta_rot), 0);
         // 4x4 Matrix
        M_Matrix.SetColumn(0, vector4_0);
        M_Matrix.SetColumn(1, vector4_1);
        M_Matrix.SetColumn(2, vector4_2);
         // Because delta increases we directly use the values
        transform.rotation = M_Matrix.rotation;
    }
}
