using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {
	private TileData tileData;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetTileData(TileData t) {
		tileData = t;
		// Call update functions to perform visual effects
		UpdateOwner(tileData.Owner);
	}

	void OnMouseDown() {
		print("Clicked: " + tileData.id);
		// RealmBase.ClaimTile(tileData.id);
		TileActionPanel.SetTargetTileData(tileData);
		// TileActionPanel.ToggleVisibility();
	}

	// ----
	// Visuals
	// ----

	// Change visuals when owner of tile changes
	public void UpdateOwner(string newOwner) {
		string currentAccount = RealmBase.contract.Caller.LocalAddressHexString.ToLower();
		print("New Owner: " + newOwner);
		print("Client Adr: " + currentAccount);
		if (newOwner == currentAccount) {
			GetComponent<Renderer>().material.color = Color.yellow;
			print("*****THEY ARE THE SAME!!!*****");
		}
	}
}
