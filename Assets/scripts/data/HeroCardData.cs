using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroCard", menuName = "Game Data/Hero Card")]
public class HeroCardData : ScriptableObject
{
    [Header("Basic Info")]
    public string heroName;
    public string countryName;

    [Header("Base Stats")]
    public int baseAttack = 1;
    public int baseDefense = 1;
    public int baseHealth = 1;

    [Header("Identity")]
    public List<TagData> tags = new List<TagData>();

    [TextArea(3, 6)]
    public string passiveDescription;

    [Header("Passive Effects")]
    public List<EffectData> passiveEffects = new List<EffectData>();

    [Header("Art")]
    public Sprite artwork;
}