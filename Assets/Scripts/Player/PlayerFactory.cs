using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.Player {

	/// <summary>
	/// Handles all creation logic for creating a character 
	/// that the player can control
	/// </summary>
	public static class PlayerFactory {

		/// <summary>
		/// Creates a player object that the player will be able to control.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="position">Position the player object will be created in.</param>
		public static GameObject CreatePlayer(Vector3 position) {
			GameObject playerInstance = GameObject.Instantiate (GetPlayerReference(), position, Quaternion.identity);

			return playerInstance;
		}

		// Lazy load player refernece
		private static GameObject playerReference = null;
		private static GameObject GetPlayerReference() {
			if (playerReference == null) {
				playerReference = Resources.Load ("Player") as GameObject;
			}
			return playerReference;
		}

	}

}