using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class CharacterSelectionController : MonoBehaviour
{
	private PlayerInputManager _inputManager;
	public bool Confirmed = false;
	private bool _loadedData = true;

	public int PlayerId;

	void Start ()
	{
		_inputManager = GetComponent<PlayerInputManager>();
		UnloadCharacterData();
	}
	
	void Update ()
	{
		if(Confirmed == true)
			return;
		
		if(PlayerId == 0)
		{
			UnloadCharacterData();
			return;
		}
		else
		{
			LoadChardacterData();
		}
		
		if(_inputManager.IsButtonPressed(PlayerInputManager.Key.Jump))
		{
			// Mark as confirmed.
			Confirmed = true;
		}
	}

	void LoadChardacterData ()
	{
		if(_loadedData)
			return;

		_inputManager.PlayerId = PlayerId;
		_loadedData = true;
		var field1 = transform.GetChild(0);
		var field2 = transform.GetChild(1);
		var field3 = transform.GetChild(2);
		field1.gameObject.SetActive(true);
		field2.gameObject.SetActive(true);
		field3.gameObject.SetActive(false);
	}
	void UnloadCharacterData ()
	{
		if(!_loadedData)
			return;

		_inputManager.PlayerId = PlayerId;
		_loadedData = false;
		var field1 = transform.GetChild(0);
		var field2 = transform.GetChild(1);
		var field3 = transform.GetChild(2);
		field1.gameObject.SetActive(false);
		field2.gameObject.SetActive(false);
		field3.gameObject.SetActive(true);
	}
}
