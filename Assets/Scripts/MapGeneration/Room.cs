using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.MapGeneration {

	/// <summary>
	/// A Single room which has a position and area it takes up.
	/// </summary>
	public class Room {

		/// <summary>
		/// Position of the room relative to map coordinates
		/// </summary>
		private Vector2 position;

		/// <summary>
		/// Area the the room takes up.
		/// 
		/// Example: [(0,0), (1,0), (0,1)] would make a room 
		/// in the shape of an L
		/// </summary>
		private Vector2[] area;

		public Room(Vector2 position, Vector2[] area){

			this.position = position;

			if (area == null) {
				this.area = new Vector2[0];
			} else {
				this.area = area;
			}

		}

		/// <summary>
		/// Position of the room relative to map coordinates
		/// </summary>
		public Vector2 Position {
			get {
				return position;
			}
		}

		/// <summary>
		/// Area the the room takes up.
		/// 
		/// Example: [(0,0), (1,0), (0,1)] would make a room 
		/// in the shape of an L
		/// </summary>
		public Vector2[] Area {
			get {
				return area;
			}
		}

		/// <summary>
		/// Whether or not this room is taking up space
		/// that the other room occupies and vice versa
		/// </summary>
		/// <param name="otherRoom">Other room.</param>
		public bool overlaps(Room otherRoom){

			if (otherRoom == null) {
				return false;
			}

			foreach (Vector2 ourArea in this.Area) {
				foreach (Vector2 otherArea in otherRoom.Area) {
					if (ourArea.x + this.Position.x == otherArea.x + otherRoom.Position.x && ourArea.y + this.Position.y == otherArea.y + otherRoom.Position.y) {
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// This Room explicitly contains the area passed in.
		/// This performs no implicit casting such as (int)vector.x.
		/// </summary>
		/// <returns><c>true</c>, if area was containsed, <c>false</c> otherwise.</returns>
		/// <param name="area">Area.</param>
		public bool ContainsArea(Vector2 area) {

			foreach (Vector2 ourArea in this.Area) {
				if (this.Position + ourArea == area) {
					return true;
				}
			}

			return false;
		}

		public override string ToString () {
			string str = "Origin: " + this.position.ToString () + ": [";
			for (var i = 0; i < this.Area.Length; i++) {
				str += this.Area [i].ToString ();
				if (i != this.Area.Length -1) {
					str += ", ";
				}
			}
			return str + "]"; 
		}

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ()) {
				return false;
			}

			Room r = (Room)obj;

			if(r.Position != this.Position){
				return false;
			}

			if (this.Area.Length != r.Area.Length) {
				return false;
			}

			bool containsArea = false;

			foreach (Vector2 otherArea in r.Area) {

				containsArea = false;

				foreach (Vector2 ourArea in this.Area) {

					if (otherArea == ourArea) {
						containsArea = true;
						break;
					}

				}

				if (containsArea == false) {
					return false;
				}
			}


			return true;

		}

		public override int GetHashCode() {
			// only way these are equal is if both rooms are in same position
			return (int)position.x ^ (int)position.y;
		}

	}

}