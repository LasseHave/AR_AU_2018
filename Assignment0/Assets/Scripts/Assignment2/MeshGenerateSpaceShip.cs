using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshGenerateSpaceShip : MonoBehaviour
{
    bool blink = false, direction_control = true;
    int frame_counter;
    float wing_counter = 0F;
    Mesh mesh;

    // Use this for initialization
    void Start()
    {
        mesh = this.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        mesh.vertices = new Vector3[] { new Vector3(1, 0, 0), new Vector3(1, 1, 1), new Vector3(2, 1, 1) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new int[] { 0, 1, 2 };

    }

    // Update is called once per frame
    void Update()
    {
        if (wing_counter >= 1)
            direction_control = true;
        else if (wing_counter <= -1)
            direction_control = false;
        
        if (direction_control) 
            wing_counter = wing_counter - 1/60F;
        else
            wing_counter = wing_counter + 1/60F;

        frame_counter +=1;
        mesh.Clear();

        Vector3[] temp_update = new Vector3[2];
        int[] temp_update_index = new int[2];
        temp_update_index[0] = 0;
        temp_update[0] = new Vector3(0, wing_counter, 0);
        temp_update_index[1] = 11;
        temp_update[1] = new Vector3(5, wing_counter, 0);
   

        mesh.vertices = Space_ship_mesh(temp_update_index, temp_update);
   
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        mesh.uv = uvs;
        mesh.triangles = new int[] { // 18 triangles!
        0, 1, 2,
        2, 1, 3,
        3, 1, 4,
        1, 5, 4,
        3, 4, 6,
        7, 3, 6,
        6, 4, 8,
        6, 8, 9,
        9, 8, 10,
        9, 10, 11,
        4, 5, 8,
        8, 5, 12,
        8, 12, 10,
        5, 13, 12,
        12, 13, 14,
        13, 15, 14,
        14, 15, 16,
        15, 17, 16
    };
           // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.grey, new Color(0.68F,0.68F,0.68F), vertices[i].z / 5F);

        colors[17] = Color.black;
         //    mesh.RecalculateNormals();
        if (frame_counter > 5)
        {
            if (blink)
                blink = false;
            else
                blink = true;
            frame_counter = 0;
            Debug.Log(Time.time);
            Debug.Log("here");
        
        } // assign the array of colors to the Mesh.
        if (blink)
            colors[7] = Color.yellow;
        else
            colors[7] = new Color(1F, 0.4F, 0F);
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    public Vector3[] Space_ship_mesh(int[] index,  Vector3[] vector3)
    {
        int count = 0;
        Vector3[] vertices_temp = new Vector3[] {
            new Vector3(0, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), // Point 0,1,2
            new Vector3(2, 0, 0), new Vector3(2, 0, 1), new Vector3(2, 0, 2), // Point 3,4,5
            new Vector3(3, 0, 0), new Vector3(2.5F, 0, -1), new Vector3(3, 0, 1),// Point 6,7,8
            new Vector3(4, 0, 0), new Vector3(4, 0, 1), new Vector3(5, 0, 0),// Point 9,10,11
            new Vector3(3, 0, 2), new Vector3(2, 0, 3), new Vector3(3, 0, 3),// Point 12,13,14
            new Vector3(2, 0, 4), new Vector3(3, 0, 4), new Vector3(2.5F, 0, 5)// Point 15,16,17
        };



        foreach (var integer in index)
        {
            vertices_temp[integer] = vector3[count];
            count++;
        }

        return vertices_temp;
    }

}
/*  Vector3[] normals = new Vector3[4];

     normals[0] = -Vector3.forward;
     normals[1] = -Vector3.forward;
     normals[2] = -Vector3.forward;
     normals[3] = -Vector3.forward;

     mesh.normals = normals;
     */
/* Vector3[] vertricies = new Vector3[4];
 vertricies[0] = new Vector3(0, 0, 0);
 vertricies[1] = new Vector3(1, 0, 0);
 vertricies[2] = new Vector3(1, 1, 0);
 vertricies[3] = new Vector3(0, 1, 0);

 mesh.vertices = vertricies;

 meshFilter.mesh = mesh;
 meshFilter.uv = vertricies;
 */

/*
var mf= this.GetComponent<MeshFilter> ();
var mesh = new Mesh();
mf.mesh = mesh;

Vector3[] vertices = new Vector3[4];

vertices[0] = new Vector3(0, 0, 0);
vertices[1] = new Vector3(width, 0, 0);
vertices[2] = new Vector3(0, height, 0);
vertices[3] = new Vector3(width, height, 0);

mesh.vertices = vertices;

int[] tri = new int[6];

tri[0] = 0;
tri[1] = 2;
tri[2] = 1;

tri[3] = 2;
tri[4] = 3;
tri[5] = 1;

mesh.triangles = tri;

Vector3[] normals  = new Vector3[4];

normals[0] = -Vector3.forward;
normals[1] = -Vector3.forward;
normals[2] = -Vector3.forward;
normals[3] = -Vector3.forward;

mesh.normals = normals;

Vector2[] uv = new Vector2[4];

uv[0] = new Vector2(0, 0);
uv[1] = new Vector2(1, 0);
uv[2] = new Vector2(0, 1);
uv[3] = new Vector2(1, 1);

mesh.uv = uv;
*/
