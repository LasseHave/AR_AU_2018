using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalconCannon : MonoBehaviour {

    public float rayLenght = 10;
    public bool shoot_flag = false;
    private Ray ray;
    private Material material;

    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 1;
    public float radius = 3.0f;
    static Material lineMaterial;
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey("up")) {
            shoot_flag = true;
        }
        else {
            shoot_flag = false;
        }
        //ShootLaser();
       // Debug.DrawRay(transform.position, transform.forward * rayLenght);
        Debug.Log(ray.direction);
	}
    /*  void onRenderObject()
      {
          CreateLineMaterial();
          // Apply the line material
          lineMaterial.SetPass(0);
          GL.PushMatrix();
          GL.MultMatrix(transform.localToWorldMatrix);
          GL.Begin(GL.LINES);
          GL.Color(Color.red);
          GL.Vertex(transform.position);
          GL.Vertex(transform.position + transform.forward * rayLenght);
          GL.End();
          GL.PopMatrix();
          Debug.Log("I was here");
      }*/
    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if ((shoot_flag) && (this.isActiveAndEnabled))
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);
            //   for (int i = 0; i < lineCount; ++i)
            // {
            /*   float a = 1 / (float)lineCount;
               float angle = a * Mathf.PI * 2;
               // Vertex colors change from red to green
               GL.Color(new Color(a, 1 - a, 0, 0.8F));
               // One vertex at transform position
               GL.Vertex3(0, 0, 0);
               // Another vertex at edge of circle
               GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);*/
            GL.Color(Color.red);
            GL.Vertex(transform.position);
            GL.Vertex(transform.position + transform.forward * rayLenght);

            //}
            GL.End();
            GL.PopMatrix();
        }
    }
}
