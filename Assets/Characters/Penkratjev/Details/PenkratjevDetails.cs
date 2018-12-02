using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenkratjevDetails : ICharacterDetails
{
	public string PreviewResource
	{
		get
		{
			return "Characters/Penkratjev/Preview";
		}
	}

	public string Name
	{
		get
		{
			return "Penkratjev";
		}
	}

	public string Power1Name
	{
		get
		{
			return "Laser of grudge";
		}
	}

	public string Power1Description
	{
		get
		{
			return "Shoots a laser out of his head for some reason";
		}
	}

	public string Power2Name
	{
		get
		{
			return "Switch";
		}
	}

	public string Power2Description
	{
		get
		{
			return "Switches work place positions with poor player nearby";
		}
	}

	public string BackgroundInformation
	{
		get
		{
			return "That's the description";
		}
	}

	public GameObject CharacterPrefab
	{
		get
		{
			return Resources.Load("Characters/Penkratjev/Penkratjev") as GameObject;
		}
	}
}
