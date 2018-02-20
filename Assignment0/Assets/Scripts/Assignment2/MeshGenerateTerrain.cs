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
    float xCord, zCord, oldxCord = 0, oldzCord = 0, oldYawCord = 0, minMovement = 0.1F;
    int yawYDegree, worldImageSize = 3;
    // Use this for initialization
    void Start()
    {
        // MESH
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        // Find controller and base
        Controller = GameObject.Find("Terrain_controller");
        Base = GameObject.Find("Terrain_base");
        image_controller = Controller.GetComponent<ImageTargetBehaviour>();
        image_base = Base.GetComponent<ImageTargetBehaviour>();
        // Configure height map
        Mat heightMapImg = Imgcodecs.imread("Assets/Assignment2/Textures/HeightMaps/mars_height_2.jpg");
        heightMap = new Mat();
        Imgproc.cvtColor(heightMapImg, heightMap, Imgproc.COLOR_RGB2GRAY);
        // Grab the render for stretching texture
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set field of view
        MatDisplay.SetCameraFoV(41.5f);
        // Only draw if both are tracked
        if (image_base.CurrentStatus == ImageTargetBehaviour.Status.TRACKED && image_controller.CurrentStatus == ImageTargetBehaviour.Status.TRACKED)
        {
            xCord = Controller.transform.position.x;
            zCord = Controller.transform.position.z;
            // Only draw if we are in positve area
            if (xCord > 0 && zCord > 0)
            {
                // Get controller angle
                yawYDegree = (int)Controller.transform.rotation.eulerAngles.y;

                // Only draw if movement is larger than X and in the positive area
                if (Mathf.Abs(xCord - oldxCord) > minMovement || Mathf.Abs(zCord - oldzCord) > minMovement || Mathf.Abs(yawYDegree - oldYawCord) > 1)
                {
                     // Generate the new mesh
                    generateMesh(xCord, zCord, yawYDegree);
                    oldxCord = xCord;
                    oldzCord = zCord;
                    oldYawCord = yawYDegree;
                }
            }
        } else
        {
            mesh.Clear();
        }
    }

    public void generateMesh(float x, float z, int yaw)
    {
        List<Vector3> verticesList = new List<Vector3>();
        List<int> triangles = new List<int>();
        // Clear mesh
        mesh.Clear();

        // Limit yaw angles 
        yaw = Mathf.Max(Mathf.Min(yaw, 90), 1);
        xCord = Mathf.Max(Mathf.Min(xCord, worldImageSize), 0);
        zCord = Mathf.Max(Mathf.Min(zCord, worldImageSize), 0);

        // Calculate amount of veriticies to be drawn
        float verticesPrWorldCoordinate = heightMap.rows() / worldImageSize;
        int xVertricies = (int)Mathf.Round(x * verticesPrWorldCoordinate);
        int zVertricies = (int)Mathf.Round(z * verticesPrWorldCoordinate);

        // Generate vertricies
        for (int i = 0; i <= xVertricies; i++)
        {
            for (int j = 0; j <= zVertricies; j++)
            {
                // Get heigh value
                heightMap.get(i,j, data);
                float heightVal = (data[0] - 128) / 255F;
                // Limit height 
                heightVal = Mathf.Max(Mathf.Min(heightVal, 1), 0);
                // Define verticy
                verticesList.Add(new Vector3((float)i / verticesPrWorldCoordinate, heightVal * (yaw / 180F), (float)j / verticesPrWorldCoordinate));
            }
        }
        // Generate triangles
        for (int i = 0; i <= xVertricies - 1; i++)
        {
            for (int j = 0; j <= zVertricies - 1; j++)
            {
                triangles.Add(i * (zVertricies + 1) + j);
                triangles.Add((i) * (zVertricies + 1) + (j + 1));
                triangles.Add((i + 1) * (zVertricies + 1) + (j));
                triangles.Add(i * (zVertricies + 1) + (j + 1));
                triangles.Add((i + 1) * (zVertricies + 1) + (j + 1));
                triangles.Add((i + 1) * (zVertricies + 1) + (j));
            }
        }
        // Draw uv
        Vector2[] uvs = new Vector2[verticesList.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(verticesList[i].x, verticesList[i].z);
        }

        // set values
        mesh.vertices = verticesList.ToArray();
        mesh.uv = uvs;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }


/*
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
    */
}
