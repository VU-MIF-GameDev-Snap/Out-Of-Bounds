﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBoundary : MonoBehaviour {

	void OnTriggerExit(Collider other) {
        if (other.CompareTag("Weapon") || other.CompareTag("Bolt"))
        {
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Die();
            //Destroy(other.gameObject);
        }
    }
}
