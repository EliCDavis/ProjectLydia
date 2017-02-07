using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lydia.Scenes.Factory
{
	
	public class HUD : MonoBehaviour {

		/// <summary>
		/// The paused menu.
		/// </summary>
		[SerializeField]
		private GameObject pausedMenu;

		/// <summary>
		/// The player object that the player controls in the scene
		/// </summary>
		private PlayerScript player = null;

		// The current Game State
		private GameState currentState = GameState.BeforeGameStart;

		// Variables for when we're fighting off a wave
		private int enemiesRemaining;

		// Variables for when we're waiting for a wave to begin
		private float timeTillNextWave;

		[SerializeField]
		Slider healthBar;

		[SerializeField]
		Text enemiesRemainingText;

		public void SetCurrentGameState(GameState state) {
			currentState = state;
		}

		public void SetEnemiesRemaining(int remaining) {
			enemiesRemaining = remaining;
			enemiesRemainingText.text = "Remaining: " + remaining.ToString();
		}

		public void SetTimeTillNextWave(float time) {
			timeTillNextWave = time;
			enemiesRemainingText.text = System.String.Format("Next Wave in : {0:0.00}", time);
			Debug.Log (time + " " + System.String.Format("Next Wave in : {0:0.00}", time));
		}

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