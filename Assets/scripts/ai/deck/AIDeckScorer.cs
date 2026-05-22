using System.Collections.Generic;
using UnityEngine;

public static class AIDeckScorer
{
    public static float ScoreHero(
        HeroCardData hero,
        CountryData country,
        AIPlayStyleProfile profile,
        AIDifficulty difficulty
    )
    {
        if (hero == null)
        {
            return -999999f;
        }

        float attackWeight = GetAttackWeight(profile);
        float defenseWeight = GetDefenseWeight(profile);
        float healthWeight = GetHealthWeight(profile);
        float comboWeight = GetComboWeight(profile);

        float score = 0f;

        score += hero.baseAttack * attackWeight;
        score += hero.baseDefense * defenseWeight;
        score += hero.baseHealth * healthWeight;

        score += ScoreEffectList(hero.passiveEffects, profile);

        int comboCount = CountPossibleTacticCombos(hero, country);
        score += comboCount * comboWeight;

        score += GetRandomBonus(profile, difficulty);

        return score;
    }

    public static float ScoreTactic(
        TacticCardData tactic,
        List<HeroCardData> selectedHeroes,
        AIPlayStyleProfile profile,
        AIDifficulty difficulty
    )
    {
        if (tactic == null)
        {
            return -999999f;
        }

        float comboWeight = GetComboWeight(profile);
        float tacticSynergyWeight = GetTacticSynergyWeight(profile);

        float score = 0f;

        score += ScoreEffectList(tactic.tacticEffects, profile);

        if (tactic.isShared)
        {
            score += tacticSynergyWeight;
        }

        int matchingHeroCount = CountMatchingHeroesForTactic(tactic, selectedHeroes);
        score += matchingHeroCount * comboWeight;

        if (tactic.tacticEffects == null || tactic.tacticEffects.Count == 0)
        {
            score -= 20f;
        }

        score += GetRandomBonus(profile, difficulty);

        return score;
    }

    public static bool CanUseTactic(
        TacticCardData tactic,
        List<HeroCardData> selectedHeroes
    )
    {
        if (tactic == null)
        {
            return false;
        }

        if (tactic.isShared)
        {
            return true;
        }

        if (tactic.requiredTags == null || tactic.requiredTags.Count == 0)
        {
            return true;
        }

        if (selectedHeroes == null)
        {
            return false;
        }

        foreach (HeroCardData hero in selectedHeroes)
        {
            if (HeroMatchesTactic(hero, tactic))
            {
                return true;
            }
        }

        return false;
    }

    private static float ScoreEffectList(
        List<EffectData> effects,
        AIPlayStyleProfile profile
    )
    {
        if (effects == null || effects.Count == 0)
        {
            return 0f;
        }

        float totalScore = 0f;

        foreach (EffectData effect in effects)
        {
            totalScore += ScoreEffect(effect, profile);
        }

        return totalScore;
    }

    private static float ScoreEffect(
        EffectData effect,
        AIPlayStyleProfile profile
    )
    {
        if (effect == null)
        {
            return 0f;
        }

        StatModifierEffectData statEffect = effect as StatModifierEffectData;

        if (statEffect != null)
        {
            return ScoreStatModifierEffect(statEffect, profile);
        }

        return ScoreSpecialEffect(effect, profile);
    }

    private static float ScoreStatModifierEffect(
        StatModifierEffectData effect,
        AIPlayStyleProfile profile
    )
    {
        float statWeight = GetWeightForStat(effect.statType, profile);
        float targetMultiplier = GetTargetMultiplier(effect.targetType);
        float durationMultiplier = GetDurationMultiplier(effect);
        float stackMultiplier = GetStackMultiplier(effect);

        return effect.value
            * statWeight
            * targetMultiplier
            * durationMultiplier
            * stackMultiplier;
    }

    private static float ScoreSpecialEffect(
        EffectData effect,
        AIPlayStyleProfile profile
    )
    {
        float baseScore = GetTacticSynergyWeight(profile);
        float targetMultiplier = Mathf.Abs(GetTargetMultiplier(effect.targetType));
        float durationMultiplier = GetDurationMultiplier(effect);
        float stackMultiplier = GetStackMultiplier(effect);

        return baseScore * targetMultiplier * durationMultiplier * stackMultiplier;
    }

