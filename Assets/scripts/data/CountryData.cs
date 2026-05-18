using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCountry", menuName = "Game Data/Country")]
public class CountryData : ScriptableObject
{
    [Header("Basic Info")]
    public string countryName;

    [Header("Battlefield Layout")]
    public List<TerrainData> battlefieldTerrains = new List<TerrainData>();

    [Header("Card Pool")]
    public List<HeroCardData> heroPool = new List<HeroCardData>();
    public List<TacticCardData> tacticPool = new List<TacticCardData>();
}