using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorCollision : MonoBehaviour {

	private GameObject sun;
	private GameObject earth;
	public ParticleSystem particleObject;
	private bool hasCollided;


	// Use this for initialization
	void Start () {
		particleObject = GetComponentInChildren<ParticleSystem> ();
		particleObject.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		sun = GameObject.Find ("Sun");
		earth = GameObject.Find ("Earth");

		var earthPos = earth.transform.position;
		var meteorPos = transform.position;

		double dist = Mathf.Sqrt (Mathf.Pow (earthPos.x - meteorPos.x, 2)
			+ Mathf.Pow (earthPos.y - meteorPos.y, 2)
			+ Mathf.Pow (earthPos.z - meteorPos.z, 2));

		float radMeteor = transform.localScale.x * 0.5F;
		float radEarth = earth.transform.localScale.x * sun.transform.localScale.x * 0.5F;

		if (dist < (radEarth + radMeteor)) {
			var collisionPoint = ((meteorPos - earthPos) * radEarth) + earthPos;
			Debug.Log ("They collided!");

			if (!hasCollided) {
				particleObject.transform.position = collisionPoint;
				particleObject.Play ();
				hasCollided = true;
			}
				
		} else {
			hasCollided = false;
		}

	}
}
