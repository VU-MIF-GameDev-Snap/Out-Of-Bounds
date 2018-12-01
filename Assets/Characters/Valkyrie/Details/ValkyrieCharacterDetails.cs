using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValkyrieCharacterDetails : ICharacterDetails
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
            return "Valkyrie";
        }
    }

    public string Power1Name
    {
        get
        {
            return "Invisibility";
        }
    }

    public string Power1Description
    {
        get
        {
            return "For the next 2 seconds become invisible and start healing.";
        }
    }

    public string Power2Name
    {
        get 
        {
            return "Homing Missiles";
        }
    }

    public string Power2Description
    {
        get
        {
            return "Launch 2 rockets that will actively seek out enemies.";
        }
    }

    public string BackgroundInformation
    {
        get
        {
            return "From the planet of ShanShan-52, Valkyrie has not yet met an opponent she was not able to defeat. Wearing a special battlesuit that was personally tailored by her mother has granted her a special place in the special forces of her home planet.";
        }
    }

    public GameObject CharacterPrefab
    {
        get
        {
            return Resources.Load("Characters/Valkyrie/Valkyrie") as GameObject;
        }
    }
}