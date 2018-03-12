using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class speech_script : MonoBehaviour {

    private string[] keywords = new string[] { "Red", "Yellow", "Black", "Blue", "Green" , "One", "Two", "Three", "Clean","Up","Down"};
    private ConfidenceLevel confidence = ConfidenceLevel.Low;

    private GameObject QuadPost1, QuadPost2, QuadPost3, QuadPostCurrent, PostIt1, PostIt2, PostIt3, PostItCurrent;

    static Material lineMaterial;

    protected PhraseRecognizer recognizer;
    protected string word = "";
    protected string post_id = "One";
    protected string color_selected = "";

    private float[] brightness = { 1F, 1F, 1F };
    private Color[] color_idx = { Color.white, Color.white, Color.white};
    
    private int index = 0;

    private void Start()
    {

        QuadPost1 = GameObject.Find("Quad_post1");
        QuadPost2 = GameObject.Find("Quad_post2");
        QuadPost3 = GameObject.Find("Quad_post3");
        QuadPostCurrent = QuadPost1;
        PostIt1 = GameObject.Find("PostIt1"); ;
        PostIt2 = GameObject.Find("PostIt2"); ;
        PostIt3 = GameObject.Find("PostIt3"); ;
        PostItCurrent = PostIt1;

        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        word = args.text;

        if (word.Equals("One") || word.Equals("Two") || word.Equals("Three"))
        {
            post_id = word;
            Debug.Log("You selected post <b>" + post_id + "</b>");
            switch (post_id)
            {
                case "One":
                    QuadPostCurrent = QuadPost1;
                    PostItCurrent = PostIt1;
                    index = 0;
                    break;
                case "Two":
                    QuadPostCurrent = QuadPost2;
                    PostItCurrent = PostIt2;
                    index = 1;
                    break;
                case "Three":
                    QuadPostCurrent = QuadPost3;
                    PostItCurrent = PostIt3;
                    index = 2;
                    break;
            }
        }
        else if (word.Equals("Clean"))
        {
            color_idx[0] = Color.white;
            color_idx[1] = Color.white;
            color_idx[2] = Color.white;
            QuadPost1.GetComponent<Renderer>().material.color = Color.white;
            QuadPost2.GetComponent<Renderer>().material.color = Color.white;
            QuadPost3.GetComponent<Renderer>().material.color = Color.white;
            brightness[0] = 1F;
            brightness[1] = 1F;
            brightness[2] = 1F;
        } else
        {
            switch(word)
            {
                case "Red":
                   // QuadPostCurrent.GetComponent<Renderer>().material.color = Color.red;
                    color_idx[index] = Color.red;
                    break;
                case "Blue":
                    color_idx[index] = Color.blue;
                    //QuadPostCurrent.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                case "Black":
                    color_idx[index] = Color.black;
                    //QuadPostCurrent.GetComponent<Renderer>().material.color = Color.black;
                    break;
                case "Green":
                    color_idx[index] = Color.green;
                    //QuadPostCurrent.GetComponent<Renderer>().material.color = Color.green;
                    break;
                case "Yellow":
                    color_idx[index] = Color.yellow;
                    //QuadPostCurrent.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
            }
            switch(word)
            {
                case "Up":
                    brightness[index] = Mathf.Min(Mathf.Max(brightness[index] + 0.25F, 0), 1);
                    //QuadPostCurrent.GetComponent<Renderer>().material.color = color_idx[index] * brightness[index];
                    break;
                case "Down":
                    brightness[index] = Mathf.Min(Mathf.Max(brightness[index] - 0.25F, 0), 1);
                    break;
            }
            QuadPostCurrent.GetComponent<Renderer>().material.color = color_idx[index] * brightness[index];
            
            Debug.Log("You selected post <b>" + post_id + "</b> and color <b>" + word + "</b>" + " with brightness <b>" + brightness[index] + "</b>");
        }
  

    }
    private void DrawSelectedLines()
    {
        GL.Begin(GL.LINES);
        GL.Color(Color.white);
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(0.52F, 0.025F, 0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(0.52F, 0.025F, -0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(0.52F, 0.025F, -0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(-0.52F, 0.025F, -0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(-0.52F, 0.025F, -0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(-0.52F, 0.025F, 0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(-0.52F, 0.025F, 0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(0.52F, 0.025F, 0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(0.52F, 0.025F, 0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(-0.52F, 0.025F, -0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(-0.52F, 0.025F, 0.52F))));
        GL.Vertex(PostItCurrent.transform.localToWorldMatrix.MultiplyPoint3x4((new Vector3(0.52F, 0.025F, -0.52F))));
        GL.End();
    }


    // Will be called after all regular rendering is done
    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.skin.label.fontSize = 16;
        GUI.Label(new Rect(10, 10, 700, 100), "Select a post it note, saying: <One, Two, Three>");
        GUI.Label(new Rect(10, 30, 700, 100), "Select a color, saying: <Red, Blue, Black, Green, Yellow>");
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);
            
            DrawSelectedLines();
     
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
  

    private void Update()
    {

    }

    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
    }
}
