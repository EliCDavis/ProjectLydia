using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float mag;

	private Rigidbody rb;

	void Start() {
		rb = GetComponent<Rigidbody> ();
	}

	void FixedUpdate() {
		float move_hor = Input.GetAxis ("Horizontal");
		float move_ver = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (move_hor, 0.0f, move_ver);
		rb.AddForce (mag * movement);
	}

	void OnTriggerEnter(Collider enemy) {
		if (enemy.gameObject.tag == "Enemy") {
			enemy.gameObject.SetActive (false);
			gameObject.SetActive (false);
		}
	}
}
