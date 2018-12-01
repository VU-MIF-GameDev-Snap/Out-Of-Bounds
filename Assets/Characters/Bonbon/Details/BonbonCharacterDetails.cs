using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonbonCharacterDetails : ICharacterDetails
{
    public string PreviewResource
    {
        get
        {
            return "Characters/Bonbon/Preview";
        }
    }

	string ICharacterDetails.Name
    {
        get
        {
            return "Bonbon";
        }
    }

    public string Power1Name
    {
        get
        {
            return "Charged punch";
        }
    }

    public string Power1Description
    {
        get
        {
            return "A special punch whose damage depends on how long it was charged.";
        }
    }

    public string Power2Name
    {
        get
        {
            return "Stoneify";
        }
    }

    public string Power2Description
    {
        get
        {
            return "Becomes immune to pushback attacks but moves slower.";
        }
    }

    public string BackgroundInformation
    {
        get
        {
            return "No background information is available on Bonbon. Some say it is due to the fact that he was a pirate and before retiring was able to use his power to erase any traces of him.";
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