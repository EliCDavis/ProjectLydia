using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

	public GameObject player;
	public float mag;
	public float max_dist;
	public float offset;

	[SerializeField]
	bool debug = false; 

	public enum wall_follow {
		STATE_FIND,
		STATE_FRONT,
		STATE_RIGHT,
		STATE_CORNER,
		STATE_LOST,
		STATE_BUFFER,
		STATE_FOUND,
	};
	private byte counter;
	private wall_follow state;
	private wall_follow last_state;

	private int health;

	[SerializeField]
	private GameObject explosionPrefab;


	public void SetPlayer(GameObject player) {
		this.player = player;
	}

	// Use this for initialization
	void Start () {
		state = wall_follow.STATE_FIND;
		counter = 0;
		health = 100;
	}
	
	// Update is called once per frame
	void Update () {

		return;

		if (player == null || debug) {
			return;
		}

		last_state = state;
		RaycastHit front, wall, play;
		float front_dist = float.MaxValue;
		float wall_dist = float.MaxValue;
		float t = -Mathf.PI / 16;

		Vector3 fwd = transform.forward;
		Vector3 rt = transform.right;

		Vector3 e2p = player.gameObject.transform.position - transform.position;
		float theta = Mathf.Acos (Vector3.Dot (transform.forward, Vector3.Normalize (e2p)));

		if (Vector3.Cross (transform.forward, e2p).magnitude > 0) {
			theta *= -1;
		}

		// Is there a straight shot at the player
		if (Physics.Raycast (transform.position, e2p, out play, max_dist, 9)) {
			if (play.distance <= e2p.magnitude + offset + 0.1f && play.distance >= e2p.magnitude + offset - 0.1f) {
				if (counter < 2) {
					state = wall_follow.STATE_BUFFER;
				}
			}
		} else {
			if (last_state == wall_follow.STATE_FOUND || last_state == wall_follow.STATE_BUFFER) {
				state = wall_follow.STATE_FIND;
			}
		}

		// Wall follow
		while (t <= Mathf.PI / 16 + 0.01f) {

			Vector3 v1 = new Vector3 (Mathf.Cos (t) * fwd.x - Mathf.Sin (t) * fwd.z, 0, Mathf.Sin (t) * fwd.x + Mathf.Cos (t) * fwd.z);
			Vector3 v2 = new Vector3 (Mathf.Cos (t) * rt.x - Mathf.Sin (t) * rt.z, 0, Mathf.Sin (t) * rt.x + Mathf.Cos (t) * rt.z);

			Debug.DrawRay (transform.position, v1);

			Debug.DrawRay (transform.position, v2);

			if (Physics.Raycast (transform.position, v1, out front, max_dist, 9)) {
				if (front.distance < front_dist) {
					front_dist = front.distance;
				}
			}

			if (Physics.Raycast (transform.position, v2, out wall, max_dist, 9)) {
				if (wall.distance < wall_dist) {
					wall_dist = wall.distance;
				}
			}
			t += Mathf.PI / 16;
		}


		switch (state) {

		case wall_follow.STATE_FIND:
			if (front_dist <= max_dist / 2) {
				if (wall_dist < max_dist / 2) {
					state = wall_follow.STATE_CORNER;
				} else {
					state = wall_follow.STATE_FRONT;
				}
			}
			if (wall_dist <= max_dist / 2) {
				state = wall_follow.STATE_RIGHT;
			}
			transform.Translate (transform.forward * mag * Time.deltaTime, Space.World);
			break;
		
		case wall_follow.STATE_FRONT:
			if (wall_dist <= max_dist / 2) {
				state = wall_follow.STATE_RIGHT;
			}
			transform.Rotate (-transform.up * 10.0f * mag * Time.deltaTime);
			break;

		case wall_follow.STATE_RIGHT:
			if (wall_dist > max_dist / 2) {
				state = wall_follow.STATE_LOST;
			} else if (front_dist <= max_dist / 2) {
				state = wall_follow.STATE_CORNER;
			}
			transform.Rotate (-transform.up * 0.5f * mag * Time.deltaTime);
			transform.Translate (transform.forward * mag * Time.deltaTime, Space.World);
			break;

		case wall_follow.STATE_CORNER:
			if (front_dist >= max_dist - 0.05f) {
				state = wall_follow.STATE_RIGHT;
			}

			transform.Rotate (-transform.up * 10.0f * mag * Time.deltaTime);
			break;

		case wall_follow.STATE_LOST:
			if (front_dist <= max_dist / 2) {
				state = wall_follow.STATE_FRONT;
			}

			transform.Translate (transform.forward * mag * Time.deltaTime, Space.World);
			transform.Rotate (transform.up * mag * Time.deltaTime);
			break;
		
		
		case wall_follow.STATE_BUFFER:
			if (counter < 2) {
				counter++;
			} else {
				state = wall_follow.STATE_FOUND;
			}

			transform.Translate (transform.forward * mag * Time.deltaTime, Space.World);
			break;

		case wall_follow.STATE_FOUND:
			if (Mathf.Abs (theta) > 0.1f) {
				if (theta > 0) {
					transform.Rotate (transform.up * 10.0f * mag * Time.deltaTime, Space.World);
				} else {
					transform.Rotate (-transform.up * 10.0f * mag * Time.deltaTime, Space.World);
				}
			} else if (Mathf.Abs (theta) < Mathf.PI / 3) {
				transform.Translate (Vector3.Normalize (e2p) * mag * Time.deltaTime, Space.World);
			}

			break;
		}
	}

	public void Damage(int damage) {
		this.health = Mathf.Max(0, this.health - damage);
		Debug.Log(this.health);
		Debug.Log(damage);
		if (this.health == 0) {
			Explode();
		}
	}

	void OnCollisionEnter(Collision c) {
		if (c.gameObject.tag == "Player") {
			c.gameObject.GetComponent<PlayerScript>().Damage(3);
			Explode(); 
		}
	}

	void Explode() {
		GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		//AudioSource explosionSound = Instantiate(Sound.Load("explosion", AudioSource), transform.position, Quaternion.identity);
		//Destroy(explosionSound, 2f);
		Destroy(explosionInstance, 1.98f);
		Destroy(gameObject);
	}
}
