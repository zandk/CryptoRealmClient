using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public struct Tile {
// 	public int x, y;
// 	public Tile(int _x, int _y) {
// 		x = _x;
// 		y = _y;
// 	}
// }

public class TileManager : MonoBehaviour {

	// Store tiles in a map id->tile
	// private Dictionary<int, Tile> tiles;

	// Have we already loaded the initial tile set?
	bool alreadyLoadedInitialTiles = false;
	// Tile count
	int tileCount;

	// Use this for initialization
	async void Start () {
	
	}
	
	// Update is called once per frame
	async void Update () {
		if (!alreadyLoadedInitialTiles && RealmBase.contract != null) {
			alreadyLoadedInitialTiles = true;
			RealmBase.GetTile(0);
			tileCount = await RealmBase.GetTileCount();
			for (int i = 0; i < tileCount; i++) {
				// tiles[i] = new Tile();
			}
		}
	}
}
