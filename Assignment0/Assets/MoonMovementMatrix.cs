using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonMovementMatrix : MonoBehaviour {

    private GameObject earth;
    float r_x = .5F, r_z = .6F, delta_rot = 0F, delta_move = 0F, step_size = ((2 * Mathf.PI) / 360F) * 0.05F;
    private Matrix4x4 M_Matrix;
    // Use this for initialization
    void Start()
    {
        earth = GameObject.Find("Earth");
    }

    // Update is called once per frame
    void Update()
    {
        // Rotation/Movement speeds (factor of 1year/rotation)
        var speed_rot = step_size * 12F;
        var speed_move = step_size * (365F/27F);
        delta_rot = delta_rot + speed_rot;
        delta_move = delta_move + speed_move;
        // Rotate object around its own axis
        Vector4 vector4_0 = new Vector4(Mathf.Cos(delta_rot), 0, -Mathf.Sin(delta_rot), 0);
        Vector4 vector4_1 = new Vector4(0, 1, 0, 0);
        Vector4 vector4_2 = new Vector4(Mathf.Sin(delta_rot), 0, Mathf.Cos(delta_rot), 0);
        // Move object according to elipsis
        Vector3 vector_ep = new Vector3(Mathf.Cos(delta_move) * r_x, 0, Mathf.Sin(delta_move) * r_z);
        Vector3 vector_ep_shifted = vector_ep + earth.transform.position;
        Vector4 vector4_3 = new Vector4(vector_ep_shifted.x, vector_ep_shifted.y, vector_ep_shifted.z, 1);
        // 4x4 Matrix
        M_Matrix.SetColumn(0, vector4_0);
        M_Matrix.SetColumn(1, vector4_1);
        M_Matrix.SetColumn(2, vector4_2);
        M_Matrix.SetColumn(3, vector4_3);
        // Because delta increases we directly use the values
        transform.position = M_Matrix.GetColumn(3);
        transform.rotation = M_Matrix.rotation;

    }
}


