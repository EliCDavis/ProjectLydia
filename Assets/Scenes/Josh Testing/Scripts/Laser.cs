using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

	private int damage;

	public void SetDamage(int damage) {
		this.damage = damage;
	}


	void OnCollisionEnter(Collision c) {
		if (c.gameObject.tag == "Enemy") {
			c.gameObject.GetComponent<AI_Controller>().Damage(damage); 
			Destroy(gameObject);
		}
		// Whenever the laser comes in contact with another collider, destroy it. 


	}

}
