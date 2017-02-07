using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lydia.Scenes.Factory
{
	
	public class HUD : MonoBehaviour {

		PlayerScript player = null;

		[SerializeField]
		Slider healthBar;

		public void SetPlayer(PlayerScript player) {
			this.player = player;
			healthBar.minValue = 0f;
			healthBar.maxValue = (float)player.GetMaxHealth ();
			Debug.Log ("Max Health: " + player.GetMaxHealth());
		}

		void Update() {

			if (player == null) {
				return;
			}

			healthBar.value = player.GetHealth ();

		}

	}

}