using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.MapGeneration {

	/// <summary>
	/// Map generator responsible for randomly generating and building 
	/// maps for the player to play the game inside.
	/// </summary>
	public static class MapGenerator {

		// lazy load the floor
		private static GameObject floorTileInstance = null;
		private static GameObject getFloorTileReference(){
			if(floorTileInstance == null){
				floorTileInstance = Resources.Load<GameObject>("MapPieces/Factory/FloorTile");
			}
			return floorTileInstance;
		}

		// Lazy load the wall
		private static GameObject wallInstance = null;
		private static GameObject getWallReference(){
			if(wallInstance == null){
				wallInstance = Resources.Load<GameObject>("MapPieces/Factory/Wall");
			}
			return wallInstance;
		}

		/// <summary>
		/// 4 directions outlined for utilitarian purposes
		/// </summary>
		private static readonly Vector2[] directions = new Vector2[]{
			Vector2.left,
			Vector2.right,
			Vector2.up,
			Vector2.down
		};

		/// <summary>
		/// The size of a single floor tile. (width and height)
		/// </summary>
		private static readonly int TILE_SIZE = 5;

		/// <summary>
		/// Creates a map object representing the layout.
		/// </summary>
		/// <returns>The map.</returns>
		/// <param name="numberOfRooms">Number of rooms the map will have</param>
		public static Map CreateMap(int numberOfRooms){
			Room[] rooms = new Room[numberOfRooms];

			List<Vector2> areaAlreadyOccupied = new List<Vector2> ();

			for(int roomIndex = 0; roomIndex < rooms.Length; roomIndex ++){
				rooms [roomIndex] = CreateRoom(areaAlreadyOccupied);
			}

			return new Map (rooms);
		}

		/// <summary>
		/// Creates the map based on a certain random seed passed in.
		/// </summary>
		/// <returns>The map.</returns>
		/// <param name="numberOfRooms">Number of rooms.</param>
		/// <param name="randomSeed">Random seed.</param>
		public static Map CreateMap(int numberOfRooms, int randomSeed){
			Random.InitState (randomSeed);
			return CreateMap(numberOfRooms);
		}

		/// <summary>
		/// Creates a room that shouldn't have any overlap
		/// with other rooms that already exist
		/// </summary>
		/// <returns>A newly created room.</returns>
		/// <param name="areaOccupied">Area occupied by other rooms.</param>
		private static Room CreateRoom(List<Vector2> areaOccupied) {

			// Room variables
			bool positionSet = false;
			Vector2 position = Vector2.zero;
			List<Vector2> roomsArea = new List<Vector2>();

			// Randomly add a new area with a random amount of chance that decreases with each area added
			while (Random.Range(.1f-(roomsArea.Count*.1f), 2f) >= 0) {
				List<Vector2> areaToPickFrom = AvailableAreaToPickFrom (areaOccupied, roomsArea);
				Vector2 pickedArea = areaToPickFrom [Random.Range (0, areaToPickFrom.Count - 1)];
				if(positionSet == false){
					position = pickedArea;
					positionSet = true;
				}
				roomsArea.Add (pickedArea);
			}

			// Add in newly ocupied area that our room is taking up to the aggregation
			areaOccupied.AddRange(roomsArea);

			// Adjust room area back with reference to room's position
			for (int i = 0; i < roomsArea.Count; i++) {
				roomsArea [i] = new Vector2 (roomsArea[i].x - position.x, roomsArea[i].y - position.y);
			}

			return new Room (position, roomsArea.ToArray());

		}

		/// <summary>
		/// Finds surrounds area tiles free that no
		/// rooms currently occupy inside the map
		/// </summary>
		/// <returns>The area to pick from.</returns>
		/// <param name="areaOccupied">Area occupied.</param>
		/// <param name="thisRoomsArea">Current rooms area.</param>
		private static List<Vector2> AvailableAreaToPickFrom(List<Vector2> areaOccupied, List<Vector2> thisRoomsArea){

			List<Vector2> roomSurroundingArea = new List<Vector2>();

			// Add in all surrounding area the room contains
			foreach (Vector2 roomArea in thisRoomsArea) {
				foreach (Vector2 direction in directions) {
					if(!roomSurroundingArea.Contains(roomArea+direction)){
						roomSurroundingArea.Add (roomArea+direction);
					}
				}
			}

			if (roomSurroundingArea.Count == 0 && thisRoomsArea.Count == 0) {

				Debug.Log ("stargin");

				// Add in any area that a room can start in
				foreach (Vector2 roomArea in areaOccupied) {
					foreach (Vector2 direction in directions) {
						if(!roomSurroundingArea.Contains(roomArea+direction)){
							roomSurroundingArea.Add (roomArea+direction);
						}
					}
				}

			}

			// Remove all area that's already ocupied
			foreach (Vector2 occupiedArea in areaOccupied) {
				if (roomSurroundingArea.Contains(occupiedArea)) {
					roomSurroundingArea.Remove (occupiedArea);
				}
			}

			return roomSurroundingArea.Count == 0 ? new List<Vector2>(new Vector2[]{new Vector2(0, 0)}): roomSurroundingArea;
		}

		/// <summary>
		/// Builds the map and places it inside of the scene.
		/// </summary>
		/// <returns>The map GameObject</returns>
		/// <param name="mapToBuild">Map to build.</param>
		public static GameObject BuildMap(Map mapToBuild){
			
			if (mapToBuild == null) {
				return null;
			}

			GameObject mapObject = new GameObject ("Map");

			foreach (Room room in mapToBuild.Rooms) {
				BuildRoom (room).transform.parent = mapObject.transform;
			}

			return mapObject;

		}

		/// <summary>
		/// Builds a room gameobject with walls and attatches the appropriate room behavior
		/// </summary>
		/// <returns>The room.</returns>
		/// <param name="roomToBuild">Room to build.</param>
		private static GameObject BuildRoom (Room roomToBuild){

			// Make sure we're not going to build anything if nothing was given to us.
			if (roomToBuild == null) {
				return null;
			}

			GameObject room = new GameObject ("Room:"+roomToBuild.Position);
			room.transform.position = new Vector3 (roomToBuild.Position.x*TILE_SIZE, 0, roomToBuild.Position.y*TILE_SIZE);

			// Build each area
			foreach (Vector2 area in roomToBuild.Area) {

				// Build the floor tiles
				GameObject floorTile = GameObject.Instantiate(getFloorTileReference(), room.transform);
				floorTile.name = "Tile:" + area.ToString ();
				floorTile.transform.position = new Vector3 ((roomToBuild.Position.x + area.x) * TILE_SIZE, 0, (roomToBuild.Position.y + area.y) * TILE_SIZE);

				// Build The Walls
				bool[] wallsToBuild = WallsForArea(area, roomToBuild.Area);

				// Left Wall
				if (wallsToBuild[0]) {
					GameObject wall = GameObject.Instantiate (getWallReference (), room.transform);
					wall.transform.position = new Vector3 (((roomToBuild.Position.x + area.x) * TILE_SIZE), 0, ((roomToBuild.Position.y + area.y) * TILE_SIZE) + (TILE_SIZE/2.0f));
					wall.transform.transform.eulerAngles = new Vector3 (0, 270, 0);
				}

				// Right Wall
				if (wallsToBuild[1]) {
					GameObject wall = GameObject.Instantiate (getWallReference (), room.transform);
					wall.transform.position = new Vector3 (((roomToBuild.Position.x + area.x) * TILE_SIZE) + TILE_SIZE, 0, ((roomToBuild.Position.y + area.y) * TILE_SIZE) + (TILE_SIZE/2.0f));
					wall.transform.transform.eulerAngles = new Vector3 (0, 90, 0);
				}

				// Top Wall
				if (wallsToBuild[2]) {
					GameObject wall = GameObject.Instantiate (getWallReference (), room.transform);
					wall.transform.position = new Vector3 (((roomToBuild.Position.x + area.x) * TILE_SIZE) + (TILE_SIZE/2.0f), 0, ((roomToBuild.Position.y + area.y) * TILE_SIZE) + TILE_SIZE);
				}

				// Bottom Wall
				if (wallsToBuild[3]) {
					GameObject wall = GameObject.Instantiate (getWallReference (), room.transform);
					wall.transform.position = new Vector3 (((roomToBuild.Position.x + area.x) * TILE_SIZE) + (TILE_SIZE/2.0f), 0, ((roomToBuild.Position.y + area.y) * TILE_SIZE));
					wall.transform.transform.eulerAngles = new Vector3 (0, 180, 0);
				}

			}

			// Attach Room Behavior for altering room at runtime

			return room;
		}


		/// <summary>
		/// Returns arrray of booleans represeting
		/// which walls to put up around an area.
		/// True means a wall exists
		/// [left, right, up, down]
		/// </summary>
		/// <returns>[left, right, up, down]</returns>
		/// <param name="area">Area.</param>
		/// <param name="entireArea">Entire area.</param>
		private static bool[] WallsForArea (Vector2 area, Vector2[] entireArea){

			bool[] walls = new bool[] { true, true, true, true };

			foreach (Vector2 curArea in entireArea) {

				if (area.Equals(curArea)) {
					continue;
				}

				// Area to left of ours
				if (area.x - 1 == curArea.x && area.y == curArea.y) {
					walls [0] = false;
				}

				// Area to right of ours
				if (area.x + 1 == curArea.x && area.y == curArea.y) {
					walls [1] = false;
				}

				// Area above ours
				if (area.y + 1 == curArea.y && area.x == curArea.x) {
					walls [2] = false;
				}

				// Area below ours
				if (area.y - 1 == curArea.y && area.x == curArea.x) {
					walls [3] = false;
				}

			}

			return walls;

		}
		
	}

}
