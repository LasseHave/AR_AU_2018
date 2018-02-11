using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalconCannon : MonoBehaviour
{
    public float rayLenght = 5;
    public bool shoot_flag = false, hasCollided = false;
    public GameObject particleObject;
    public ParticleSystem ParticleSystem;
    public RaycastHit hit;
    static Material lineMaterial;

    // Use this for initialization
    void Start()
    {
        particleObject = GameObject.Find("TieFighterExplosion");
        ParticleSystem = particleObject.GetComponent<ParticleSystem>();
        ParticleSystem.Stop();
    }

    private void performExplosion(Vector3 hit_point)
    {
        if (!hasCollided)
        {
            particleObject.transform.position = hit_point;// new Vector3(0,0,0);
            ParticleSystem.Play(); hasCollided = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
            shoot_flag = true;
        else
            shoot_flag = false;

    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (shoot_flag)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + transform.forward * rayLenght);
            GL.End();

            if (Physics.Raycast(transform.position, transform.forward, out hit, rayLenght))
            {
                Debug.Log("Found an object - distance: " + hit.distance + hit.point);
                Debug.Log("Found an object - Point: " + hit.point);
                Debug.DrawLine(transform.position, hit.point);
                performExplosion(hit.point);
            }
            else
                hasCollided = false;
        }
        else
            hasCollided = false;
    }

  

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
}