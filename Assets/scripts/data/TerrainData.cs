using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTerrain", menuName = "Game Data/Terrain")]
public class TerrainData : ScriptableObject
{
    public string terrainName;

    [TextArea(2, 4)]
    public string description;

    [Header("AI / Gameplay Tags")]
    public List<TagData> favoredTags = new List<TagData>();

    [Header("Simple Terrain Bonus")]
    public int attackBonus;
    public int defenseBonus;
    public int healthBonus;
}