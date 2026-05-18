using System.Collections.Generic;
using UnityEngine;

public static class AIDeckPlanner
{
    private class HeroScore
    {
        public HeroCardData hero;
        public float score;

        public HeroScore(HeroCardData hero, float score)
        {
            this.hero = hero;
            this.score = score;
        }
    }

    private class TacticScore
    {
        public TacticCardData tactic;
        public float score;

        public TacticScore(TacticCardData tactic, float score)
        {
            this.tactic = tactic;
            this.score = score;
        }
    }

    public static AIDeckSelectionResult BuildDeck(
        CountryData country,
        AIDifficulty difficulty,
        AIPlayStyleProfile profile
    )
    {
        AIDeckSelectionResult result = new AIDeckSelectionResult();

        if (country == null)
        {
            Debug.LogError("AI deck planner failed: country is null");
            return result;
        }

        if (country.heroPool == null || country.heroPool.Count == 0)
        {
            Debug.LogError("AI deck planner failed: hero pool is empty");
            return result;
        }

        if (country.tacticPool == null || country.tacticPool.Count == 0)
        {
            Debug.LogError("AI deck planner failed: tactic pool is empty");
            return result;
        }

        SelectHeroes(country, difficulty, profile, result);
        SelectTactics(country, difficulty, profile, result);

        if (!result.IsComplete())
        {
            Debug.LogWarning(
                "AI deck is not complete. Heroes = " +
                result.selectedHeroes.Count +
                "/15, Tactics = " +
                result.selectedTactics.Count +
                "/9"
            );
        }

        return result;
    }

    private static void SelectHeroes(
        CountryData country,
        AIDifficulty difficulty,
        AIPlayStyleProfile profile,
        AIDeckSelectionResult result
    )
    {
        List<HeroScore> scoredHeroes = new List<HeroScore>();

        foreach (HeroCardData hero in country.heroPool)
        {
            if (hero == null)
            {
                continue;
            }

            float score = AIDeckScorer.ScoreHero(hero, country, profile, difficulty);
            scoredHeroes.Add(new HeroScore(hero, score));
        }

        scoredHeroes.Sort((a, b) => b.score.CompareTo(a.score));

        for (int i = 0; i < scoredHeroes.Count; i++)
        {
            if (result.selectedHeroes.Count >= 15)
            {
                break;
            }

            result.selectedHeroes.Add(scoredHeroes[i].hero);
        }
    }

    private static void SelectTactics(
        CountryData country,
        AIDifficulty difficulty,
        AIPlayStyleProfile profile,
        AIDeckSelectionResult result
    )
    {
        List<TacticScore> scoredTactics = new List<TacticScore>();

        foreach (TacticCardData tactic in country.tacticPool)
        {
            if (tactic == null)
            {
                continue;
            }

            if (!AIDeckScorer.CanUseTactic(tactic, result.selectedHeroes))
            {
                continue;
            }

            float score = AIDeckScorer.ScoreTactic(
                tactic,
                result.selectedHeroes,
                profile,
                difficulty
            );

            scoredTactics.Add(new TacticScore(tactic, score));
        }

        scoredTactics.Sort((a, b) => b.score.CompareTo(a.score));

        for (int i = 0; i < scoredTactics.Count; i++)
        {
            if (result.selectedTactics.Count >= 9)
            {
                break;
            }

            result.selectedTactics.Add(scoredTactics[i].tactic);
        }
    }
}