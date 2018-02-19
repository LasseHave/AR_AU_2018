using OpenCVForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

//[ExecuteInEditMode]
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
        Mat heightMapImg = Imgcodecs.imread("Assets/Assignment2/Textures/HeightMaps/mars_height_2.jpg");
        heightMap = new Mat();
        Imgproc.cvtColor(heightMapImg, heightMap, Imgproc.COLOR_RGB2GRAY);
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MatDisplay.SetCameraFoV(41.5f);
        if (image_base.CurrentStatus == ImageTargetBehaviour.Status.TRACKED && image_controller.CurrentStatus == ImageTargetBehaviour.Status.TRACKED)
        {
        float xCord = Controller.transform.position.x;
        float zCord = Controller.transform.position.z;
        int controller_rot = (int)Controller.transform.rotation.eulerAngles.y;
        int resolution = 200;
        if (xCord > 0 && zCord > 0)
        {
            mesh = generateMeshMatrix(xCord, zCord, controller_rot, resolution, mesh);
        }
       } else
        {
            mesh.Clear();
        }
    }

    public Mesh generateMeshMatrix(float x, float z, int rot, int resolution, Mesh mesh) // x,y are the lengths of the square making
    {
        mesh.Clear();
        rot = Mathf.Max(Mathf.Min(rot, 180), 1);
        int xint = (int)Mathf.Round(x * resolution);
        int zint = (int)Mathf.Round(z * resolution);
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
                heightVal = Mathf.Max(Mathf.Min(heightVal, 1), 0);



                verticesList.Add(new Vector3((float)i / resolution, heightVal * (rot / 360F), (float)j / resolution));
            }
        }
        // Generate triangles
        for (int i = 0; i <= xint - 1; i++)
        {
            for (int j = 0; j <= zint - 1; j++)
            {
                triangles.Add(i * (zint + 1) + j);
                triangles.Add((i) * (zint + 1) + (j + 1));
                triangles.Add((i + 1) * (zint + 1) + (j));
                triangles.Add(i * (zint + 1) + (j + 1));
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
        rend.material.mainTextureScale = new Vector2( 1/((float)x) * ((x * resolution)/heightMap.rows()),  1/((float)z) * ((z * resolution) / heightMap.cols()));
        mesh.vertices = verticesList.ToArray();
        mesh.uv = uvs;
        mesh.triangles = triangles.ToArray();

        // Return mesh
        return mesh;
    }
}
