using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshGenerateTerrain : MonoBehaviour
{
    private GameObject Controller, Base;
    private Mesh mesh;
    // Use this for initialization
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        Controller = GameObject.Find("Terrain_controller");
        Base = GameObject.Find("Terrain_base");
    }

    // Update is called once per frame
    void Update()
    {
        mesh = generateMeshMatrix(4, 4, mesh);
    }

    public Mesh generateMeshMatrix(float x, float z, Mesh mesh) // x,y are the lengths of the square making
    {
        mesh.Clear();
        int xint = (int)Mathf.Round(x);
        int zint = (int)Mathf.Round(z);
        int counter = 0;
        int vertexCount = (xint+1)* (zint+1);
        int triangleCount = xint * zint * 2 * 3;
        Vector3[] vertricies = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];

        // Generate vertricies
        for (int i = 0; i <= xint; i++)
        {
            for (int j = 0; j <= zint; j++)
            {
                vertricies[counter] = new Vector3(i, 0, j);
                counter++;
            }
        }
        counter = 0;
        // Generate triangles
        for (int i = 0; i <= xint - 1; i++)
        {
            for (int j = 0; j <= zint - 1; j++)
            {
                triangles[counter] = i * (xint+1) + j;
                counter++;
                triangles[counter] = (i + 1) * (xint + 1) + (j + 1);
                counter++;
                triangles[counter] = (i + 1) * (xint + 1) + j;
                counter++;
                triangles[counter] = i * (xint + 1) + j;
                counter++;
                triangles[counter] = i * (xint + 1) + (j + 1);
                counter++;
                triangles[counter] = (i + 1) * (xint + 1) + (j + 1);
                counter++;
            }
        }
        // Draw uv
        Vector2[] uvs = new Vector2[vertricies.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertricies[i].x, vertricies[i].z);
        }
        // set values
        mesh.vertices = vertricies;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        // Return mesh
        return mesh;
    }
}
