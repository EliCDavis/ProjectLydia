using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	public Animator anim;
	public Rigidbody rbody;
	public Laser laser;
	public float speedMultiplier;
	private float inputV;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rbody = GetComponent<Rigidbody>();
		speedMultiplier = 6f;
	}
	
	// Update is called once per frame
	void Update () {
		inputV = Input.GetAxis("Vertical");
		anim.SetFloat("inputV", inputV);

		float move = inputV*speedMultiplier*Time.deltaTime;
		
		if (move < 0f) {
			speedMultiplier = 2f;
		} else {
			speedMultiplier = 6f;
		}
		
		if (Input.GetKeyDown("2")) {

			int random = Random.Range(0,2);

			if (random == 0) {
				anim.Play("DAMAGED00", -1, 0f);
			} else {
				anim.Play("DAMAGED01", -1, 0f);
			} 
		}

		if (Input.GetMouseButtonDown(0)) {
			if (inputV == 0) {
				anim.Play("assault_combat_shoot", -1, 0f);
			}

			laser.testing();
		}

		if (Input.GetMouseButtonDown(1)) {
			if (inputV == 0) {
				anim.Play("assault_combat_shoot_burst", -1, 0f);
			}
		}

		CorrectPosition();
		transform.Translate(new Vector3(0, 0, move));
	}

	void CorrectPosition() {
        Vector3 relativePos = Input.mousePosition;
        relativePos.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        relativePos = Camera.main.ScreenToWorldPoint(relativePos);
        transform.LookAt(relativePos);
        Vector3 tempAngle = transform.eulerAngles;
        tempAngle.x = 0;
        tempAngle.z = 0;
        transform.eulerAngles = tempAngle;
	}

	void OnTriggerEnter(Collider enemy) {
		if (enemy.gameObject.name == "Enemy1") {
			anim.Play("DAMAGED00", -1, 0f);
		} else if (enemy.gameObject.name == "Enemy2") {
			anim.Play("DAMAGED01", -1, 0.6f);
		}
	}

}
