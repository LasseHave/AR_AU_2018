using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class assignment3_b : MonoBehaviour {

    private GameObject QuadPost1, QuadPost2;
    private GameObject Post1, Post2;
    static Material lineMaterial;
    private bool plot_line = false;
    private MeshCollider meshCollider;
    private BoxCollider boxCollider;

    public ImageTargetBehaviour post_2;
    public ImageTargetBehaviour post_1;

    // Use this for initialization
    void Start () {
        QuadPost1 = GameObject.Find("QuadP1");
        QuadPost2 = GameObject.Find("QuadP2");
        Post1 = GameObject.Find("PostIt_1");
        Post2 = GameObject.Find("PostIt_2");
        post_1 = Post1.GetComponent<ImageTargetBehaviour>();
        post_2 = Post2.GetComponent<ImageTargetBehaviour>();
        meshCollider = QuadPost1.GetComponent<MeshCollider>();
        boxCollider = QuadPost1.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update () {
      //  Debug.Log(boxCollider.enabled);
      if (post_1.CurrentStatus == ImageTargetBehaviour.Status.NOT_FOUND || post_2.CurrentStatus == ImageTargetBehaviour.Status.NOT_FOUND)
        {
            plot_line = false;
        }
        Debug.Log(plot_line);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision22!!!");
        if (plot_line)
        {
            plot_line = false;
        }
        else
        {
            plot_line = true;
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (plot_line)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            //Visualize Rays from Frontcannons
            VisualizeRay(QuadPost1, QuadPost2);
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
    private void VisualizeRay(GameObject gameObject1, GameObject gameObject2)
    {
        GL.Begin(GL.LINES);
        GL.Color(Color.green);
        GL.Vertex(gameObject1.transform.position);
        GL.Vertex(gameObject2.transform.position);
        GL.End();
    }
}
