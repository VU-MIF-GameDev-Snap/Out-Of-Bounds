using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputManager))]
public class CharacterSelectionController : MonoBehaviour
{
	private PlayerInputManager _inputManager;
	private int _loadedData = 0;
	private int _selectedCharacterId = 0;

	public bool Confirmed = false;

	public int PlayerId;

	[Header("Inner elements to control")]
	public GameObject ConfirmedObject;
	public GameObject CharacterImageGroup;
	public GameObject CharacterDescriptionGroup;
	public GameObject PressJoinObject;

	void Start ()
	{
		_inputManager = GetComponent<PlayerInputManager>();
		UnloadCharacterData();
	}

	void Update ()
	{
		if(Confirmed == true)
		{
			return;
		}

		if(PlayerId == 0)
		{
			UnloadCharacterData();
			return;
		}

		if(TryLoadCharacterData(_selectedCharacterId))
		{
			// This update just loaded character data, do not check current user input.
			return;
		}

		if(_inputManager.IsButtonPressed(PlayerInputManager.Key.Jump))
		{
			Confirmed = true;
			ConfirmedObject.SetActive(true);

			Globals.Players.Add(new PlayerDescriptor
				{
					ControlsId = _inputManager.PlayerId,
					CharacterId = _selectedCharacterId
				});
		}

		if(_inputManager.GetAxis(PlayerInputManager.Key.MoveVertical) < 0)
		{
			// move down the list
			if(_selectedCharacterId >= CharactersManager.CharactersList.Count -1)
			{
				// Reached the end.
			}
			else
			{
				_selectedCharacterId++;
			}

		}
		else if(_inputManager.GetAxis(PlayerInputManager.Key.MoveVertical) > 0)
		{
			// move up the list
			if(_selectedCharacterId <= 0)
			{
				// Reached the beginning.
			}
			else
			{
				_selectedCharacterId--;
			}
		}
	}

	public void Reset ()
	{
		PlayerId = 0;
		UnloadCharacterData();
		Confirmed = false;
	}

	void UpdateUpDownArrows ()
	{
		var upArrowImageObj = CharacterImageGroup.transform.GetChild(1).gameObject;
		upArrowImageObj.SetActive(_selectedCharacterId > 0);
		var downArrowImageObj = CharacterImageGroup.transform.GetChild(2).gameObject;
		downArrowImageObj.SetActive(_selectedCharacterId < CharactersManager.CharactersList.Count - 1);
	}

	bool TryLoadCharacterData (int characterId)
	{
		if(_loadedData == characterId)
		{
			return false;
		}

		_inputManager.PlayerId = PlayerId;
		_loadedData = characterId;

		CharacterImageGroup.SetActive(true);
		var imageObj = CharacterImageGroup.transform.GetChild(0).GetComponent<Image>();
		var imageTexture = Resources.Load(CharactersManager.CharactersList[characterId].PreviewResource) as Texture2D;
		imageObj.sprite = Sprite.Create(imageTexture, new Rect(0f, 0f, imageTexture.width, imageTexture.height), new Vector2());
		imageObj.color = new Color(1f, 1f, 1f);


		CharacterDescriptionGroup.SetActive(true);
		var titleObj = CharacterDescriptionGroup.transform.GetChild(0).GetComponent<Text>();
		titleObj.text = CharactersManager.CharactersList[characterId].Name;
		var power1Obj = CharacterDescriptionGroup.transform.GetChild(1).GetComponent<Text>();
		power1Obj.text = CharactersManager.CharactersList[characterId].Power1Description;
		var power2Obj = CharacterDescriptionGroup.transform.GetChild(2).GetComponent<Text>();
		power2Obj.text = CharactersManager.CharactersList[characterId].Power2Description;
		var descriptionObj = CharacterDescriptionGroup.transform.GetChild(3).GetComponent<Text>();
		descriptionObj.text = CharactersManager.CharactersList[characterId].Description;

		PressJoinObject.SetActive(false);

		UpdateUpDownArrows();

		return true;
	}
	void UnloadCharacterData ()
	{
		if(_loadedData == -1)
		{
			return;
		}

		_inputManager.PlayerId = 0;
		_loadedData = -1;

		CharacterImageGroup.SetActive(false);
		CharacterDescriptionGroup.SetActive(false);
		ConfirmedObject.SetActive(false);
		PressJoinObject.SetActive(true);
	}
}
