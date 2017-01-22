using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lydia.MapGeneration {

	/// <summary>
	/// Map generator responsible for randomly generating and building 
	/// maps for the player to play the game inside.
	/// </summary>
	public static class MapGenerator {


		private static GameObject floorTileInstance = null;
		private static GameObject getFloorTileReference(){
			if(floorTileInstance == null){
				floorTileInstance = Resources.Load<GameObject>("MapPieces/Factory/FloorTile");
			}
			return floorTileInstance;
		}

		private static GameObject wallInstance = null;
		private static GameObject getWallReference(){
			if(wallInstance == null){
				wallInstance = Resources.Load<GameObject>("MapPieces/Factory/Wall");
			}
			return wallInstance;
		}

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

			List<Vector2> areaOccupied = new List<Vector2> ();

			for(int roomIndex = 0; roomIndex < rooms.Length; roomIndex ++){
				Vector2[] area = new Vector2[]{
					new Vector2(0,0),
					new Vector2(0,1)
				};
				rooms [roomIndex] = new Room (new Vector2(roomIndex, 0), area);
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

		private static Room CreateRoom(List<Vector2> areaOccupied) {

			Vector2 position;

			if (areaOccupied.Count == 0) {
				position = Vector2.zero;
			}

			return new Room (position, null);
		}

		private Vector2[] availableAreaToPickFrom(List<Vector2> areaOccupied, List<Vector2> thisRoomsArea){

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

			GameObject room = new GameObject ("Room");
			room.transform.position = new Vector3 (roomToBuild.Position.x, 0, roomToBuild.Position.y);

			// Build each area
			foreach (Vector2 area in roomToBuild.Area) {

				// Build the floor tiles
				GameObject floorTile = GameObject.Instantiate(getFloorTileReference(), room.transform);
				floorTile.transform.position = new Vector3 ((roomToBuild.Position.x + area.x) * TILE_SIZE, 0, (roomToBuild.Position.y + area.y) * TILE_SIZE);

				// Build The Walls
				bool[] wallsToBuild = WallsForArea(area, roomToBuild.Area);

				// Left Wall
				if (wallsToBuild[0]) {
					GameObject wall = GameObject.Instantiate (getWallReference (), room.transform);
					wall.transform.position = new Vector3 (((roomToBuild.Position.x + area.x) * TILE_SIZE), 0, ((roomToBuild.Position.y + area.y) * TILE_SIZE) + (TILE_SIZE/2.0f));
					wall.transform.transform.eulerAngles = new Vector3 (0, 90, 0);
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
				if (area.x - 1 == curArea.x) {
					walls [0] = false;
				}

				// Area to right of ours
				if (area.x + 1 == curArea.x) {
					walls [1] = false;
				}

				// 
				if (area.y + 1 == curArea.y) {
					walls [2] = false;
				}

				if (area.y - 1 == curArea.y) {
					walls [3] = false;
				}

			}

			return walls;

		}
		
	}

}
