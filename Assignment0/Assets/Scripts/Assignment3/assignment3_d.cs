using Vuforia;
using UnityEngine;
using System.Collections.Generic;

public class assignment3_d : MonoBehaviour, IVirtualButtonEventHandler
{

    private GameObject keyboard;
    private GameObject A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, DEL, SPACE;
    private GameObject textField;
    public bool grabbed = true;
    public int counter = 0;
    private string text;

    void Start()
    {
        text = "";
        A = GameObject.Find("A");
        B = GameObject.Find("B");
        C = GameObject.Find("C");
        D = GameObject.Find("D");
        E = GameObject.Find("E");
        F = GameObject.Find("F");
        G = GameObject.Find("G");
        H = GameObject.Find("H");
        I = GameObject.Find("I");
        J = GameObject.Find("J");
        K = GameObject.Find("K");
        L = GameObject.Find("L");
        M = GameObject.Find("M");
        N = GameObject.Find("N");
        O = GameObject.Find("O");
        P = GameObject.Find("P");
        Q = GameObject.Find("Q");
        R = GameObject.Find("R");
        S = GameObject.Find("S");
        T = GameObject.Find("T");
        U = GameObject.Find("U");
        V = GameObject.Find("V");
        W = GameObject.Find("W");
        X = GameObject.Find("X");
        Y = GameObject.Find("Y");
        Z = GameObject.Find("Z");
        DEL = GameObject.Find("DEL");
        SPACE = GameObject.Find("SPACE");

        textField = GameObject.Find("PostIt1Text");


        VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; ++i)
        {
            vbs[i].RegisterEventHandler(this);
        }
        Debug.Log(vbs[0]);


    }

    // Update is called once per frame
    void Update()
    {

        if (grabbed == false)
        {
            counter += 1;
            if (counter > 60)
            {
                counter = 0;
                grabbed = true;
            }
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {

        if (grabbed)
        {
            grabbed = false;
            switch (vb.VirtualButtonName)
            {
                case "A":
                    addText("A");
                    break;
                case "B":
                    addText("B");
                    break;
                case "C":
                    addText("C");
                    break;
                case "D":
                    addText("D");
                    break;
                case "E":
                    addText("E");
                    break;
                case "F":
                    addText("F");
                    break;
                case "G":
                    addText("G");
                    break;
                case "H":
                    addText("H");
                    break;
                case "I":
                    addText("I");
                    break;
                case "J":
                    addText("J");
                    break;
                case "K":
                    addText("K");
                    break;
                case "L":
                    addText("L");
                    break;
                case "M":
                    addText("M");
                    break;
                case "N":
                    addText("N");
                    break;
                case "O":
                    addText("O");
                    break;
                case "P":
                    addText("P");
                    break;
                case "Q":
                    addText("Q");
                    break;
                case "R":
                    addText("R");
                    break;
                case "S":
                    addText("S");
                    break;
                case "T":
                    addText("T");
                    break;
                case "U":
                    addText("U");
                    break;
                case "V":
                    addText("V");
                    break;
                case "W":
                    addText("W");
                    break;
                case "X":
                    addText("X");
                    break;
                case "Y":
                    addText("Y");
                    break;
                case "Z":
                    addText("Z");
                    break;
                case "SPACE":
                    addText(" ");
                    break;
                case "DEL":
                    removeChar();
                    break;
            }
        }
    }

    public IEnumerable<WaitForSeconds> Waaait(float s)
    {
        yield return new WaitForSeconds(s);
    }
    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        //vbButtonObject.GetComponent<AudioSource>().Stop();
    }

    private void addText(string input)
    {
        text = text + input;
        textField.GetComponent<TextMesh>().text = text;
        Debug.Log(text);
    }
    private void removeChar()
    {
        if (text.Length > 0)
        {
            text = text.Remove(text.Length - 1);
            textField.GetComponent<TextMesh>().text = text;
            Debug.Log(text);
        }
    }
}
