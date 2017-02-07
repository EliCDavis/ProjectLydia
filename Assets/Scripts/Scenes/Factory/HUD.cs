using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Lydia.Scenes.Factory
{
	
	public class HUD : MonoBehaviour {

		/// <summary>
		/// The paused menu.
		/// </summary>
		[SerializeField]
		private GameObject pausedMenu;

		/// <summary>
		/// Menu presented to the player when they die.
		/// </summary>
		[SerializeField]
		private GameObject deadMenu;

		/// <summary>
		/// The menu that is presented when the player clears all stages
		/// </summary>
		[SerializeField]
		private GameObject winMenu;

		/// <summary>
		/// The player object that the player controls in the scene
		/// </summary>
		private PlayerScript player = null;

		// The current Game State
		private GameState currentState = GameState.BeforeGameStart;

		[SerializeField]
		Slider healthBar;

		[SerializeField]
		Text enemiesRemainingText;

		public void SetCurrentGameState(GameState state) {
			currentState = state;

			if (state == GameState.PlayerDead) {
				deadMenu.SetActive (true);
			}

			if (state == GameState.AllWavesCleared) {
				winMenu.SetActive (true);
			}

		}

		public void RestartScene () {
			SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		}

		public void GoToMainMenu() {
			SceneManager.LoadScene ("MainMenu");
		}

		public void SetEnemiesRemaining(int remaining) {
			enemiesRemainingText.text = "Remaining: " + remaining.ToString();
		}

		public void SetTimeTillNextWave(float time) {
			enemiesRemainingText.text = System.String.Format("{0:0.} till wave", time);
		}

		public void SetPlayer(PlayerScript player) {
			this.player = player;
			healthBar.minValue = 0f;
			healthBar.maxValue = (float)player.GetMaxHealth ();
		}

		void Update() {

			if (player == null) {
				return;
			}

			healthBar.value = player.GetHealth ();

		}

	}

}