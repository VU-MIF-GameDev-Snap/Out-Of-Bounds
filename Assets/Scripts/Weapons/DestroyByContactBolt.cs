using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyByContactBolt : MonoBehaviour {
    
	private GameController _gameController;

	void Start() {

	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
