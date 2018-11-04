using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSelectionController : MonoBehaviour {

	private IEnumerable<PlayerInputManager> _playerInputManagers;
	private IEnumerable<GameObject> _uiSelectPlayers;
	// Use this for initialization
	void Start () {
		_playerInputManagers = GetComponents(typeof(PlayerInputManager)).Where((Component c) => {
			return c is PlayerInputManager;
		}).Select((Component c) => {
			return c as PlayerInputManager;
		});
		_uiSelectPlayers = GameObject.FindGameObjectsWithTag("UI_CharacterSelectComponent");
	}
	
	// Update is called once per frame
	void Update () {
		// wait for button presses from all players "a" "space" stuff like that
		// if pressed, show required elements.
		// 
		if(!_uiSelectPlayers.Any((GameObject s) => {
			var selector = s.GetComponent<CharacterSelectionController>();
			return selector.PlayerId != 0 && !selector.Confirmed;
		}) && 2 <= _uiSelectPlayers.Count((GameObject s) => {
			var selector = s.GetComponent<CharacterSelectionController>();
			return selector.PlayerId != 0 && !selector.Confirmed;
		}))
		{
			return;
		}

		foreach (var manager in _playerInputManagers)
		{
			if(manager.IsButtonPressed(PlayerInputManager.Key.Jump))
			{
				if(Globals.ActivePlayers.Contains(manager.PlayerId) || Globals.ActivePlayers.Count >= 4)
				{
					break;
				}
				Globals.ActivePlayers.Add(manager.PlayerId);
				// set board 
				
				var freeSpot = _uiSelectPlayers.FirstOrDefault((GameObject o) => {
					return (o.GetComponent<CharacterSelectionController>().PlayerId == 0);
				});
				
				if(freeSpot == null)
					break;
				
				freeSpot.GetComponent<CharacterSelectionController>().PlayerId = manager.PlayerId;
			}
		}
	}

	
}
