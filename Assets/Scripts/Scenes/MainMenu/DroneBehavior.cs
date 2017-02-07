using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.Scenes.MainMenu {

	public class DroneBehavior : MonoBehaviour {

		/// <summary>
		/// So if there's multiple drones in the scene, they will start floating up
		/// and down at a different starting point so their not weirdly synced up
		/// </summary>
		float startingDisplacement;

		/// <summary>
		/// These drone's blades don't animate on their own, luckily it's just a matter
		/// of rotating them which is easy to do.
		/// </summary>
		[SerializeField]
		Transform[] bladesToRotate;

		void Start() {
			startingDisplacement = Random.Range (0, 10);
		}

		void Update () {

			// Add hovering effect floating up and down
			transform.Translate (Vector3.up*Mathf.Sin(Time.time +startingDisplacement)*Time.deltaTime*.5f);

			// Look at the mouse
			transform.LookAt (Camera.main.transform.position + (Camera.main.ScreenPointToRay(Input.mousePosition).direction * 2));

			// Animate the blades
			foreach(Transform blade in bladesToRotate){
				blade.Rotate (0, 1080 * Time.deltaTime, 0);
			}

		}

	}

}