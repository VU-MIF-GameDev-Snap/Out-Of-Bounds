using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonbonCharacterDetails : ICharacterDetails
{

    public string PreviewResource
    {
        get
        {
            return "Characters/Valkyrie/Preview";
        }
    }

	string ICharacterDetails.Name
    {
        get
        {
            return "Bonbon";
        }
    }

    public string Power1Description
    {
        get
        {
            return "Some unknown power.";
        }
    }

    public string Power2Description
    {
        get
        {
            return "Stoneify";
        }
    }

    public string Description
    {
        get
        {
            return "Bonbon can't do magic, he just fights.";
        }
    }

    public GameObject CharacterPrefab
    {
        get
        {
            return Resources.Load("Characters/Bonbon/Bonbon") as GameObject;
        }
    }
}
