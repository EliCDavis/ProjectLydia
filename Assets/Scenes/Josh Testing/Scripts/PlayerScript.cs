using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {
	SingleShot,
	DualShot,
	TriShot
}

public class PlayerScript : MonoBehaviour {

	[SerializeField]
	public float speedMultiplier;

	private Animator anim;
	private Rigidbody rbody;

	private float inputV;
	private float move;

	private GameObject bulletReference;

	private GunType currentGunType = GunType.SingleShot;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rbody = GetComponent<Rigidbody>();
		speedMultiplier = 6f;

		// Load the bullet reference from our prefabs
		bulletReference = Resources.Load("laser") as GameObject;

	}
	
	// Update is called once per frame
	void Update () {
		inputV = Input.GetAxis("Vertical");
		anim.SetFloat("inputV", inputV);

		move = inputV*speedMultiplier;
		
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
			Shoot(currentGunType, move);
			if (inputV == 0) {
				anim.Play("assault_combat_shoot", -1, 0f);
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			Shoot(GunType.TriShot, move);
			if (inputV == 0) {
				anim.Play("assault_combat_shoot_burst", -1, 0f);
			}
		}

		CorrectPosition();
		transform.Translate(new Vector3(0, 0, move*Time.deltaTime));
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

	void Shoot(GunType fireType){
		Shoot(fireType, 0);
	}

	void Shoot(GunType fireType, float initialSpeed) {

		switch(fireType) {
			case GunType.SingleShot:
				ShootSingleShot(initialSpeed);
			break;
			case GunType.DualShot:

			break;
			case GunType.TriShot:

			break;
		}

	}

	/// <summary>
	/// Fires a single shot, the most basic type of firing a gun.
	/// </summary>
	void ShootSingleShot(float initialSpeed) {

		// Create the correct bullet spawn position 
		Vector3 spawnPosition = transform.position + (transform.forward);
		spawnPosition.y = transform.position.y + 1;

		// Instantiate the bullet
		GameObject laserInstance = Instantiate(bulletReference, spawnPosition, transform.rotation);

		// Move the bullet appropriatly
		Rigidbody bulletBody = laserInstance.GetComponent<Rigidbody>();
		bulletBody.velocity = laserInstance.transform.forward*initialSpeed;
		bulletBody.AddForce(transform.forward*500);	

	}

	void OnTriggerEnter(Collider enemy) {
		if (enemy.gameObject.name == "Enemy1") {
			anim.Play("DAMAGED00", -1, 0f);
		} else if (enemy.gameObject.name == "Enemy2") {
			anim.Play("DAMAGED01", -1, 0.6f);
		}
	}

}
