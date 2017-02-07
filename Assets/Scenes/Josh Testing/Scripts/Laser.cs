using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}


	void OnCollisionEnter(Collision c) {
		// Whenever the laser comes in contact with another collider, destroy it. 
		//Destroy(gameObject);

	}

}
