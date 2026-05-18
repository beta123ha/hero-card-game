using System.Collections.Generic;
using UnityEngine;

public static class AITerrainScorer
{
    public static float ScoreTerrainForDeck(
        TerrainData terrain,
        List<HeroCardData> selectedHeroes,
        AIPlayStyleProfile profile,
        AIDifficulty difficulty
    )
    {
        if (terrain == null)
        {
            return -999999f;
        }

        float score = 0f;

        score += ScoreTerrainBonusByProfile(terrain, profile);
        score += ScoreTagSynergy(terrain, selectedHeroes, profile);
        score += GetRandomBonus(profile, difficulty);

        return score;
    }

    private static float ScoreTerrainBonusByProfile(
        TerrainData terrain,
        AIPlayStyleProfile profile
    )
    {
        float attackWeight = 3f;
        float defenseWeight = 3f;
        float healthWeight = 3f;
        float terrainSynergyWeight = 3f;

        if (profile != null)
        {
            attackWeight = profile.attackWeight;
            defenseWeight = profile.defenseWeight;
            healthWeight = profile.healthWeight;
            terrainSynergyWeight = profile.terrainSynergyWeight;
        }

        float score = 0f;

        score += terrain.attackBonus * attackWeight;
        score += terrain.defenseBonus * defenseWeight;
        score += terrain.healthBonus * healthWeight;

        score *= terrainSynergyWeight;

        return score;
    }

    private static float ScoreTagSynergy(
        TerrainData terrain,
        List<HeroCardData> selectedHeroes,
        AIPlayStyleProfile profile
    )
    {
        if (terrain.favoredTags == null || selectedHeroes == null)
        {
            return 0f;
        }

        float terrainSynergyWeight = 3f;

        if (profile != null)
        {
            terrainSynergyWeight = profile.terrainSynergyWeight;
        }

        int matchCount = 0;

        foreach (HeroCardData hero in selectedHeroes)
        {
            if (hero == null || hero.tags == null)
            {
                continue;
            }

            foreach (TagData favoredTag in terrain.favoredTags)
            {
                if (favoredTag != null && hero.tags.Contains(favoredTag))
                {
                    matchCount++;
                }
            }
        }

        return matchCount * terrainSynergyWeight;
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
}