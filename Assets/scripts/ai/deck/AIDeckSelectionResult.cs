using System.Collections.Generic;

public class AIDeckSelectionResult
{
    public List<HeroCardData> selectedHeroes = new List<HeroCardData>();
    public List<TacticCardData> selectedTactics = new List<TacticCardData>();

    public bool IsComplete()
    {
        return selectedHeroes.Count == 15 && selectedTactics.Count == 9;
    }
}