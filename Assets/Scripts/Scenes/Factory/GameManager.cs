using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lydia.MapGeneration;

namespace Lydia.Scenes.Factory {

	/// <summary>
	/// Manages The Game State and it's associated variables 
	/// such as number of enemies, what wave we're currently
	/// on, the map's current state, etc.
	/// </summary>
	public class GameManager : MonoBehaviour {

		Map currentMap = null;

		void Start () {
			RestartLevel ();
		}
	
		void RestartLevel() {
			currentMap = MapGenerator.CreateMap (5, 1);
			MapGenerator.BuildMap(currentMap);
		}

		void RestartLevelWithSameMap() {
			MapGenerator.BuildMap(currentMap);
		}

	}

}