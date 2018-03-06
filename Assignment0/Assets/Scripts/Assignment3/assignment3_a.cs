using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class assignment3_a : MonoBehaviour {

	public string[] keywords = new string[] { "up", "blue", "green", "yellow", "purple" };
	public ConfidenceLevel confidence = ConfidenceLevel.Low;

	public KeywordRecognizer recognizer;
	protected string word = "red";

	// Follows this
	// https://lightbuzz.com/speech-recognition-unity/
	// Use this for initialization
	void Start () {
		if (keywords != null)
		{
			recognizer = new KeywordRecognizer(keywords, confidence);
			recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
			recognizer.Start();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
	{
		word = args.text;
		Debug.Log("You said: <b>" + word + "</b>");
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
