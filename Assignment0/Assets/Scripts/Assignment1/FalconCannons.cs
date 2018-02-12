using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalconCannons : MonoBehaviour
{
    // Define objects/ variables
    public GameObject particleObject, FrontCannon1, FrontCannon2, MainCannon, MainCannonTop, Falcon_Object;
    public ParticleSystem ParticleSystem;
    public bool shoot_flag = false, holdFire = true;
    static Material lineMaterial;
    public RaycastHit hit;
    public float rayLenght = 1, MainYaw = 0, MainPitch = 0;


    // Use this for initialization
    void Start()
    {
        // Setup Collision stuff
        particleObject = GameObject.Find("TieFighterExplosion");
        ParticleSystem = particleObject.GetComponent<ParticleSystem>();
        ParticleSystem.Stop();

        // Grab objects
        Falcon_Object = GameObject.Find("Falcon");
        FrontCannon1 = GameObject.Find("FrontCannon1");
        FrontCannon2 = GameObject.Find("FrontCannon2");
        MainCannon = GameObject.Find("MainCannon");
        MainCannonTop = GameObject.Find("MainCannonTop");
    }

    // Update is called once per frame
    void Update()
    {
        // Update cannon positions
        CannonRendering();
        // Check keys
        Keyboard_Checker();
     }

    public void Keyboard_Checker()
    {   // Check if shoot
        if (Input.GetKey("space"))
            shoot_flag = true;
        else
        {
            shoot_flag = false;
            holdFire = true;
        }
        // Adjust main cannon
        if (Input.GetKey("down") && MainPitch < 90 && MainPitch >= -90)
            MainPitch += 1;
        if (Input.GetKey("up") && MainPitch <= 90 && MainPitch > -90)
            MainPitch -= 1;
        if (Input.GetKey("right") && MainYaw < 90 && MainYaw >= -90)
            MainYaw += 1;
        if (Input.GetKey("left") && MainYaw <= 90 && MainYaw > -90)
            MainYaw -= 1;

    }
    public void CannonRendering()
    {
        // Shift Local positions to world space based on Falcon M Matrix
        Matrix4x4 M = Falcon_Object.transform.localToWorldMatrix;
        Vector3 scale = Falcon_Object.transform.localScale;
        // Front Cannon 1
        FrontCannon1.transform.position = M.MultiplyPoint3x4(new Vector3(
        0.004F * (1F / scale.x),
        0,
        0.048F * (1F / scale.z)));
        // Front Cannon 2
        FrontCannon2.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
        -0.004F * (1F / scale.x),
        0,
        0.048F * (1F / scale.z)));
        // Main Pipe
        MainCannon.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
        0,
        0.025F / 2 * (1F / scale.y),
        -0.009F * (1F / scale.z)));
        // Main top Cannon
        MainCannonTop.transform.position = Falcon_Object.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(
        0,
        0.025F * (1F / scale.y),
        -0.009F * (1F / scale.z)));

        // Set cannons rotation
        FrontCannon1.transform.rotation = Falcon_Object.transform.rotation;
        FrontCannon2.transform.rotation = Falcon_Object.transform.rotation;
        MainCannon.transform.rotation = Falcon_Object.transform.rotation;  // Include Yaw / Pitch Rotations
        MainCannonTop.transform.rotation = Falcon_Object.transform.rotation * Quaternion.Euler(MainPitch, MainYaw, 0);
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (shoot_flag)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            //Visualize Rays from Frontcannons
            VisualizeRay(FrontCannon1);
            VisualizeRay(FrontCannon2);
            VisualizeRay(MainCannonTop);

            //Shoot ray from cannons, if hit start explosion on hitpoint
            ShootRay(FrontCannon1);
            ShootRay(FrontCannon2);
            ShootRay(MainCannonTop);
        }
    }
    // Generate material for the ray
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
    // Visualize the laser rays using GL
    private void VisualizeRay(GameObject gameObject)
    {
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(gameObject.transform.position);
        GL.Vertex(gameObject.transform.position + gameObject.transform.forward * rayLenght);
        GL.End();
    }
    // Do the actual object deteciton with the physics Ray
    private void ShootRay(GameObject gameObject)
    {
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, rayLenght))
            PerformExplosion(hit.point);
        else
            holdFire = true;
    }
    // Start the explosion animation
    private void PerformExplosion(Vector3 hit_point)
    {
        if (holdFire)
        {
            particleObject.transform.position = hit_point;
            ParticleSystem.Play();
            holdFire = false;
        }
    }
}
