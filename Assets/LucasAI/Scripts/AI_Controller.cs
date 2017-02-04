using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

	public GameObject player;
	public float mag;
	public float max_dist;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 e2p = Vector3.Normalize(player.transform.position - transform.position);
		float t = Mathf.Acos (Vector3.Dot (e2p, transform.forward));

		if (transform.forward.x * e2p.z - transform.forward.z * e2p.x > 0) {
			t *= -1;
		}

		RaycastHit hit;
		float theta = 0;
		Vector3 sum = new Vector3 (0, 0, 0);
		while (theta <= Mathf.PI + 0.01f) {
			Vector3 r = transform.right;
			float sin = Mathf.Sin (theta);
			float cos = Mathf.Cos (theta);
			Vector3 dir = new Vector3 (cos * r.x - sin * r.z, 0, sin * r.x + cos * r.z);
			if (Physics.Raycast (transform.position, dir, out hit, max_dist)) {
				Debug.Log ("Hi");
				sum -= dir;
			}
			theta += Mathf.PI / 8;
		}

		if (Mathf.Abs (t) > 0.01f) {
			transform.Rotate (0, mag * t, 0);
		}

		sum += transform.forward;

		if (player.activeSelf) {
			transform.Translate (sum * mag  * Time.deltaTime, Space.World);
		}
	}
}
