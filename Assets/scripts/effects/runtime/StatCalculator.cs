using System.Collections.Generic;

public static class StatCalculator
{
    public static int CalculateAttack(
        int baseAttack,
        List<EffectInstance> activeEffects
    )
    {
        return CalculateStat(baseAttack, StatType.Attack, activeEffects);
    }

    public static int CalculateDefense(
        int baseDefense,
        List<EffectInstance> activeEffects
    )
    {
        return CalculateStat(baseDefense, StatType.Defense, activeEffects);
    }

    public static int CalculateHealth(
        int baseHealth,
        List<EffectInstance> activeEffects
    )
    {
        return CalculateStat(baseHealth, StatType.Health, activeEffects);
    }

    private static int CalculateStat(
        int baseValue,
        StatType statType,
        List<EffectInstance> activeEffects
    )
    {
        int result = baseValue;

        if (activeEffects != null)
        {
            foreach (EffectInstance effect in activeEffects)
            {
                if (effect == null || effect.effectData == null)
                {
                    continue;
                }

                StatModifierEffectData statEffect = effect.effectData as StatModifierEffectData;

                if (statEffect == null)
                {
                    continue;
                }

                if (statEffect.statType != statType)
                {
                    continue;
                }

                result += statEffect.value * effect.stackCount;
            }
        }

        if (result < 1)
        {
            result = 1;
        }

        return result;
    }
}