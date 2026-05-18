using UnityEngine;

[CreateAssetMenu(fileName = "NewStatModifierEffect", menuName = "Game Data/Effects/Stat Modifier")]
public class StatModifierEffectData : EffectData
{
    [Header("Stat Modifier")]
    public StatType statType = StatType.Attack;
    public int value = 0;
}