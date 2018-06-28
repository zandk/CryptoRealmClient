using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Canvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnChooseAccount(string id) {
		RealmBase.accountId = id;
		SceneManager.LoadScene("game", LoadSceneMode.Single);
	}
}
