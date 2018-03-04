using Vuforia;
using UnityEngine;

public class assignment3_d : MonoBehaviour, IVirtualButtonEventHandler {

	private GameObject keyboard;
	private GameObject A;
	private GameObject B;


	void Start () {

		A = GameObject.Find("A");
		A = GameObject.Find("B");

		VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
		for(int i = 0; i < vbs.Length; ++i)
		{
			vbs[i].RegisterEventHandler(this);
		}
		Debug.Log(vbs[0]);


	}

	// Update is called once per frame
	void Update () {


	}

	public void OnButtonPressed(VirtualButtonBehaviour vb)
	{
		
		switch (vb.VirtualButtonName)
		{
		case "A":
			Debug.Log("A");
			break;
		}
		case "B":
		Debug.Log("A");
		break;
	}
		//vbButtonObject.GetComponent<AudioSource>().Play();
	}

	public void OnButtonReleased(VirtualButtonBehaviour vb)
	{
		//vbButtonObject.GetComponent<AudioSource>().Stop();
	}
}
