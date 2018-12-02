using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharactersManager
{
	private static IList<ICharacterDetails> _charactersList = new List<ICharacterDetails>();
    public static IList<ICharacterDetails> CharactersList
    {
        get { return _charactersList; }
    }


	static CharactersManager ()
	{
		CharactersList.Add(new ValkyrieCharacterDetails());
		CharactersList.Add(new BonbonCharacterDetails());
		CharactersList.Add(new MaleanCharacterDetails());
		CharactersList.Add(new PenkratjevDetails());
	}
}
