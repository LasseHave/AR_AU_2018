using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour {
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("This works");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
