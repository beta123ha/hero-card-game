using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHandCardUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text cardNameText;
    public TMP_Text cardTypeText;
    public TMP_Text cardStatsText;
    public TMP_Text cardDescriptionText;

    private int handIndex;
    private BattleUIController owner;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    public void Setup(BattleCardInstance card, int handIndex, BattleUIController owner)
    {
        this.handIndex = handIndex;
        this.owner = owner;

        if (card == null)
        {
            ShowMissingCard();
            return;
        }

        if (cardNameText != null)
        {
            cardNameText.text = card.GetCardName();
        }

        if (card.cardType == BattleCardType.Hero)
        {
            ShowHeroCard(card.heroData);
        }
        else if (card.cardType == BattleCardType.Tactic)
        {
            ShowTacticCard(card.tacticData);
        }
        else
        {
            ShowMissingCard();
        }
    }

    private void ShowHeroCard(HeroCardData heroData)
    {
        if (cardTypeText != null)
        {
            cardTypeText.text = "Hero";
        }

        if (heroData == null)
        {
            ShowMissingCard();
            return;
        }

        if (cardStatsText != null)
        {
            cardStatsText.text =
                "ATK " + heroData.baseAttack
                + " / DEF " + heroData.baseDefense
                + " / HP " + heroData.baseHealth;
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = heroData.passiveDescription;
        }
    }

    private void ShowTacticCard(TacticCardData tacticData)
    {
        if (cardTypeText != null)
        {
            cardTypeText.text = "Tactic";
        }

        if (tacticData == null)
        {
            ShowMissingCard();
            return;
        }

        if (cardStatsText != null)
        {
            cardStatsText.text = "";
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = tacticData.effectDescription;
        }
    }

    private void ShowMissingCard()
    {
        if (cardNameText != null)
        {
            cardNameText.text = "Missing Card";
        }

        if (cardTypeText != null)
        {
            cardTypeText.text = "";
        }

        if (cardStatsText != null)
        {
            cardStatsText.text = "";
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = "";
        }
    }

    private void HandleClick()
    {
        if (owner != null)
        {
            owner.OnHandCardClicked(handIndex);
        }
    }
}