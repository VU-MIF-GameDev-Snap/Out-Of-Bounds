using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlaylistController))]

public class GameController : MonoBehaviour
{
	[SerializeField]
	GameObject GameEndingPanel;
	[SerializeField]
	Text GameEndingWinnerTextField;
	[SerializeField]
	float GameWinAnnounceDelay = 2f;

	private AudioSource displayMusic;
	private PlaylistController _playlistController;
	private float _gameEndedAt = 0;
	private bool _gameEnded = false;
	public List<GameObject> weapons = new List<GameObject>();
	public List<AudioClip> songs = new List<AudioClip>();
	public Vector3 weaponSpawnValues;
	public float weaponSpawnWait;


	// Use this for initialization
	void Start ()
	{
		SpawnPlayers();
		StartCoroutine(SpawnWeaponWaves());
		_playlistController = GetComponent<PlaylistController>();
		displayMusic = GetComponent<AudioSource>();
		StartCoroutine(PlayBakgroundMusic());
	}

	// Update is called once per frame
	void Update ()
	{
		CheckPlayersAlive();
		HandleGameEnd();
	}

	private void HandleGameEnd ()
	{
		if (!_gameEnded || _gameEndedAt + 2 > Time.time)
		{
			return;
		}

		var existingPlayers = GameObject.FindGameObjectsWithTag("Player").Select((playerGO) =>
		{
			return playerGO.GetComponent<PlayerController>();
		});

		// Freeze game.
		Time.timeScale = 0;
		GameEndingPanel.SetActive(true);
		if (existingPlayers.Count() == 0)
		{
			GameEndingWinnerTextField.text = "Draw!";
		}
		else
		{
			GameEndingWinnerTextField.text = "And the winner is: Player" + existingPlayers.First().GetComponent<PlayerInputManager>().PlayerId;
		}
	}

	private void CheckPlayersAlive ()
	{
		if (_gameEnded)
		{
			return;
		}

		var existingPlayers = GameObject.FindGameObjectsWithTag("Player").Select((playerGO) =>
		{
			return playerGO.GetComponent<PlayerController>();
		});
		if (existingPlayers.Count() > 1)
		{
			return;
		}
		// Only 1 player left, announce winner after delay!
		_gameEnded = true;
		_gameEndedAt = Time.time;
	}

	IEnumerator PlayBakgroundMusic ()
	{
		while (true)
		{
			songs = _playlistController.Songs;
			if (songs.Count < 1)
			{
				// Nothing to play.
				break;
			}
			displayMusic.clip = songs[UnityEngine.Random.Range(0, songs.Count)];
			displayMusic.Play();
			yield return new WaitForSeconds(displayMusic.clip.length);
		}
	}

	IEnumerator SpawnWeaponWaves ()
	{
		if (weapons.Count == 0)
			yield return new WaitForSeconds(weaponSpawnWait);
		else
		{
			while (true)
			{
				GameObject weapon = weapons[UnityEngine.Random.Range(0, weapons.Count)];

				Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-weaponSpawnValues.x, weaponSpawnValues.x), weaponSpawnValues.y, weaponSpawnValues.z);
				Quaternion spawnRotation = Quaternion.Euler(0, 90, 0);
				Instantiate(weapon, spawnPosition, spawnRotation);

				yield return new WaitForSeconds(weaponSpawnWait);
			}
		}
	}

	private void SpawnPlayers ()
	{
		var playerSpawnpoints = GameObject.FindGameObjectWithTag("PlayerSpawnpoints");
		var spawnpointsList = new List<Vector3>();
		for (int i = 0; i < playerSpawnpoints.transform.childCount; i++)
		{
			spawnpointsList.Add(playerSpawnpoints.transform.GetChild(i).transform.position);
		}

		for (int i = 0; i < Globals.Players.Count; i++)
		{
			var targetSpawnpointId = UnityEngine.Random.Range(0, spawnpointsList.Count);
			var spawnpoint = spawnpointsList[targetSpawnpointId];

			var obj = Instantiate(CharactersManager.CharactersList[Globals.Players[i].CharacterId].CharacterPrefab, spawnpoint, new Quaternion());
			obj.GetComponent<PlayerInputManager>().PlayerId = Globals.Players[i].ControlsId;

			spawnpointsList.RemoveAt(targetSpawnpointId);
		}
	}
}
