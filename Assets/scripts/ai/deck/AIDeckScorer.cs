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

        float attackWeight = 3f;
        float defenseWeight = 3f;
        float healthWeight = 3f;
        float comboWeight = 3f;

        if (profile != null)
        {
            attackWeight = profile.attackWeight;
            defenseWeight = profile.defenseWeight;
            healthWeight = profile.healthWeight;
            comboWeight = profile.comboWeight;
        }

        float score = 0f;

        score += hero.baseAttack * attackWeight;
        score += hero.baseDefense * defenseWeight;
        score += hero.baseHealth * healthWeight;

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

        float attackWeight = 3f;
        float defenseWeight = 3f;
        float healthWeight = 3f;
        float comboWeight = 3f;
        float tacticSynergyWeight = 3f;

        if (profile != null)
        {
            attackWeight = profile.attackWeight;
            defenseWeight = profile.defenseWeight;
            healthWeight = profile.healthWeight;
            comboWeight = profile.comboWeight;
            tacticSynergyWeight = profile.tacticSynergyWeight;
        }

        float score = 0f;

        score += tactic.attackBonus * attackWeight;
        score += tactic.defenseBonus * defenseWeight;
        score += tactic.healthBonus * healthWeight;

        if (tactic.isShared)
        {
            score += tacticSynergyWeight;
        }

        int matchingHeroCount = CountMatchingHeroesForTactic(tactic, selectedHeroes);
        score += matchingHeroCount * comboWeight;

        score += GetRandomBonus(profile, difficulty);

        return score;
    }

    public static bool CanUseTactic(TacticCardData tactic, List<HeroCardData> selectedHeroes)
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

    private static int CountPossibleTacticCombos(HeroCardData hero, CountryData country)
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

    private static bool HeroMatchesTactic(HeroCardData hero, TacticCardData tactic)
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

    private static float GetRandomBonus(AIPlayStyleProfile profile, AIDifficulty difficulty)
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
}