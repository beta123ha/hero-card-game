using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTacticCard", menuName = "Game Data/Tactic Card")]
public class TacticCardData : ScriptableObject
{
    [Header("Basic Info")]
    public string tacticName;
    public string countryName;
    public bool isShared;

    [Header("Requirement")]
    public List<TagData> requiredTags = new List<TagData>();

    [Header("Simple Effect For First Version")]
    public int attackBonus;
    public int defenseBonus;
    public int healthBonus;

    [Header("Tactic Effects")]
    public List<EffectData> tacticEffects = new List<EffectData>();

    [TextArea(3, 6)]
    public string effectDescription;

    [Header("Art")]
    public Sprite artwork;
}