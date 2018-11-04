using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowPlayerHPScreen : MonoBehaviour
{
	private class HealthTracker
	{
		public PlayerInputManager PlayerIM;
		public PlayerController PlayerC;
		public GameObject Text;
	}

	private GameObject[] _allPlayers;
	private IList<HealthTracker> _healthTrackers = new List<HealthTracker>();

	public GameObject PlayerHpShowPrefab;

	void Start ()
	{
		_allPlayers = GameObject.FindGameObjectsWithTag("Player");

		foreach (var player in _allPlayers)
		{
			var inputManager = player.GetComponent<PlayerInputManager>();

			var hpShow = Instantiate(PlayerHpShowPrefab, new Vector3(), new Quaternion());
			hpShow.transform.SetParent(transform, false);
			_healthTrackers.Add(new HealthTracker{
				PlayerIM = player.GetComponent<PlayerInputManager>(),
				PlayerC = player.GetComponent<PlayerController>(),
				Text = hpShow
			});
		}
	}

	void Update ()
	{
		foreach (var healthTracker in _healthTrackers)
		{
			healthTracker.Text.transform.GetChild(0).GetComponent<Text>().text = "Player" + healthTracker.PlayerIM.PlayerId;
			healthTracker.Text.transform.GetChild(1).GetComponent<Text>().text = "hp: " + healthTracker.PlayerC.hitpoints;
		}
	}
}
