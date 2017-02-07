using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.Enemy {

	public static class EnemyFactory {
		
		/// <summary>
		/// Creates the enemy given the type and position. Most basic form of 
		/// enemy instantiation
		/// </summary>
		/// <returns>The enemy.</returns>
		/// <param name="enemyType">Enemy type.</param>
		/// <param name="position">Position.</param>
		public static GameObject CreateEnemy(EnemyType enemyType, Vector3 position) {

			GameObject enemyInstance = GameObject.Instantiate (GetEnemyReference (enemyType), position, Quaternion.identity);
			enemyInstance.layer = 9; // Layer 9 is enemies

			return enemyInstance;

		}

		private static GameObject GetEnemyReference(EnemyType enemyType) {
			switch (enemyType) {

			case EnemyType.Drone:
				return GetDroneReference ();
			
			}

			return null;
		}

		// Lazy load drone refernece
		private static GameObject droneReference = null;
		private static GameObject GetDroneReference() {
			if (droneReference == null) {
				droneReference = Resources.Load ("Enemies/Drone") as GameObject;
			}
			return droneReference;
		}

	}

}