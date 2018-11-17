using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharactersManager
{
	// get full List?
	// Get by name?
	private static IList<ICharacterDetails> _charactersList = new List<ICharacterDetails>();

	static CharactersManager ()
	{
		CharactersList.Add(new ValkyrieCharacterDetails());
        CharactersList.Add(new BonbonCharacterDetails());
	}

    public static IList<ICharacterDetails> CharactersList
    {
        get
        {
            return _charactersList;
        }
    }
}
