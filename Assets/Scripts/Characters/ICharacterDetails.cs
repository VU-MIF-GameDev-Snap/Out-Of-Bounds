using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implemented by each playable character
/// </summary>
public interface ICharacterDetails
{
	string PreviewResource { get; }
    string Name { get; }
    string Power1Name { get; }
    string Power1Description { get; }
    string Power2Name { get; }
    string Power2Description { get; }
    string BackgroundInformation { get; }
	GameObject CharacterPrefab { get; }
}