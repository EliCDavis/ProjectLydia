using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.MapGeneration {

	public class RoomBehavior : MonoBehaviour {

		private Room roomReference;

		public void SetRoomReference(Room reference) {

			if(reference == null){
				Debug.LogError ("Room reference is null! Returning without setting reference!");
			}

			this.roomReference = reference;

		}

		public Room GetRoomReference () {
			return roomReference;
		}

		/// <summary>
		/// Whether or not the room contains the point in world space
		/// </summary>
		/// <returns><c>true</c>, if point was containsed, <c>false</c> otherwise.</returns>
		/// <param name="point">Point in world space</param>
		public bool ContainsPoint (Vector3 point) {

			if (roomReference == null) {
				return false;
			}

			foreach (Vector2 area in roomReference.Area) {
				Rect dimensions = new Rect ((area.x + roomReference.Position.x) * MapGenerator.TILE_SIZE,
											(area.y + roomReference.Position.y) * MapGenerator.TILE_SIZE,
											MapGenerator.TILE_SIZE,
											MapGenerator.TILE_SIZE);

				if (dimensions.x <= point.x && dimensions.x + dimensions.width >= point.x) {
					if (dimensions.y <= point.z && dimensions.y + dimensions.height >= point.z) {
						return true;
					}
				}
			}

			return false;
		}

	}

}