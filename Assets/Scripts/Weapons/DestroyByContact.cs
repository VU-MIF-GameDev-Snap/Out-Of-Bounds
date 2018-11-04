using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;

	private GameController _gameController;

	void Start() {

		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");

		if (gameControllerObject != null) {

			_gameController = gameControllerObject.GetComponent<GameController> ();
		} 

		if (_gameController == null) {

			Debug.Log ("Cannot find 'Game Controller' script");
		}
	}

	//void OnTriggerEnter(Collider other) {

	//	if (other.CompareTag("Boundary") || other.CompareTag("Enemy") || other.CompareTag("Powerup")) {

	//		return;
	//	}

	//	if (explosion != null) {
			
	//		Instantiate (explosion, transform.position, transform.rotation);
	//	}

	//	if (other.CompareTag("Player")) {

	//		Indexer indexer = other.GetComponent<Indexer> ();

	//		if (indexer != null) {
				
	//			_gameController.AddScore (scoreValue, indexer.playerIndex);
	//			_gameController.AddLives (-1, indexer.playerIndex);

	//			if (_gameController.GetLives (indexer.playerIndex) == 0) {

	//				Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
	//				other.gameObject.SetActive (false); 
	//				_gameController.GameOver (); 
	//			}
	//		}
	//	}

	//	else {

	//		if (other.CompareTag ("Bolt")) {

	//			Indexer indexer = other.GetComponent<Indexer> ();

	//			if (indexer != null)
	//				_gameController.AddScore (scoreValue, indexer.playerIndex);
	//		}

	//		Destroy (other.gameObject); 
	//	}
			
	//	Destroy (gameObject);
	//}
}
