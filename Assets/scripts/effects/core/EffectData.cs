using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    [Header("Basic Info")]
    public string effectName;

    [TextArea(2, 4)]
    public string description;

    [Header("Target")]
    public EffectTargetType targetType = EffectTargetType.SelectedAllyHero;

    [Header("Duration")]
    public EffectDurationType durationType = EffectDurationType.UntilEndOfTurn;
    public int durationTurns = 1;

    [Header("Stacking")]
    public EffectStackingType stackingType = EffectStackingType.NotStackableKeepStrongest;
    public int maxStacks = 1;

    [Tooltip("Effects with the same stack key are considered the same kind of effect. If empty, effectName will be used.")]
    public string stackKey;
}