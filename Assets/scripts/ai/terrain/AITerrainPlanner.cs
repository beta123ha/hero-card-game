using System.Collections.Generic;
using UnityEngine;

public static class AITerrainPlanner
{
    private class SwapCandidate
    {
        public int firstIndex;
        public int secondIndex;
        public float scoreGain;

        public SwapCandidate(int firstIndex, int secondIndex, float scoreGain)
        {
            this.firstIndex = firstIndex;
            this.secondIndex = secondIndex;
            this.scoreGain = scoreGain;
        }
    }

    public static List<TerrainData> CreateInitialOrder(CountryData country)
    {
        List<TerrainData> result = new List<TerrainData>();

        if (country == null || country.battlefieldTerrains == null)
        {
            return result;
        }

        result.AddRange(country.battlefieldTerrains);
        Shuffle(result);

        return result;
    }

    public static AITerrainSwapDecision DecideNextSwap(
        List<TerrainData> enemyTerrainOrder,
        List<TerrainData> playerTerrainOrder,
        List<HeroCardData> enemyHeroes,
        List<HeroCardData> playerHeroes,
        AIPlayStyleProfile profile,
        AIDifficulty difficulty
    )
    {
        if (enemyTerrainOrder == null || playerTerrainOrder == null)
        {
            return AITerrainSwapDecision.NoSwap("Missing terrain order.");
        }

        if (enemyTerrainOrder.Count != 7 || playerTerrainOrder.Count != 7)
        {
            return AITerrainSwapDecision.NoSwap("Terrain order must have 7 slots.");
        }

        float currentScore = ScoreFullEnemyOrder(
            enemyTerrainOrder,
            playerTerrainOrder,
            enemyHeroes,
            playerHeroes,
            profile
        );

        List<SwapCandidate> candidates = new List<SwapCandidate>();

        for (int i = 0; i < enemyTerrainOrder.Count; i++)
        {
            for (int j = i + 1; j < enemyTerrainOrder.Count; j++)
            {
                List<TerrainData> testOrder = new List<TerrainData>();
                testOrder.AddRange(enemyTerrainOrder);

                Swap(testOrder, i, j);

                float newScore = ScoreFullEnemyOrder(
                    testOrder,
                    playerTerrainOrder,
                    enemyHeroes,
                    playerHeroes,
                    profile
                );

                float gain = newScore - currentScore;

                if (gain > 0.05f)
                {
                    candidates.Add(new SwapCandidate(i, j, gain));
                }
            }
        }

        if (candidates.Count == 0)
        {
            return AITerrainSwapDecision.NoSwap("No useful counter swap found.");
        }

        candidates.Sort((a, b) => b.scoreGain.CompareTo(a.scoreGain));

        SwapCandidate chosen = ChooseCandidateByDifficulty(candidates, difficulty);

        return new AITerrainSwapDecision(
            true,
            chosen.firstIndex,
            chosen.secondIndex,
            "Counter terrain swap. Gain = " + chosen.scoreGain
        );
    }

    private static SwapCandidate ChooseCandidateByDifficulty(
        List<SwapCandidate> candidates,
        AIDifficulty difficulty
    )
    {
        if (difficulty == AIDifficulty.Easy)
        {
            int limit = Mathf.Min(5, candidates.Count);
            int randomIndex = Random.Range(0, limit);
            return candidates[randomIndex];
        }

        if (difficulty == AIDifficulty.Normal)
        {
            int limit = Mathf.Min(3, candidates.Count);
            int randomIndex = Random.Range(0, limit);
            return candidates[randomIndex];
        }

        return candidates[0];
    }

