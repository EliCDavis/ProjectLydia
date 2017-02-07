using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lydia.MapGeneration;
using Lydia.Enemy;
using Lydia.Player;

namespace Lydia.Scenes.Factory
{

	/// <summary>
	/// Manages The Game State and it's associated variables 
	/// such as number of enemies, what wave we're currently
	/// on, the map's current state, etc.
	/// </summary>
	public class GameManager : MonoBehaviour
	{

		/// <summary>
		/// What wants to follow the player
		/// </summary>
		[SerializeField]
		private UnityStandardAssets.Utility.FollowTarget followTarget;

		[SerializeField]
		private HUD playerHUD;

		/// <summary>
		/// How much time a player will have to get ready for the
		/// next incoming wave after finished off the last one.
		/// </summary>
		private int timeBetweenWaves = 5;

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

		/// <summary>
		/// The currently alive enemies in the scene
		/// </summary>
		private List<GameObject> currentEnemies;

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
		/// Reference to the GameObject the player can control in the scene
		/// </summary>
		private PlayerScript player;

		/// <summary>
		/// Called once the scene has been initialized
		/// </summary>
		void Start ()
		{
			currentEnemies = new List<GameObject> ();
			currentStateOfGame = GameState.BeforeGameStart;
			timeOfGameStateChange = Time.time;
		}

		/// <summary>
		/// Called once a frame
		/// </summary>
		void Update ()
		{
			
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

			case GameState.BeforeGameStart:
				BeforeGameStartStateUpdate ();
				break;

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

			if (newState == currentStateOfGame) {
				Debug.LogWarning ("Attempting to swtich state to a the state we're already on.");
				return;
			}

			timeOfGameStateChange = Time.time;

			switch (newState) {

			case GameState.WaveInProgress:
				numberOfEnemiesSpawnedThisWave = 0;
				NextWave (currentMapBehavior, player.transform.position, ++currentWave);
				break;

			case GameState.AllWavesCleared:
				Destroy (player.gameObject);
				break;

			}

			currentStateOfGame = newState;
			playerHUD.SetCurrentGameState (newState);
		}

		private void BeforeGameStartStateUpdate() {
			Random.InitState (0);

			currentMapBehavior = GenerateMap (MapGenerator.CreateMap(25));

			GameObject playerObj = PlayerFactory.CreatePlayer (currentMapBehavior.GetMapReference().Rooms[0].GetRandomPosition());
			player = playerObj.GetComponent<PlayerScript> ();
			playerHUD.SetPlayer (player);
			followTarget.target = player.transform;
			SwitchState (GameState.WaitingForWave);
		}

		/// <summary>
		/// Update function when the player is waiting for the 
		/// next wave to begin
		/// </summary>
		private void WaitingForWaveStateUpdate ()
		{

			playerHUD.SetTimeTillNextWave (timeOfGameStateChange + timeBetweenWaves - Time.time);

			// Start off the wave after a certain amount of time has passed
			if (timeOfGameStateChange + timeBetweenWaves < Time.time) {
				SwitchState (GameState.WaveInProgress);
			}
		}

		/// <summary>
		/// Update function for whenever the wave is currentely in progress
		/// </summary>
		private void WaveInProgressStateUpdate ()
		{

			if (player.GetHealth() == 0) {
				SwitchState (GameState.PlayerDead);
				Destroy (player.gameObject);
				return;
			}

			// Constantly clean list of destroyed enemies...
			while(currentEnemies.Contains(null)){
				currentEnemies.Remove (null);
			}

			// If all enemies are dead and we've spawned all we wanted too
			if (currentEnemies.Count == 0 && numberOfEnemiesForThisWave == numberOfEnemiesSpawnedThisWave) {

				if (currentMapBehavior.GetMapReference ().Rooms.Count == 1) {
					SwitchState (GameState.AllWavesCleared);
				} else {
					SwitchState (GameState.WaitingForWave);
				}

			}
			else if (currentEnemies.Count < 15 && numberOfEnemiesSpawnedThisWave < numberOfEnemiesForThisWave) {
				SpawnEnemy (currentMapBehavior.RoomThatContainsPoint(player.transform.position), player.gameObject);
				numberOfEnemiesSpawnedThisWave++;
			}

			playerHUD.SetEnemiesRemaining (numberOfEnemiesForThisWave - numberOfEnemiesSpawnedThisWave + currentEnemies.Count);

		}

		public MapBehavior GenerateMap (Map mapToBuild)
		{
			GameObject mapObject = MapGenerator.BuildMap (mapToBuild);
			return mapObject.GetComponent<MapBehavior> ();
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
			if (currentEnemies.Count != 0) {
				Debug.LogError ("Using function in wrong context: Should only be used when there are no enemies in the scene.");
				return 0;
			}

			while(currentEnemies.Count < maxEnemies && Random.Range(0, currentEnemies.Count) < room.Area.Length*3){
				SpawnEnemy (room, player.gameObject);
			}

			return currentEnemies.Count;
		}

		/// <summary>
		/// Spawns a single enemy which will follow a single target.
		/// </summary>
		/// <param name="room">Room.</param>
		/// <param name="target">Target.</param>
		private void SpawnEnemy(Room room, GameObject target) {
			GameObject enemy = EnemyFactory.CreateEnemy (EnemyType.Drone, room.GetRandomPosition ());
			enemy.GetComponent<AI_Controller> ().SetPlayer (target);
			enemy.GetComponent<PotentialFieldsAI> ().SetTarget (target.gameObject);
			currentEnemies.Add(enemy);
		}

		/// <summary>
		/// Blows up the walls to the next room and begins the next wave of enemies.
		/// </summary>
		/// <param name="mapBehavior">Map behavior.</param>
		/// <param name="playerPosition">Player position.</param>
		private void NextWave (MapBehavior mapBehavior, Vector3 playerPosition, int currentWave)
		{

			if(currentWave == 10){
				player.SetGunType (GunType.Power2);
			}

			if(currentWave == 20){
				player.SetGunType (GunType.Power3);
			}

			playerHUD.SetCurrentWave (currentWave);

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
				numberOfEnemiesForThisWave = (int)(10 + (Mathf.Log10(currentWave)*15) + (Mathf.Pow(currentWave,3.1f) / 1000f));

				// Spawn enemies
				numberOfEnemiesSpawnedThisWave = SpawnInitialEnemies(selectedRoomToMerge, currentWave, numberOfEnemiesForThisWave);

				// Merge Two rooms
				currentMapBehavior.MergeRooms (roomPlayerIsIn, selectedRoomToMerge);

			} else {
				// We're at the end of the game! No more rooms to merge!
				Debug.Log ("You Won! Or we where just unable to find a neighboring room.");
				SwitchState (GameState.AllWavesCleared);
			}

		}


	}

}