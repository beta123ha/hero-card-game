using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardButtonUI : MonoBehaviour
{
    public TMP_Text labelText;

    private DeckSetupController controller;
    private HeroCardData heroData;
    private TacticCardData tacticData;
    private bool isHero;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void SetupHero(HeroCardData hero, DeckSetupController deckController)
    {
        isHero = true;
        heroData = hero;
        controller = deckController;

        RefreshVisual(false, true);
    }

    public void SetupTactic(TacticCardData tactic, DeckSetupController deckController)
    {
        isHero = false;
        tacticData = tactic;
        controller = deckController;

        RefreshVisual(false, true);
    }

    private void OnClick()
    {
        if (isHero)
        {
            controller.ToggleHero(heroData);
        }
        else
        {
            controller.ToggleTactic(tacticData);
        }
    }

    public void RefreshVisual(bool isSelected, bool isAvailable)
    {
        string baseName = isHero ? heroData.heroName : tacticData.tacticName;
        string suffix = "";

        if (isSelected)
        {
            suffix += " [selected]";
        }

        if (!isHero && !isAvailable)
        {
            suffix += " [locked]";
        }

        labelText.text = baseName + suffix;
    }
}