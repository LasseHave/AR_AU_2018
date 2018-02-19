using OpenCVForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

[ExecuteInEditMode]
public class MeshGenerateTerrain : MonoBehaviour
{
    public GameObject Controller, Base;
    public Mesh mesh;
    public ImageTargetBehaviour image_base, image_controller;
    private Mat heightMap;
    public Renderer rend;
    private byte[] data = new byte[1];
    // Use this for initialization
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        Controller = GameObject.Find("Terrain_controller");
        Base = GameObject.Find("Terrain_base");
        image_base = Base.GetComponent<ImageTargetBehaviour>();
        image_controller = Controller.GetComponent<ImageTargetBehaviour>();
        Mat heightMapImg = Imgcodecs.imread("Assets/Assignment2/Textures/HeightMaps/height_map1.jpg");
        heightMap = new Mat();
        Imgproc.cvtColor(heightMapImg, heightMap, Imgproc.COLOR_RGB2GRAY);
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (image_base.CurrentStatus == ImageTargetBehaviour.Status.TRACKED && image_controller.CurrentStatus == ImageTargetBehaviour.Status.TRACKED)
        //{
            float xCord = Controller.transform.position.x;
            float zCord = Controller.transform.position.z;
            mesh = generateMeshMatrix(xCord, zCord, mesh);
        //} else
        //{
        //    mesh.Clear();
        //}
    }

        public Mesh generateMeshMatrix(float x, float z, Mesh mesh) // x,y are the lengths of the square making
    {
        mesh.Clear();
        int xint = (int)Mathf.Round(x*10);
        int zint = (int)Mathf.Round(z*10);
        //int vertexCount = (xint+1)* (zint+1);
        //int triangleCount = xint * zint * 2 * 3;
        List<Vector3> verticesList = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate vertricies
        for (int i = 0; i <= xint; i++)
        {
            for (int j = 0; j <= zint; j++)
            {
                heightMap.get((int)i, (int)j, data);
                float heightVal = (data[0] - 128) / 255F;
                verticesList.Add(new Vector3(i/10F, heightVal, j/10F));
            }
        }
        // Generate triangles
        for (int i = 0; i <= xint - 1; i++)
        {
            for (int j = 0; j <= zint - 1; j++)
            {
                triangles.Add(i * (zint + 1) + j);
                triangles.Add((i) * (zint + 1) + (j+1));
                triangles.Add((i + 1) * (zint + 1) + (j));
                triangles.Add(i * (zint + 1) + (j+1));
                triangles.Add((i + 1) * (zint + 1) + (j + 1));
                triangles.Add((i + 1) * (zint + 1) + (j));
            }
        }
        // Draw uv
        Vector2[] uvs = new Vector2[verticesList.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(verticesList[i].x, verticesList[i].z);
        }
        // set values
        rend.material.mainTextureScale = new Vector2((float)1/(xint/10), (float)1/(zint/10));
        mesh.vertices = verticesList.ToArray();
        mesh.uv = uvs;
        mesh.triangles = triangles.ToArray();
        // Return mesh
        return mesh;
    }
}
