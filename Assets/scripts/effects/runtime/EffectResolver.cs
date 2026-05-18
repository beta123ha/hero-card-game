using System.Collections.Generic;
using UnityEngine;

public static class EffectResolver
{
    public static void AddEffect(
        List<EffectInstance> activeEffects,
        EffectData effectData,
        string sourceName
    )
    {
        if (activeEffects == null || effectData == null)
        {
            return;
        }

        EffectInstance existingEffect = FindSameStackEffect(activeEffects, effectData);

        if (existingEffect == null)
        {
            activeEffects.Add(new EffectInstance(effectData, sourceName));
            return;
        }

        ApplyStackingRule(activeEffects, existingEffect, effectData, sourceName);
    }

    public static void TickEndOfTurn(List<EffectInstance> activeEffects)
    {
        if (activeEffects == null)
        {
            return;
        }

        foreach (EffectInstance effect in activeEffects)
        {
            effect.TickTurn();
        }

        RemoveExpiredEffects(activeEffects);
    }

    public static void RemoveExpiredEffects(List<EffectInstance> activeEffects)
    {
        if (activeEffects == null)
        {
            return;
        }

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i] == null || activeEffects[i].IsExpired())
            {
                activeEffects.RemoveAt(i);
            }
        }
    }

    private static void ApplyStackingRule(
        List<EffectInstance> activeEffects,
        EffectInstance existingEffect,
        EffectData newEffectData,
        string sourceName
    )
    {
        if (existingEffect == null || newEffectData == null)
        {
            return;
        }

        switch (newEffectData.stackingType)
        {
            case EffectStackingType.NotStackableKeepStrongest:
                if (IsNewEffectStronger(newEffectData, existingEffect.effectData))
                {
                    activeEffects.Remove(existingEffect);
                    activeEffects.Add(new EffectInstance(newEffectData, sourceName));
                }
                break;

            case EffectStackingType.NotStackableRefreshDuration:
                existingEffect.RefreshDuration();
                break;

            case EffectStackingType.Stackable:
                existingEffect.AddStack();
                break;

            case EffectStackingType.StackableWithLimit:
                existingEffect.AddStack();
                break;
        }
    }

    private static EffectInstance FindSameStackEffect(
        List<EffectInstance> activeEffects,
        EffectData effectData
    )
    {
        string newStackKey = GetStackKey(effectData);

        foreach (EffectInstance effect in activeEffects)
        {
            if (effect == null || effect.effectData == null)
            {
                continue;
            }

            string existingStackKey = GetStackKey(effect.effectData);

            if (existingStackKey == newStackKey)
            {
                return effect;
            }
        }

        return null;
    }

    private static string GetStackKey(EffectData effectData)
    {
        if (effectData == null)
        {
            return "";
        }

        if (!string.IsNullOrEmpty(effectData.stackKey))
        {
            return effectData.stackKey;
        }

        return effectData.effectName;
    }

    private static bool IsNewEffectStronger(
        EffectData newEffect,
        EffectData oldEffect
    )
    {
        if (newEffect == null)
        {
            return false;
        }

        if (oldEffect == null)
        {
            return true;
        }

        StatModifierEffectData newStatEffect = newEffect as StatModifierEffectData;
        StatModifierEffectData oldStatEffect = oldEffect as StatModifierEffectData;

        if (newStatEffect != null && oldStatEffect != null)
        {
            return Mathf.Abs(newStatEffect.value) > Mathf.Abs(oldStatEffect.value);
        }

        return false;
    }
}