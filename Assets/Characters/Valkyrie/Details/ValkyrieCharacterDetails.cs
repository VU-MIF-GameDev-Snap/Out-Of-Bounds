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

    public string Power1Description
    {
        get
        {
            return "Does not like magic";
        }
    }

    public string Power2Description
    {
        get
        {
            return "Likes cakes";
        }
    }

    public string Description
    {
        get
        {
            return "This is a description, long description, whatever....";
        }
    }
}
