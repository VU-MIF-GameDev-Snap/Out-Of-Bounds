using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBoundary : MonoBehaviour
{

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag("Weapon") || other.CompareTag("Bolt") || other.CompareTag("Missile"))
		{
			Destroy(other.gameObject);
		}

		if (other.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerController>().OnExitArena();
		}
	}

	void OnTriggerEnter (Collider other)
	{
        if (other.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerController>().OnReenterArena();
		}
	}
}
