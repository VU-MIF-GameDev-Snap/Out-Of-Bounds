using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	[Header("Who controls this player")]
	public int PlayerId = 1;

	public enum Key
	{
		MoveHorizontal,
		MoveVertical,
		Jump,
		Dash,
		Punch,
		Power1,
	}


	private Dictionary<Key, string> _playerKeyMappings = new Dictionary<Key, string>();

	public void Start ()
	{
		var values = Enum.GetValues(typeof(Key)).Cast<Key>();
		foreach (var item in values)
		{
			_playerKeyMappings.Add(item, "Player" + PlayerId + "_" + item.ToString("G"));
		}
	}

	public bool IsButtonDown (Key key)
	{
		string value;
		if(!_playerKeyMappings.TryGetValue(key, out value))
		{
			return false;
		}
		
		return Input.GetButton(value);
	}

	public bool IsButtonPressed (Key key)
	{
		string value;
		if(!_playerKeyMappings.TryGetValue(key, out value))
		{
			return false;
		}
		
		return Input.GetButtonDown(value);
	}

	public float GetAxis (Key key)
	{
		string value;
		if(!_playerKeyMappings.TryGetValue(key, out value))
		{
			return 0;
		}
		
		return Input.GetAxis(value);
	}
}
