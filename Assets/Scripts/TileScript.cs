using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {
	public TileData tileData;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {
		print("Clicked: " + tileData.id);
		RealmBase.ClaimTile(tileData.id);
	}

	public void UpdateOwner(string newOwner) {
		string currentAccount = RealmBase.contract.Caller.LocalAddressHexString.ToLower();
		print("New Owner: " + newOwner);
		print("Client Adr: " + currentAccount);
		if (newOwner == currentAccount) {
			print("*****THEY ARE THE SAME!!!*****");
		}
	}
}
