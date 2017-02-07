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
		currentGunType = GunType.SingleShot;

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

	void Shoot(){
		Shoot(GunType.SingleShot, 0);
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
				ShootDualShot(initialSpeed);
			break;

			case GunType.TriShot:
				ShootTriShot(initialSpeed);
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
		laserInstance.GetComponent<Laser>().SetDamage(10);	

	}
	/*
	private Quaternion RotatePoint(Vector3 angle, Vector3 point){
		return Quaternion.Euler(angle) * point;
	}

	private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle){
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(angle) *dir;
		return dir + pivot;
	}
	*/

	/// <summary>
	/// Fires a dual shot.
	/// </summary>
	void ShootDualShot(float initialSpeed) {
		 
		// Create the correct bullet spawn position 
		//Vector3 spawnPosition1 = RotatePointAroundPivot(transform.position + transform.forward, transform.position, new Vector3(0, -45, 0));
		//Vector3 spawnPosition2 = RotatePointAroundPivot(transform.position + transform.forward, transform.position, new Vector3(0, 45, 0));
		Vector3 spawnPosition1 = transform.position + transform.forward;
		Vector3 spawnPosition2 = transform.position + transform.forward;

		spawnPosition1.y = transform.position.y + 1;
		spawnPosition1.x = transform.position.x + 0.20f;
		spawnPosition2.y = transform.position.y + 1;
		spawnPosition2.x = transform.position.x - 0.20f;

		// Instantiate the bullet
		//GameObject laserInstance1 = Instantiate(bulletReference, spawnPosition1,  RotatePoint(new Vector3(0, -45, 0), transform.position) );
		//GameObject laserInstance2 = Instantiate(bulletReference, spawnPosition2,  RotatePoint(new Vector3(0, 45, 0),  transform.position) );

		float angle = transform.eulerAngles.y;
		float rotation = 1 ;

		if (angle > 180) {
			angle -= 360;
		}

		if (angle > 130 || angle < -130) {
			rotation = 1;
		}
		print(angle);
		GameObject laserInstance1 = Instantiate(bulletReference, spawnPosition1, transform.rotation*=Quaternion.Euler(0, -10f*rotation, 0));
		GameObject laserInstance2 = Instantiate(bulletReference, spawnPosition2, transform.rotation*=Quaternion.Euler(0, 20f*rotation, 0));

		// Move the bullet appropriatly
		Rigidbody bulletBody1 = laserInstance1.GetComponent<Rigidbody>();
		Rigidbody bulletBody2 = laserInstance2.GetComponent<Rigidbody>();

		bulletBody1.velocity = laserInstance1.transform.forward*initialSpeed;
		bulletBody2.velocity = laserInstance2.transform.forward*initialSpeed;

		bulletBody1.AddForce(laserInstance1.transform.forward*500);
		bulletBody2.AddForce(laserInstance2.transform.forward*500);

		laserInstance1.GetComponent<Laser>().SetDamage(10);
		laserInstance2.GetComponent<Laser>().SetDamage(10);

	}

	/// <summary>
	/// Fires a tri shot.
	/// </summary>
	void ShootTriShot(float initialSpeed) {
		 
		Vector3 spawnPosition1 = transform.position + (transform.forward);
		Vector3 spawnPosition2 = transform.position + (transform.forward);


		spawnPosition1.y = transform.position.y + 1;
		//spawnPosition1.x = transform.position.x - 0.10f;
		spawnPosition2.y = transform.position.y + 1;
		//spawnPosition2.x = transform.position.x + 0.10f;

		// Instantiate the bullet
		GameObject laserInstance1 = Instantiate(bulletReference, spawnPosition1, transform.rotation);
		GameObject laserInstance2 = Instantiate(bulletReference, spawnPosition2, transform.rotation);

		// Move the bullet appropriatly
		Rigidbody bulletBody1 = laserInstance1.GetComponent<Rigidbody>();
		Rigidbody bulletBody2 = laserInstance2.GetComponent<Rigidbody>();

		bulletBody1.velocity = laserInstance1.transform.forward*initialSpeed;
		bulletBody2.velocity = laserInstance2.transform.forward*initialSpeed;

		bulletBody1.AddForce(transform.forward*500);
		bulletBody2.AddForce(transform.forward*500);

		laserInstance1.GetComponent<Laser>().SetDamage(10);
		laserInstance2.GetComponent<Laser>().SetDamage(10);

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

	void OnTriggerEnter(Collider enemy) {
		if (enemy.gameObject.name == "Enemy1") {
			anim.Play("DAMAGED00", -1, 0f);
		} else if (enemy.gameObject.name == "Enemy2") {
			anim.Play("DAMAGED01", -1, 0.6f);
		}
	}

}
