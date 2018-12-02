using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleanCharacterDetails : ICharacterDetails
{

    public string PreviewResource
    {
        get
        {
            return "Characters/Malean/Preview";
        }
    }

	string ICharacterDetails.Name
    {
        get
        {
            return "Malean";
        }
    }

    public string Power1Name
    {
        get
        {
            return "Cyborg Strike";
        }
    }

    public string Power1Description
    {
        get
        {
            return "Cyborg Strike - strikes very hard up or down";
        }
    }

    public string Power2Name
    {
        get
        {
            return "Black Hole";
        }
    }

    public string Power2Description
    {
        get
        {
            return "Black hole - casts a black hole which sucks";
        }
    }

    public string BackgroundInformation
    {
        get
        {
            return "Malean was specifically created to fight in arenas. It doesn't have feelings nor emotions, it's only purpose is to win. Every time Malean loses its memory is wiped to cleanly from this sad occurence.";
        }
    }

    public GameObject CharacterPrefab
    {
        get
        {
            return Resources.Load("Characters/Malean/Malean") as GameObject;
        }
    }
}
