using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileActionPanel : MonoBehaviour {
	// Instance
	private static TileActionPanel instance;
	// The data for the selected tile
	private static TileData targetTileData;
	// UI elements
	public Text titleText;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Updates the title text, but appends a prefix and suffix
	void SetTitleText(string text) {
		titleText.text = "Tile " + text;
	}

	// Call when the claim button is pressed
	public void OnClaimButtonPress() {
		if (targetTileData == null)
			return;
		// Send the claim transaction
		RealmBase.ClaimTile(targetTileData.id);
	}

	// Call when the spread button is pressed
	public void OnSpreadButtonPress() {
		if (targetTileData == null)
			return;
		// Send the claim transaction
		RealmBase.SpreadTile(targetTileData.id);
	}

	public static void SetTargetTileData(TileData t) {
		// Update target tile data
		targetTileData = t;
		// Update UI
		instance.SetTitleText(t.id.ToString());
	}

	public static void ToggleVisibility() {
		// Make sure an instance has been created
		if (instance == null) {
			return;
		}
		// Toggle the panel's visibility
		instance.gameObject.SetActive(!instance.gameObject.activeSelf);
	}
}