    private static float GetWeightForStat(
        StatType statType,
        AIPlayStyleProfile profile
    )
    {
        if (statType == StatType.Attack)
        {
            return GetAttackWeight(profile);
        }

        if (statType == StatType.Defense)
        {
            return GetDefenseWeight(profile);
        }

        if (statType == StatType.Health)
        {
            return GetHealthWeight(profile);
        }

        return 1f;
    }

    private static float GetTargetMultiplier(EffectTargetType targetType)
    {
        switch (targetType)
        {
            case EffectTargetType.SelfHero:
            case EffectTargetType.SelectedAllyHero:
            case EffectTargetType.OwnerPlayer:
                return 1f;

            case EffectTargetType.AllAllyHeroes:
                return 1.5f;

            case EffectTargetType.SelectedEnemyHero:
            case EffectTargetType.OpponentPlayer:
                return -1f;

            case EffectTargetType.AllEnemyHeroes:
                return -1.5f;

            default:
                return 1f;
        }
    }

    private static float GetDurationMultiplier(EffectData effect)
    {
        if (effect == null)
        {
            return 1f;
        }

        if (effect.durationType == EffectDurationType.Instant)
        {
            return 0.8f;
        }

        if (effect.durationType == EffectDurationType.UntilEndOfTurn)
        {
            return Mathf.Max(1f, effect.durationTurns);
        }

        if (effect.durationType == EffectDurationType.Permanent)
        {
            return 2f;
        }

        if (effect.durationType == EffectDurationType.WhileConditionTrue)
        {
            return 1.5f;
        }

        return 1f;
    }

    private static float GetStackMultiplier(EffectData effect)
    {
        if (effect == null)
        {
            return 1f;
        }

        if (effect.stackingType == EffectStackingType.Stackable)
        {
            return 1.4f;
        }

        if (effect.stackingType == EffectStackingType.StackableWithLimit)
        {
            return 1f + Mathf.Max(0, effect.maxStacks - 1) * 0.2f;
        }

        return 1f;
    }

    private static int CountPossibleTacticCombos(
        HeroCardData hero,
        CountryData country
    )
    {
        if (hero == null || country == null || country.tacticPool == null)
        {
            return 0;
        }

        int count = 0;

        foreach (TacticCardData tactic in country.tacticPool)
        {
            if (tactic == null)
            {
                continue;
            }

            if (tactic.isShared)
            {
                continue;
            }

            if (HeroMatchesTactic(hero, tactic))
            {
                count++;
            }
        }

        return count;
    }

    private static int CountMatchingHeroesForTactic(
        TacticCardData tactic,
        List<HeroCardData> selectedHeroes
    )
    {
        if (tactic == null || selectedHeroes == null)
        {
            return 0;
        }

        if (tactic.isShared)
        {
            return 0;
        }

        int count = 0;

        foreach (HeroCardData hero in selectedHeroes)
        {
            if (HeroMatchesTactic(hero, tactic))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HeroMatchesTactic(
        HeroCardData hero,
        TacticCardData tactic
    )
    {
        if (hero == null || tactic == null)
        {
            return false;
        }

        if (hero.tags == null || tactic.requiredTags == null)
        {
            return false;
        }

        foreach (TagData requiredTag in tactic.requiredTags)
        {
            if (requiredTag != null && hero.tags.Contains(requiredTag))
            {
                return true;
            }
        }

        return false;
    }

    private static float GetRandomBonus(
        AIPlayStyleProfile profile,
        AIDifficulty difficulty
    )
    {
        float baseRandom = 1f;

        if (profile != null)
        {
            baseRandom = profile.randomnessWeight;
        }

        if (difficulty == AIDifficulty.Easy)
        {
            return Random.Range(0f, baseRandom * 5f);
        }

        if (difficulty == AIDifficulty.Normal)
        {
            return Random.Range(0f, baseRandom * 2f);
        }

        return Random.Range(0f, baseRandom * 0.5f);
    }

    private static float GetAttackWeight(AIPlayStyleProfile profile)
    {
        return profile != null ? profile.attackWeight : 3f;
    }

    private static float GetDefenseWeight(AIPlayStyleProfile profile)
    {
        return profile != null ? profile.defenseWeight : 3f;
    }

    private static float GetHealthWeight(AIPlayStyleProfile profile)
    {
        return profile != null ? profile.healthWeight : 3f;
    }

    private static float GetComboWeight(AIPlayStyleProfile profile)
    {
        return profile != null ? profile.comboWeight : 3f;
    }

    private static float GetTacticSynergyWeight(AIPlayStyleProfile profile)
    {
        return profile != null ? profile.tacticSynergyWeight : 3f;
    }
}