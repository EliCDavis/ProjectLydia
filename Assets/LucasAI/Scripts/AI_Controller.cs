using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

	public GameObject player;
	public float vmax;
	public float mag;
	public float max_dist;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 enemyToPlayer = Vector3.Normalize(player.transform.position - transform.position);
		float theta = Mathf.Acos (Vector3.Dot (enemyToPlayer, transform.forward));
		if (enemyToPlayer.x*transform.position.z - enemyToPlayer.z*transform.position.x < 0)
			theta *= -1;
		if (Mathf.Abs (theta) > 0.01f) {
			transform.Rotate (0, theta, 0);
		}
		transform.Translate (transform.forward * Time.deltaTime, Space.World);
	}


}
