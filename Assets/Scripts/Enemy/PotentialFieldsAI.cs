using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.Enemy {

	public class PotentialFieldsAI : MonoBehaviour {

		[SerializeField]
		GameObject target = null;

		private float targetAttractiveForce = 5;

		private float speed = 2f;

		public void SetTarget(GameObject target) {
			this.target = target;
		}


		float GetDistance(Vector3 dir) {
			Debug.DrawRay (transform.position, dir*3);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, dir, out hit, 100)) {
				return hit.collider.gameObject.GetInstanceID() == target.GetInstanceID() ? -1 : hit.distance;
			}
			return -1;
		}

		void Update () {

			if (target == null) {
				return;
			}

			Vector3 repulsion = Vector3.zero;

			for (int angle = -80; angle <= 80; angle+=10) {
				Vector3 dir = Quaternion.AngleAxis (angle, Vector3.up) * transform.forward;
				float dist = GetDistance (dir);
				if (dist  > 0 && dist < 10) {
					repulsion += (dir * (1f/dist));
				}
			}

			Vector3 movementDirection = Vector3.Normalize (( (target.transform.position - this.transform.position).normalized * targetAttractiveForce) - repulsion);
			movementDirection.y = 0;

			// Smoothly rotate towards the target point.
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementDirection), speed * Time.deltaTime);

			// Move forwards
			transform.Translate (Vector3.forward* Time.deltaTime * speed);

		}
	}

}