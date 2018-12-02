using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectionController : MonoBehaviour
{

	private IEnumerable<PlayerInputManager> _playerInputManagers;
	private IEnumerable<GameObject> _uiSelectPlayers;
	// Use this for initialization
	void Start ()
	{
		Globals.Players.Clear();
		_playerInputManagers = GetComponents(typeof(PlayerInputManager)).Where((Component c) => {
			return c is PlayerInputManager;
		}).Select((Component c) => {
			return c as PlayerInputManager;
		});
		_uiSelectPlayers = GameObject.FindGameObjectsWithTag("UI_CharacterSelectComponent");
	}

	// Update is called once per frame
	void Update ()
	{
		// Load first scene if at least 2 players have selected and confirmed selection.
		if(_uiSelectPlayers.Where((GameObject s) => {
			var selector = s.GetComponent<CharacterSelectionController>();
			return selector.PlayerId != 0;
		}).All((GameObject s) => {
			var selector = s.GetComponent<CharacterSelectionController>();
			return selector.Confirmed;
		}) && 2 <= _uiSelectPlayers.Count((GameObject s) => {
			var selector = s.GetComponent<CharacterSelectionController>();
			return selector.PlayerId != 0 && selector.Confirmed;
		}))
		{
			SceneManager.LoadScene(1);
		}

		foreach (var manager in _playerInputManagers)
		{
			if(_uiSelectPlayers.Any((o) =>
				{
					return o.GetComponent<CharacterSelectionController>().PlayerId == manager.PlayerId;
				}))
			{
				continue;
			}

			if(manager.IsButtonPressed(PlayerInputManager.Key.Jump))
			{
				// New player has pressed "join"
				// If there are any free spots only then add new player
				var freeSpot = _uiSelectPlayers.FirstOrDefault((GameObject o) =>
					{
						return (o.GetComponent<CharacterSelectionController>().PlayerId == 0);
					});

				if(freeSpot == null)
				{
					break;
				}

				freeSpot.GetComponent<CharacterSelectionController>().PlayerId = manager.PlayerId;
			}
		}
	}

	public void ResetPlayerSelections ()
	{
		foreach (var uiSelector in _uiSelectPlayers)
		{
			uiSelector.GetComponent<CharacterSelectionController>().Reset();
		}
		Globals.Players.Clear();
	}
}
