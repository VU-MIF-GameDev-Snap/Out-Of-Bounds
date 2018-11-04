using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	[Header("Who controls this player")]
	public int PlayerId = 1;
	private int _previousPlayerId = 1;

	public enum Key
	{
		MoveHorizontal,
		MoveVertical,
		Jump,
		Dash,
		Punch,
	}


	private Dictionary<Key, string> _playerKeyMappings = new Dictionary<Key, string>();

	public void Start ()
	{
		_playerKeyMappings = new Dictionary<Key, string>();
		var values = Enum.GetValues(typeof(Key)).Cast<Key>();
		foreach (var item in values)
		{
			_playerKeyMappings.Add(item, "Player" + PlayerId + "_" + item.ToString("G"));
		}
		_previousPlayerId = PlayerId;
	}

	public bool IsButtonDown (Key key)
	{
		UpdateKeyMappings();

		string value;
		if(!_playerKeyMappings.TryGetValue(key, out value))
		{
			return false;
		}
		
		return Input.GetButton(value);
	}

	public bool IsButtonPressed (Key key)
	{
		UpdateKeyMappings();

		string value;
		if(!_playerKeyMappings.TryGetValue(key, out value))
		{
			return false;
		}
		
		return Input.GetButtonDown(value);
	}

	public float GetAxis (Key key)
	{
		UpdateKeyMappings();

		string value;
		if(!_playerKeyMappings.TryGetValue(key, out value))
		{
			return 0;
		}
		
		return Input.GetAxis(value);
	}

	private void UpdateKeyMappings ()
	{
		if(PlayerId != _previousPlayerId)
		{
			Start();
		}
	}
}
