using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.MapGeneration {

	public class MapBehavior : MonoBehaviour {

		Map mapReference = null;

		// lazy load rooms
		private List<RoomBehavior> roomsInstance = null;
		private List<RoomBehavior> getRooms() {
			if (roomsInstance == null) {
				roomsInstance = new List<RoomBehavior> ();
			}

			return roomsInstance;
		}

		// Lazy load explosion effect
		private GameObject explosionReference = null;
		private GameObject GetExplosionReference() {
			if (explosionReference == null) {
				explosionReference = Resources.Load<GameObject> ("BigExplosionEffect");
			}
			return explosionReference;
		}

		public void AddRoom(RoomBehavior room) {
			getRooms().Add (room);
		}

		public void SetMapReference (Map map) {

			if (map == null) {
				Debug.LogWarning ("MapBehavior.SetMapReference(): map reference is null!");
				return;
			}

			mapReference = map;

		}

		public Map GetMapReference () {
			return mapReference;
		}

		/// <summary>
		/// Merges the two rooms together if their adjacent to eachother,
		/// deleting any walls that seperate them.
		/// </summary>
		/// <returns>The merged room.</returns>
		/// <param name="room1">Room1.</param>
		/// <param name="room2">Room2.</param>
		public RoomBehavior MergeRooms(Room room1, Room room2) {

			if (room1 == null || room2 == null) {
				Debug.LogError ("Unable to merge rooms due to one or more of the rooms being null!");
				return null;
			}

			if (room1.Equals(room2)) {
				Debug.LogWarning ("Unable to merge rooms due to the two rooms being passed in are already the same.");
				return null;
			}

			RoomBehavior room1Behavior = null;
			RoomBehavior room2Behavior = null;

			foreach (RoomBehavior roomBehavior in this.getRooms()) {
				if (roomBehavior.GetRoomReference ().Equals (room1)) {
					room1Behavior = roomBehavior;
				} else if (roomBehavior.GetRoomReference ().Equals (room2)) {
					room2Behavior = roomBehavior;
				}
			}

			if (room1Behavior == null || room2Behavior == null) {
				Debug.LogError ("Unable to merge rooms due to inability to find Room Behaviors");
				return null;
			}

			Room mergedRoom = this.mapReference.MergeRooms (room1Behavior.GetRoomReference (), room2Behavior.GetRoomReference ());

			// Create the new room object
			GameObject mergedRoomObject = new GameObject("Room"+mergedRoom.Position);
			RoomBehavior roomBehaviorInstance = mergedRoomObject.AddComponent<RoomBehavior>();
			mergedRoomObject.transform.parent = transform;
			roomBehaviorInstance.SetRoomReference (mergedRoom);
			getRooms ().Add (roomBehaviorInstance);

			// Find walls and destory them
			foreach (Transform childFromRoom1 in room1Behavior.transform) {
				foreach (Transform childFromRoom2 in room2Behavior.transform) {
					if (childFromRoom1.position == childFromRoom2.position) {

						GameObject explosion = Instantiate(GetExplosionReference (),childFromRoom1.position, Quaternion.identity);
						Destroy (explosion, 3.95f);

						// Destroy the walls
						Destroy (childFromRoom1.gameObject);
						Destroy (childFromRoom2.gameObject);

					}
				}
			}

			this.getRooms ().Remove (room1Behavior);
			this.getRooms ().Remove (room2Behavior);

			for (int i = room1Behavior.transform.childCount-1; i >= 0; i--) {
				room1Behavior.transform.GetChild (i).parent = mergedRoomObject.transform;
			}
			for (int i = room2Behavior.transform.childCount-1; i >= 0; i--) {
				room2Behavior.transform.GetChild (i).parent = mergedRoomObject.transform;
			}

			Destroy (room1Behavior.gameObject);
			Destroy (room2Behavior.gameObject);

			return roomBehaviorInstance;

		}

		/// <summary>
		/// Finds the room where that point resides inside.
		/// The y direction is completely ignored and only
		/// the x and z directions are taken into account.
		/// </summary>
		/// <returns>The Room that contains the point. Null if we can't find a room</returns>
		/// <param name="point">Point in world space.</param>
		public Room RoomThatContainsPoint(Vector3 point){

			foreach (RoomBehavior room in this.getRooms()) {
				if (room.ContainsPoint (point)) {
					return room.GetRoomReference ();
				}
			}

			return null;
		}



	}

}