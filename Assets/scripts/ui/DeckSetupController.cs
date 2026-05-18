using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSetupController : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text countryText;
    public TMP_Text heroCountText;
    public TMP_Text tacticCountText;
    public TMP_Text messageText;

    public Transform heroListParent;
    public Transform tacticListParent;

    public DeckCardButtonUI buttonPrefab;

    [Header("AI Profiles")]
    public AIPlayStyleProfile aggressiveProfile;
    public AIPlayStyleProfile defensiveProfile;
    public AIPlayStyleProfile balancedProfile;

    private CountryData currentCountry;

    private List<HeroCardData> selectedHeroes = new List<HeroCardData>();
    private List<TacticCardData> selectedTactics = new List<TacticCardData>();

    private Dictionary<HeroCardData, DeckCardButtonUI> heroButtons =
        new Dictionary<HeroCardData, DeckCardButtonUI>();

    private Dictionary<TacticCardData, DeckCardButtonUI> tacticButtons =
        new Dictionary<TacticCardData, DeckCardButtonUI>();

    private void Start()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession is missing");
            return;
        }

        currentCountry = GameSession.Instance.playerCountry;

        if (currentCountry == null)
        {
            Debug.LogError("player country is missing");
            return;
        }

        countryText.text = "Country: " + currentCountry.countryName;

        CreateHeroButtons();
        CreateTacticButtons();

        UpdateUI("Select 15 heroes and 9 tactics.");
    }

    private void CreateHeroButtons()
    {
        foreach (HeroCardData hero in currentCountry.heroPool)
        {
            DeckCardButtonUI button = Instantiate(buttonPrefab, heroListParent);
            button.SetupHero(hero, this);
            heroButtons.Add(hero, button);
        }
    }

    private void CreateTacticButtons()
    {
        foreach (TacticCardData tactic in currentCountry.tacticPool)
        {
            DeckCardButtonUI button = Instantiate(buttonPrefab, tacticListParent);
            button.SetupTactic(tactic, this);
            tacticButtons.Add(tactic, button);
        }
    }

    public void ToggleHero(HeroCardData hero)
    {
        if (selectedHeroes.Contains(hero))
        {
            selectedHeroes.Remove(hero);
            RemoveInvalidTacticsAfterHeroChange();
            UpdateUI(hero.heroName + " removed.");
            return;
        }

        if (selectedHeroes.Count >= 15)
        {
            UpdateUI("You can only select 15 heroes.");
            return;
        }

        selectedHeroes.Add(hero);
        UpdateUI(hero.heroName + " selected.");
    }

    public void ToggleTactic(TacticCardData tactic)
    {
        if (selectedTactics.Contains(tactic))
        {
            selectedTactics.Remove(tactic);
            UpdateUI(tactic.tacticName + " removed.");
            return;
        }

        if (selectedTactics.Count >= 9)
        {
            UpdateUI("You can only select 9 tactics.");
            return;
        }

        if (!CanUseTactic(tactic))
        {
            UpdateUI("This tactic requires a matching hero tag.");
            return;
        }

        selectedTactics.Add(tactic);
        UpdateUI(tactic.tacticName + " selected.");
    }

    private bool CanUseTactic(TacticCardData tactic)
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

        foreach (HeroCardData hero in selectedHeroes)
        {
            if (hero == null || hero.tags == null)
            {
                continue;
            }

            foreach (TagData requiredTag in tactic.requiredTags)
            {
                if (requiredTag != null && hero.tags.Contains(requiredTag))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void RemoveInvalidTacticsAfterHeroChange()
    {
        for (int i = selectedTactics.Count - 1; i >= 0; i--)
        {
            if (!CanUseTactic(selectedTactics[i]))
            {
                selectedTactics.RemoveAt(i);
            }
        }
    }

    public void BackToPlayerSetup()
    {
        SceneManager.LoadScene("player_setup");
    }

    public void ConfirmAndGoNext()
    {
        if (selectedHeroes.Count != 15)
        {
            UpdateUI("You must select exactly 15 heroes.");
            return;
        }

        if (selectedTactics.Count != 9)
        {
            UpdateUI("You must select exactly 9 tactics.");
            return;
        }

        if (GameSession.Instance == null)
        {
            Debug.LogError("GameSession is missing");
            return;
        }

        SavePlayerDeck();

        bool enemyDeckCreated = CreateAndSaveEnemyDeck();

        if (!enemyDeckCreated)
        {
            UpdateUI("Enemy deck could not be created. Check AI setup.");
            return;
        }

        SceneManager.LoadScene("opponent_deck_preview");
    }

    private void SavePlayerDeck()
    {
        GameSession.Instance.selectedHeroes.Clear();
        GameSession.Instance.selectedHeroes.AddRange(selectedHeroes);

        GameSession.Instance.selectedTactics.Clear();
        GameSession.Instance.selectedTactics.AddRange(selectedTactics);
    }

    private bool CreateAndSaveEnemyDeck()
    {
        CountryData enemyCountry = GameSession.Instance.enemyCountry;

        if (enemyCountry == null)
        {
            Debug.LogError("enemy country is missing");
            return false;
        }

        AIPlayStyleProfile profile = GetProfileForStyle(GameSession.Instance.enemyBasePlayStyle);

        if (profile == null)
        {
            Debug.LogError("AI profile is missing for style: " + GameSession.Instance.enemyBasePlayStyle);
            return false;
        }

        AIDeckSelectionResult enemyDeck = AIDeckPlanner.BuildDeck(
            enemyCountry,
            GameSession.Instance.enemyDifficulty,
            profile
        );

        if (enemyDeck == null)
        {
            Debug.LogError("AI deck result is null");
            return false;
        }

        if (!enemyDeck.IsComplete())
        {
            Debug.LogError(
                "Enemy deck is incomplete. Heroes = " +
                enemyDeck.selectedHeroes.Count +
                "/15, Tactics = " +
                enemyDeck.selectedTactics.Count +
                "/9"
            );

            return false;
        }

        GameSession.Instance.enemySelectedHeroes.Clear();
        GameSession.Instance.enemySelectedHeroes.AddRange(enemyDeck.selectedHeroes);

        GameSession.Instance.enemySelectedTactics.Clear();
        GameSession.Instance.enemySelectedTactics.AddRange(enemyDeck.selectedTactics);

        Debug.Log(
            "Enemy deck created. Style = " +
            GameSession.Instance.enemyBasePlayStyle +
            ", Difficulty = " +
            GameSession.Instance.enemyDifficulty +
            ", Heroes = " +
            GameSession.Instance.enemySelectedHeroes.Count +
            ", Tactics = " +
            GameSession.Instance.enemySelectedTactics.Count
        );

        return true;
    }

    private AIPlayStyleProfile GetProfileForStyle(AIPlayStyle style)
    {
        if (style == AIPlayStyle.Aggressive)
        {
            return aggressiveProfile;
        }

        if (style == AIPlayStyle.Defensive)
        {
            return defensiveProfile;
        }

        return balancedProfile;
    }

    private void UpdateUI(string message)
    {
        heroCountText.text = "Heroes: " + selectedHeroes.Count + " / 15";
        tacticCountText.text = "Tactics: " + selectedTactics.Count + " / 9";
        messageText.text = message;

        foreach (KeyValuePair<HeroCardData, DeckCardButtonUI> pair in heroButtons)
        {
            bool isSelected = selectedHeroes.Contains(pair.Key);
            pair.Value.RefreshVisual(isSelected, true);
        }

        foreach (KeyValuePair<TacticCardData, DeckCardButtonUI> pair in tacticButtons)
        {
            bool isSelected = selectedTactics.Contains(pair.Key);
            bool isAvailable = CanUseTactic(pair.Key);
            pair.Value.RefreshVisual(isSelected, isAvailable);
        }
    }
}