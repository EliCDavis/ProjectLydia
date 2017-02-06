using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lydia.MapGeneration;

namespace Lydia.Scenes.Factory
{

	/// <summary>
	/// The different states the game can be in.
	/// </summary>
	enum GameState
	{
		WaveInProgress,
		WaitingForWave
	}

	/// <summary>
	/// Manages The Game State and it's associated variables 
	/// such as number of enemies, what wave we're currently
	/// on, the map's current state, etc.
	/// </summary>
	public class GameManager : MonoBehaviour
	{

		/// <summary>
		/// How much time a player will have to get ready for the
		/// next incoming wave after finished off the last one.
		/// </summary>
		private int timeBetweenWaves = 10;

		/// <summary>
		/// Whether or not the game is currently paused
		/// </summary>
		private bool currentelyPaused = false;

		/// <summary>
		/// The current state of the game.
		/// </summary>
		private GameState currentStateOfGame;

		/// <summary>
		/// The game time of the last time the game state changed
		/// </summary>
		private float timeOfGameStateChange;

		/// <summary>
		/// Blueprints of the current map we're on
		/// </summary>
		private Map currentMap = null;

		/// <summary>
		/// Current in game map for the player to interact with
		/// </summary>
		private MapBehavior currentMapBehavior = null;

		/// <summary>
		/// The current wave the player is on.
		/// </summary>
		private int currentWave = 0;

		[SerializeField]
		private GameObject player;



		/// <summary>
		/// Called once the scene has been initialized
		/// </summary>
		void Start ()
		{
			RestartLevel ();
			currentStateOfGame = GameState.WaitingForWave;
			timeOfGameStateChange = Time.time;
		}

		/// <summary>
		/// Called once a frame
		/// </summary>
		void Update ()
		{
			
			if (Input.GetKeyDown ("s")) {
				NextWave (currentMapBehavior, player.transform.position, ++currentWave);
			}

			if (!currentelyPaused) {
				GameStateUpdate ();
			}

		}

		/// <summary>
		/// Updates appropriatly according to the current state we're in.
		/// </summary>
		private void GameStateUpdate ()
		{
			switch (currentStateOfGame) {

			case GameState.WaitingForWave:
				WaitingForWaveStateUpdate ();
				break;

			case GameState.WaveInProgress:
				WaveInProgressStateUpdate ();
				break;
			
			}
		}

		/// <summary>
		/// Switchs the current game state.
		/// 
		/// Acts as the 'start' function for whatever the new state is.
		/// </summary>
		/// <param name="newState">New state.</param>
		private void SwitchState (GameState newState)
		{
			timeOfGameStateChange = Time.time;

			switch (newState) {

			case GameState.WaveInProgress:
				numberOfEnemiesSpawnedThisWave = 0;
				NextWave (currentMapBehavior, player.transform.position, ++currentWave);
				break;

			}

			currentStateOfGame = newState;

		}

		/// <summary>
		/// Update function when the player is waiting for the 
		/// next wave to begin
		/// </summary>
		private void WaitingForWaveStateUpdate ()
		{
			// Start off the wave after a certain amount of time has passed
			if (timeOfGameStateChange + timeBetweenWaves < Time.time) {
				SwitchState (GameState.WaveInProgress);
			}
		}

		/// <summary>
		/// The currently alive enemies in the scene
		/// </summary>
		private List<GameObject> currentEnemies = new List<GameObject>();

		/// <summary>
		/// How many enemies that have been spawned since
		/// the begining of the current wave we are on.
		/// 
		/// This includes dead enemies
		/// </summary>
		private int numberOfEnemiesSpawnedThisWave = 0;

		/// <summary>
		/// How many enemies we're supposed to spawn this wave.
		/// </summary>
		private int numberOfEnemiesForThisWave = 0;

		/// <summary>
		/// Update function for whenever the wave is currentely in progress
		/// </summary>
		private void WaveInProgressStateUpdate ()
		{

			// If all enemies are dead and we've spawned all we wanted too
			if (currentEnemies.Count == 0 && numberOfEnemiesForThisWave == numberOfEnemiesSpawnedThisWave) {
				SwitchState (GameState.WaitingForWave);
			}

		}

		public void RestartLevel ()
		{
			currentMap = MapGenerator.CreateMap (25, 1);
			GameObject mapObject = MapGenerator.BuildMap (currentMap);
			currentMapBehavior = mapObject.GetComponent<MapBehavior> ();
		}

		public void RestartLevelWithSameMap ()
		{
			GameObject mapObject = MapGenerator.BuildMap (currentMap);
			currentMapBehavior = mapObject.GetComponent<MapBehavior> ();
		}

		/// <summary>
		/// Does initial spawning for enemies of the current wave.
		/// 
		/// Should only be called once per wave.  Any additional eneimes
		/// that are added to the wave after initialization should be done
		/// elsewhere
		/// </summary>
		/// <returns>How many enemies where spawned in this initialization.</returns>
		/// <param name="room">Room.</param>
		/// <param name="wave">Wave.</param>
		/// <param name="maxEnemies">How many enemies your allowed to spawn</param>
		private int SpawnInitialEnemies (Room room, int wave, int maxEnemies)
		{
			int enemiesSpawned = 0;

			while(enemiesSpawned < maxEnemies && Random.Range(0, enemiesSpawned) < room.Area.Length*4){
				enemiesSpawned++;

				GameObject enemy = GameObject.CreatePrimitive (PrimitiveType.Sphere);

				enemy.transform.position = room.GetRandomPosition ();

			}

			return enemiesSpawned;
		}

		/// <summary>
		/// Blows up the walls to the next room and begins the next wave of enemies.
		/// </summary>
		/// <param name="mapBehavior">Map behavior.</param>
		/// <param name="playerPosition">Player position.</param>
		private void NextWave (MapBehavior mapBehavior, Vector3 playerPosition, int currentWave)
		{

			if (mapBehavior == null) {
				Debug.LogError ("Trying to start the next wave with no current map");
				return;
			}

			// Get Room Player is in
			Room roomPlayerIsIn = mapBehavior.RoomThatContainsPoint (playerPosition);

			// Get Neighboring Rooms of Player's
			Room[] neighboringRooms = currentMapBehavior.GetMapReference ().RoomsSurrounding (roomPlayerIsIn);

			// Pick random Neighboring room
			Room selectedRoomToMerge = null;

			// If we where able to find a neighboring room..
			if (neighboringRooms != null && neighboringRooms.Length > 0) {
				selectedRoomToMerge = neighboringRooms [Random.Range (0, neighboringRooms.Length - 1)];
			}

			if (selectedRoomToMerge != null) {

				// Calculate enemies of this wave
				// Why this formula? I fucked around on a graphing calculator till I got something I liked
				numberOfEnemiesForThisWave = (int)(10 + (Mathf.Log10(currentWave)*10) + (Mathf.Pow(currentWave,3.1f) / 1000f));

				// Spawn enemies
				numberOfEnemiesSpawnedThisWave = SpawnInitialEnemies(selectedRoomToMerge, currentWave, numberOfEnemiesForThisWave);

				// Merge Two rooms
				currentMapBehavior.MergeRooms (roomPlayerIsIn, selectedRoomToMerge);

			} else {
				// We're at the end of the game! No more rooms to merge!
				Debug.Log ("You Won! Or we where just unable to find a neighboring room.");
			}

		}


	}

}