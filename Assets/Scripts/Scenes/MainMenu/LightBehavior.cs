using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.Scenes.MainMenu {

	public class LightBehavior : MonoBehaviour {

		[SerializeField]
		private Light emergencyLight;

		void Update () {

			if (emergencyLight == null) {
				return;
			}

			emergencyLight.color = new Color (.5f*Mathf.Sin(Time.time*4), 0, 0);

		}

	}

}