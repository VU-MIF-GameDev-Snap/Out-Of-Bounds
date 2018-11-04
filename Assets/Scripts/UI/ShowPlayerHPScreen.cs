using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowPlayerHPScreen : MonoBehaviour
{
	private GameObject[] _allPlayers;

	void Start ()
	{
		_allPlayers = GameObject.FindGameObjectsWithTag("Player");
		foreach (var player in allPlayers)
		{
			var inputManager = player.GetComponent<PlayerInputManager>();
			transform.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Player" + inputManager.PlayerId;
			transform.gameObject.transform.GetChild(1).GetComponent<Text>().text = player.GetComponent<PlayerController>().HP;
		}
	}
}
