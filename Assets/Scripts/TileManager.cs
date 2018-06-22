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

	// Prefab for a hex
	public GameObject hexPrefab;

	// Store tiles in a map id->tile
	private Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();

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
			tileCount = await RealmBase.GetTileCount();
			for (int i = 0; i < tileCount; i++) {
				Tile t = await RealmBase.GetTile(i);
				print(i + ": " + t.X + ", " + t.Y);
				var x = 18 * (Mathf.Sqrt(3) * t.X  +  Mathf.Sqrt(3)/2f * t.Y);
        var y = 20 * (3f/2f * t.Y);
				t.gameObject = Instantiate(hexPrefab, new Vector3(x, 0, y), Quaternion.identity);

				tiles.Add(i, t);
				// tiles[i] = new Tile();
			}
		}
	}
}
