using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class speech_script : MonoBehaviour {

    private string[] keywords = new string[] { "Red", "Yellow", "Black", "Blue", "Green" , "One", "Two", "Three", "Clean","Up","Down"};
    private ConfidenceLevel confidence = ConfidenceLevel.Medium;

    private GameObject QuadPost1, QuadPost2, QuadPost3, QuadPostCurrent;

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
            switch (post_id)
            {
                case "One":
                    QuadPostCurrent = QuadPost1;
                    index = 0;
                    break;
                case "Two":
                    QuadPostCurrent = QuadPost2;
                    index = 1;
                    break;
                case "Three":
                    QuadPostCurrent = QuadPost3;
                    index = 2;
                    break;
            }
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
