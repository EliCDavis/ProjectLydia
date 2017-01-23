using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

	public Animator anim;
	public Rigidbody rbody;
	private bool run;
	private bool shoot; 

	private float inputH;
	private float inputV;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rbody = GetComponent<Rigidbody>();
		run = false;
		shoot = false;
	}
	
	// Update is called once per frame
	void Update () {

		// If Player enters '1', then the wave animation will play. 
		if (Input.GetKeyDown("1")) {
			anim.Play("Wave", -1, 0f);
		}

		if (Input.GetKeyDown("2")) {

			int random = Random.Range(0,2);

			if (random == 0) {
				anim.Play("DAMAGED00", -1, 0f);
			} else {
				anim.Play("DAMAGED01", -1, 0f);
			} 
		}

		if (Input.GetKey(KeyCode.LeftShift)) {
			run = true;
		} else {
			run = false;
		}

		if (Input.GetMouseButtonDown(0) && !run) {
			anim.Play("assault_combat_shoot", -1, 0f);
			shoot = true;
		}

		if (Input.GetMouseButtonDown(1)) {
			//anim.Play("assault_combat_shoot_burst", -1, 0f);
			shoot = true;
		}

		inputH = Input.GetAxis("Horizontal");
		inputV = Input.GetAxis("Vertical");

		anim.SetFloat("inputH", inputH);
		anim.SetFloat("inputV", inputV);
		anim.SetBool("run", run);
		anim.SetBool("shoot", shoot);

		float moveX = inputH*2f*Time.deltaTime;
		float moveZ = inputV*2f*Time.deltaTime;

		if (moveZ <= 0f) {
			moveX = 0f;
		} else if (run) {
			moveX*=3f;
			moveZ*=3f;
		}

		CorrectPosition();
		transform.Translate(new Vector3(moveX, 0, moveZ));
		shoot = false;
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
