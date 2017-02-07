using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType {
	Power1,
	Power2,
	Power3
}


public class PlayerScript : MonoBehaviour {

	[SerializeField]
	public float speedMultiplier;

	private Animator anim;
	private Rigidbody rbody;

	private float inputV;
	private float move;
	private int damageMultiplier;

	private GameObject bulletReference;

	[SerializeField]
	private GunType currentGunType;

	private int health;
	private int maxHealth = 100;

	// Use this for initialization
	void Start () {
		health = maxHealth;
		anim = GetComponent<Animator>();
		rbody = GetComponent<Rigidbody>();
		speedMultiplier = 6f;

		// Start the game with a single shot
		currentGunType = GunType.Power1;

		// Load the bullet reference from our prefabs
		bulletReference = Resources.Load("laser") as GameObject;

	}
	
	void Update () {
		inputV = Input.GetAxis("Vertical");
		anim.SetFloat("inputV", inputV);

		move = inputV*speedMultiplier;
		
		if (move < 0f) {
			speedMultiplier = 2f;
		} else {
			speedMultiplier = 6f;
		}
	

		if (Input.GetMouseButtonDown(0)) {
			Shoot(currentGunType, move);

			// The following audio clip was found at https://www.youtube.com/watch?v=qeAzuWw2v9o
			// It was stripped from the video and converted to the format we needed. 
			AudioSource laserClip = GetComponent<AudioSource>();
			laserClip.Play();
			if (inputV == 0) {
				anim.Play("assault_combat_shoot", -1, 0f);
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

	void Shoot(){
		Shoot(GunType.Power1, 0);
	}

	void Shoot(GunType fireType){
		Shoot(fireType, 0);
	}

	void Shoot(GunType fireType, float initialSpeed) {

		switch(fireType) {
			case GunType.Power1:
				damageMultiplier = 1;
				ShootSingleShot(initialSpeed, damageMultiplier);
			break;

			case GunType.Power2:
				damageMultiplier = 2;
				ShootSingleShot(initialSpeed, damageMultiplier);
			break;

			case GunType.Power3:
				damageMultiplier = 3;
				ShootSingleShot(initialSpeed, damageMultiplier);
			break;
		}

	}

	/// <summary>
	/// Fires a single shot, the most basic type of firing a gun.
	/// </summary>
	void ShootSingleShot(float initialSpeed, int damageMultiplier) {
		 
		// Create the correct bullet spawn position 
		Vector3 spawnPosition = transform.position + (transform.forward);
		spawnPosition.y = transform.position.y + 1;

		// Instantiate the bullet
		GameObject laserInstance = Instantiate(bulletReference, spawnPosition, transform.rotation);

		// Move the bullet appropriatly
		Rigidbody bulletBody = laserInstance.GetComponent<Rigidbody>();
		bulletBody.velocity = laserInstance.transform.forward*initialSpeed;
		bulletBody.AddForce(transform.forward*500);
		laserInstance.GetComponent<Laser>().SetDamage(10*damageMultiplier);	

	}

	public int GetHealth() {
		return this.health;
	}

	public int GetMaxHealth() {
		return this.maxHealth;
	}

	public void Damage(int damage) {
		this.health = Mathf.Max(0, this.health - damage);
		anim.Play("DAMAGED00", -1, 0f);
	}

}
