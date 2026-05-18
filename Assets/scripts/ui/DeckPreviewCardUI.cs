using TMPro;
using UnityEngine;

public class DeckPreviewCardUI : MonoBehaviour
{
    public TMP_Text labelText;

    public void SetupHero(HeroCardData hero)
    {
        if (hero == null)
        {
            labelText.text = "Missing Hero";
            return;
        }

        labelText.text =
            hero.heroName +
            "\nATK: " + hero.baseAttack +
            " | DEF: " + hero.baseDefense +
            " | HP: " + hero.baseHealth +
            "\nTags: " + BuildTagText(hero.tags);
    }

    public void SetupTactic(TacticCardData tactic)
    {
        if (tactic == null)
        {
            labelText.text = "Missing Tactic";
            return;
        }

        string typeText = tactic.isShared ? "Shared" : "Tag Required";

        labelText.text =
            tactic.tacticName +
            "\nType: " + typeText +
            "\nATK+: " + tactic.attackBonus +
            " | DEF+: " + tactic.defenseBonus +
            " | HP+: " + tactic.healthBonus +
            "\nRequires: " + BuildTagText(tactic.requiredTags);
    }

    private string BuildTagText(System.Collections.Generic.List<TagData> tags)
    {
        if (tags == null || tags.Count == 0)
        {
            return "None";
        }

        string result = "";

        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i] == null)
            {
                continue;
            }

            if (result != "")
            {
                result += ", ";
            }

            result += tags[i].tagName;
        }

        if (result == "")
        {
            return "None";
        }

        return result;
    }
}