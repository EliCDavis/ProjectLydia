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
			this.rooms = new List<Room> (rooms);
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
		/// <returns><c>true</c>, if room was added, <c>false</c> otherwise.</returns>
		/// <param name="roomToAdd">Room to add.</param>
		public bool AddRoom(Room roomToAdd) {

			/**
			 * Make sure the room we're trying to add isn't 
			 * trying to take up the space of another room
			 */
			foreach (Room room in rooms) {
				if (room.overlaps(roomToAdd)) {
					return false;
				}
			}

			rooms.Add (roomToAdd);
			return true;
		}

	}

}