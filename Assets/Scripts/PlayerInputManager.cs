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
		AimHorizontal,
		AimVertical,
		Jump,
		Dash,
		Punch,
		Power1,
        Shoot
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

	public Vector2 GetAimDirection ()
	{
		if(PlayerId == 1)
		{
			// Special case for Player1: Aim direction is direction from player to mouse cursor.

			var playerPosOnScreen = Camera.main.WorldToScreenPoint(transform.position);
			return (Input.mousePosition - playerPosOnScreen).normalized;
		}

		var moveHorizontal = GetAxis(Key.MoveHorizontal);
		var moveVertical = GetAxis(Key.MoveVertical);
		var aimHorizontal = GetAxis(Key.AimHorizontal);
		var aimVertical = GetAxis(Key.AimVertical);
		if(aimHorizontal == 0 && aimVertical == 0)
		{
			return new Vector2(moveHorizontal, moveVertical).normalized;
		}
		return new Vector2(aimHorizontal, aimVertical).normalized;
	}

	private void UpdateKeyMappings ()
	{
		if(PlayerId != _previousPlayerId)
		{
			Start();
		}
	}
}
