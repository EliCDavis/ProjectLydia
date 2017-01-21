using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.Scenes.MainMenu {

	/// <summary>
	/// Takes Control of the emergency light and plays the error noise
	/// at the appropriate times.
	/// </summary>
	public class SceneBehavior : MonoBehaviour {

		[SerializeField]
		private Light emergencyLight;

		[SerializeField]
		private AudioSource sirenNoise; 

		/// <summary>
		/// How quickly the siren will flash.
		/// The number is arbitrary but the higher it is
		/// the faster it will flash
		/// </summary>
		[SerializeField]
		private float sirenSpeed = 4f;

		private bool canPlayNoise = false;

		void Update () {

			if (emergencyLight != null) {
				emergencyLight.color = new Color (.5f*Mathf.Sin(Time.time*sirenSpeed), 0, 0);
			}

			if(sirenNoise != null){

				if (Mathf.Sin (Time.time * sirenSpeed) > 0.8f) {

					if (canPlayNoise) {
						sirenNoise.Play ();
						canPlayNoise = false;
					}

				} else {
					canPlayNoise = true;
				}

			}

		}

	}

}