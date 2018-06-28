using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Org.BouncyCastle.Math;


public class TileScript : MonoBehaviour {
	private TileData tileData;
	private GameObject improvement;

	// improvements
	public GameObject city;

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
		UpdateImprovement(tileData.Improvement);
	}

	void OnMouseDown() {
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}
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
		if (newOwner != "0x0000000000000000000000000000000000000000") {
			if (newOwner == currentAccount) {
				GetComponent<Renderer>().material.color = Color.green;
			} else {
				GetComponent<Renderer>().material.color = Color.blue;
			}
		}
	}

	public void UpdateImprovement(BigInteger improvementId) {
		if (improvement != null) {
			GameObject.Destroy(improvement);
		}
		if (improvementId.IntValue == 1) {
			GameObject newImprovement = Instantiate(city, new Vector3(0, 0, 0), Quaternion.identity);
			newImprovement.gameObject.transform.parent = gameObject.transform;
			newImprovement.transform.localPosition = new Vector3(0, 0, 0);
			newImprovement.transform.localRotation = gameObject.transform.rotation;
			improvement = newImprovement;
		}
	}
}
