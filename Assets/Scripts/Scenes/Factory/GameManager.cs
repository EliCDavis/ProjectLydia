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
		MapBehavior currentMapBehavior = null;

		[SerializeField]
		GameObject player;

		void Start () {
			RestartLevel ();
		}
	
		void Update() {
			if(Input.GetKeyDown("s")){
				NextWave (currentMapBehavior, player.transform.position);
			}
		}

		public void RestartLevel() {
			currentMap = MapGenerator.CreateMap (25, 1);
			GameObject mapObject = MapGenerator.BuildMap(currentMap);
			currentMapBehavior = mapObject.GetComponent<MapBehavior> ();
		}

		public void RestartLevelWithSameMap() {
			GameObject mapObject = MapGenerator.BuildMap(currentMap);
			currentMapBehavior = mapObject.GetComponent<MapBehavior> ();
		}

		/// <summary>
		/// Blows up the walls to the next room and begins the next wave of enemies.
		/// </summary>
		/// <param name="mapBehavior">Map behavior.</param>
		/// <param name="playerPosition">Player position.</param>
		private void NextWave(MapBehavior mapBehavior, Vector3 playerPosition) {

			if (mapBehavior == null) {
				Debug.LogError ("Trying to start the next wave with no current map");
				return;
			}

			Debug.Log ("Starting Next Wave...");

			// Get Room Player is in
			Room roomPlayerIsIn = mapBehavior.RoomThatContainsPoint(playerPosition);

			if (roomPlayerIsIn != null) {
				Debug.Log ("Player is in room: " + roomPlayerIsIn.ToString ());
			} else {
				Debug.Log ("Player is not in a room");
			}

			// Get Neighboring Rooms of Player's
			Room[] neighboringRooms = currentMapBehavior.GetMapReference().RoomsSurrounding(roomPlayerIsIn);

			// Pick random Neighboring room
			Room selectedRoomToMerge = null;

			if (neighboringRooms != null && neighboringRooms.Length > 0) {
				selectedRoomToMerge = neighboringRooms[Random.Range(0, neighboringRooms.Length - 1)];
			}

			if (selectedRoomToMerge != null) {
				// Merge Two rooms
				currentMapBehavior.MergeRooms (roomPlayerIsIn, selectedRoomToMerge);
			} else {
				// We're at the end of the game! No more rooms to merge!
				Debug.Log("You Won!");
			}

		}


	}

}