    private static float ScoreFullEnemyOrder(
        List<TerrainData> enemyTerrainOrder,
        List<TerrainData> playerTerrainOrder,
        List<HeroCardData> enemyHeroes,
        List<HeroCardData> playerHeroes,
        AIPlayStyleProfile profile
    )
    {
        float totalScore = 0f;

        for (int i = 0; i < enemyTerrainOrder.Count; i++)
        {
            TerrainData enemyTerrain = enemyTerrainOrder[i];
            TerrainData playerTerrain = playerTerrainOrder[i];

            float slotImportance = GetSlotImportance(i);

            float slotScore = ScoreEnemySlotAgainstPlayerSlot(
                enemyTerrain,
                playerTerrain,
                enemyHeroes,
                playerHeroes,
                profile
            );

            totalScore += slotScore * slotImportance;
        }

        return totalScore;
    }

    private static float ScoreEnemySlotAgainstPlayerSlot(
        TerrainData enemyTerrain,
        TerrainData playerTerrain,
        List<HeroCardData> enemyHeroes,
        List<HeroCardData> playerHeroes,
        AIPlayStyleProfile profile
    )
    {
        float enemyTerrainFit = ScoreTerrainForDeck(enemyTerrain, enemyHeroes, profile);
        float playerTerrainThreat = ScoreTerrainForDeckBalanced(playerTerrain, playerHeroes);

        AIPlayStyle style = AIPlayStyle.Balanced;

        if (profile != null)
        {
            style = profile.playStyle;
        }

        float counterWeight = 3f;

        if (profile != null)
        {
            counterWeight = profile.counterPlayerWeight;
        }

        if (style == AIPlayStyle.Aggressive)
        {
            float playerWeakness = Mathf.Max(0f, 20f - playerTerrainThreat);
            return enemyTerrainFit + playerWeakness * counterWeight;
        }

        if (style == AIPlayStyle.Defensive)
        {
            return enemyTerrainFit + playerTerrainThreat * counterWeight;
        }

        float balancedCounter =
            Mathf.Abs(enemyTerrainFit - playerTerrainThreat) * 0.5f +
            playerTerrainThreat * 0.5f;

        return enemyTerrainFit + balancedCounter * counterWeight;
    }

    private static float ScoreTerrainForDeck(
        TerrainData terrain,
        List<HeroCardData> heroes,
        AIPlayStyleProfile profile
    )
    {
        if (terrain == null)
        {
            return 0f;
        }

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

        int matchingTagCount = CountMatchingHeroTags(terrain, heroes);
        score += matchingTagCount * terrainSynergyWeight;

        return score;
    }

    private static float ScoreTerrainForDeckBalanced(
        TerrainData terrain,
        List<HeroCardData> heroes
    )
    {
        if (terrain == null)
        {
            return 0f;
        }

        float score = 0f;

        score += terrain.attackBonus * 3f;
        score += terrain.defenseBonus * 3f;
        score += terrain.healthBonus * 3f;

        int matchingTagCount = CountMatchingHeroTags(terrain, heroes);
        score += matchingTagCount * 3f;

        return score;
    }

    private static int CountMatchingHeroTags(
        TerrainData terrain,
        List<HeroCardData> heroes
    )
    {
        if (terrain == null || terrain.favoredTags == null || heroes == null)
        {
            return 0;
        }

        int count = 0;

        foreach (HeroCardData hero in heroes)
        {
            if (hero == null || hero.tags == null)
            {
                continue;
            }

            foreach (TagData favoredTag in terrain.favoredTags)
            {
                if (favoredTag != null && hero.tags.Contains(favoredTag))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static float GetSlotImportance(int slotIndex)
    {
        if (slotIndex == 3)
        {
            return 1.25f;
        }

        if (slotIndex == 2 || slotIndex == 4)
        {
            return 1.1f;
        }

        if (slotIndex == 1 || slotIndex == 5)
        {
            return 1f;
        }

        return 0.85f;
    }

    private static void Swap(List<TerrainData> list, int firstIndex, int secondIndex)
    {
        TerrainData temp = list[firstIndex];
        list[firstIndex] = list[secondIndex];
        list[secondIndex] = temp;
    }

    private static void Shuffle(List<TerrainData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            TerrainData temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}