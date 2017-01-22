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

	}

}