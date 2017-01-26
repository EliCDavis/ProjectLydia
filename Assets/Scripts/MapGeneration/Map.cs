using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.MapGeneration {

	/// <summary>
	/// A Map's blueprints which we can build from.
	/// Contains rooms and their locations
	/// </summary>
	public class Map {

		/// <summary>
		/// All rooms the map contains
		/// </summary>
		private List<Room> rooms;

		public Map(){
			this.rooms = new List<Room> ();
		}

		public Map(Room[] rooms){
			this.rooms = new List<Room> ();

			Room addResult = null;

			// Add rooms one by one and check if any overlap with eachother
			for (var i = 0; i < rooms.Length; i ++) {
				addResult = this.AddRoom (rooms [i]);
				if (addResult != null) {
					Debug.LogWarning ("Unable to add room due to overlapping issues. Room[" + i + "]: " + rooms[i].ToString() + ". Overlaps with: " + addResult.ToString());
				}
			}
		}

		/// <summary>
		/// All rooms the map contains
		/// </summary>
		public List<Room> Rooms {
			get {
				return rooms;
			}
		}

		/// <summary>
		/// Adds the room to the map if it doesn't overlap with other rooms
		/// </summary>
		/// <returns><c>null</c>, if room was added, <c>room instace</c> of the room it's overlapping with.</returns>
		/// <param name="roomToAdd">Room to add.</param>
		public Room AddRoom(Room roomToAdd) {

			/**
			 * Make sure the room we're trying to add isn't 
			 * trying to take up the space of another room
			 */
			foreach (Room room in rooms) {
				if (room.overlaps(roomToAdd)) {
					return room;
				}
			}

			rooms.Add (roomToAdd);
			return null;
		}

		/// <summary>
		/// Merges the rooms together removes the two previous rooms
		/// and then adds the merged room to the map.
		/// </summary>
		/// <returns>The rooms.</returns>
		/// <param name="room1">Room1.</param>
		/// <param name="room2">Room2.</param>
		public Room MergeRooms(Room room1, Room room2) {

			if (room1 == null || room2 == null) {
				Debug.LogWarning ("Can not merge null rooms");
				return null;
			}

			if (!this.Rooms.Contains(room1)) {
				Debug.LogError ("This map does not contain the first room passed in");
				return null;
			}

			if (!this.Rooms.Contains(room2)) {
				Debug.LogError ("The map doesn't contain the second room passed in");
				return null;
			}

			List<Vector2> combinedArea = new List<Vector2> ();
			foreach (Vector2 area in room1.Area) {
				combinedArea.Add (room1.Position + area);
			}
			foreach (Vector2 area in room2.Area) {
				combinedArea.Add (room2.Position + area - room1.Position);
			}

			Room mergedRoom = new Room (room1.Position, combinedArea.ToArray());

			// Remove rooms before adding or the map won't let you add (because the room would overlap with other)
			this.Rooms.Remove(room1);
			this.Rooms.Remove(room2);

			// Add in our merged room
			this.AddRoom (mergedRoom);

			return mergedRoom;
		}

		public Room[] RoomsSurrounding(Room room) {

			List<Vector2> areaSurroundingRoom = new List<Vector2> ();

			/*
			 * Get all area that surounds the room.
			 */
			foreach (Vector2 area in room.Area) {
				foreach(Vector2 direction in MapGenerator.directions){
					if (!areaSurroundingRoom.Contains(area + direction)) {
						areaSurroundingRoom.Add (area + direction);
					}
				}
			}


			List<Room> roomsSurounding = new List<Room> ();

			/*
			 * Find area that rooms contain
			 * 
			 * OPTIMIZATION: Only loop through rooms that haven't been added yet
			 */
			foreach (Vector2 surroundingArea in areaSurroundingRoom) {
				foreach (Room roomInMap in this.Rooms) {

					if (roomsSurounding.IndexOf (roomInMap) == -1 && roomInMap.ContainsArea (surroundingArea) && !roomInMap.Equals(room)) {
						roomsSurounding.Add (roomInMap);
					}
				}
			}

			return roomsSurounding.ToArray();
		}

	}

}