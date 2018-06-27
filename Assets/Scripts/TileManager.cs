using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loom.Unity3d;
using Loom.Nethereum.ABI.FunctionEncoding.Attributes;

// public struct Tile {
// 	public int x, y;
// 	public Tile(int _x, int _y) {
// 		x = _x;
// 		y = _y;
// 	}
// }

public class TileManager : MonoBehaviour {

	// Prefab for a hex
	public GameObject hexPrefab;

	// Store tiles in a map id->tile
	private Dictionary<uint, GameObject> tileObjs = new Dictionary<uint, GameObject>();

	// Have we already loaded the initial tile set?
	bool alreadyLoadedInitialTiles = false;
	// Tile count
	int tileCount;
	// Events queue
	private Queue<EvmChainEventArgs> _events = new Queue<EvmChainEventArgs>();

	// Use this for initialization
	async void Start () {
		
	}

	private void ContractEventReceived(object sender, EvmChainEventArgs e)
	{
			Debug.LogFormat("Received smart contract event: " + e.EventName);
			_events.Enqueue(e);
	}
	
	// Update is called once per frame
	async void Update () {

		if (RealmBase.contract != null) {

			// Load initial map
			if (!alreadyLoadedInitialTiles && RealmBase.contract != null) {
				RealmBase.contract.EventReceived += ContractEventReceived;
				alreadyLoadedInitialTiles = true;
				tileCount = await RealmBase.GetTileCount();
				for (uint i = 0; i < tileCount; i++) {
					// Get tile data from contract data
					print("Count: " + tileCount);
					
					// print(i + ": " + t.X + ", " + t.Y);
					// Add tile object to map
					SpawnNewTileObject(i);
					
					// tiles[i] = new Tile();
				}
			}

			// Handle all queued events
			HandleEvents();

			
		}
		
	}

	// Handles all events that are in the _events queue
	void HandleEvents() {
		// Go through the queue and handle each event 	
		while(_events.Count > 0) {
			EvmChainEventArgs e = _events.Dequeue();
			if (e.EventName == "OnNewTile") {
				OnNewTileEvent newTileEvent = e.DecodeEventDTO<OnNewTileEvent>();
				uint id = (uint)newTileEvent.TileId.IntValue;
				SpawnNewTileObject(id);
				Debug.LogFormat("NewTile event id: " + (uint)newTileEvent.TileId.IntValue);
			} else if (e.EventName == "OnUpdateTileOwner") {
				OnUpdateTileOwnerEvent updateTileOwnerEvent = e.DecodeEventDTO<OnUpdateTileOwnerEvent>();
				uint id = (uint)updateTileOwnerEvent.TileId.IntValue;
				string newOwner = updateTileOwnerEvent.NewOwner;
				tileObjs[id].GetComponent<TileScript>().UpdateOwner(newOwner);
			}
		}
	}

	async void SpawnNewTileObject(uint id) {
		print("New Tile: " + id);
		// Attempt to get the Tile data from the network
		TileData t = await RealmBase.GetTile(id);
		if (t == null) {
			print("Error: The tile you are trying to spawn doesn't exist.");
		}
		// Set id
		t.id = id;
		// Calculate where the tile should be displayed
		var x = 1 * (Mathf.Sqrt(3) * t.Y  +  Mathf.Sqrt(3)/2f * t.X);
		var z = 1 * (3f/2f * t.X);
		var newTileObject = Instantiate(hexPrefab, new Vector3(x, 0, z), Quaternion.identity);
		newTileObject.transform.Rotate(0, 90, 0);
		// Instantiate new tile object
		newTileObject.GetComponent<TileScript>().tileData = t;
		tileObjs.Add(id, newTileObject);
	}
}
