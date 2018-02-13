using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerateSpaceShip : MonoBehaviour
{

    float width;
    float height;
    Mesh mesh;
    MeshFilter meshFilter;

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

        mesh.Clear();

        mesh.vertices = new Vector3[] {
            new Vector3(0, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), // Point 0,1,2
            new Vector3(2, 0, 0), new Vector3(2, 0, 1), new Vector3(2, 0, 2), // Point 3,4,5
            new Vector3(3, 0, 0), new Vector3(2.5F, 0, -1), new Vector3(3, 0, 1),// Point 6,7,8
            new Vector3(4, 0, 0), new Vector3(4, 0, 1), new Vector3(5, 0, 0),// Point 9,10,11
            new Vector3(3, 0, 2), new Vector3(2, 0, 3), new Vector3(3, 0, 3),// Point 12,13,14
            new Vector3(2, 0, 4), new Vector3(3, 0, 4), new Vector3(2.5F, 0, 5)// Point 15,16,17
        };
        
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
        /*Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;
        */
        Debug.Log("here");
    }

}
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
