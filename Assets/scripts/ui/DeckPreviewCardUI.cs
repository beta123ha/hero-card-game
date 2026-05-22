using System.Collections.Generic;
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
            "\nTags: " + BuildTagText(hero.tags) +
            "\nPassive: " + BuildEffectText(hero.passiveEffects);
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
            "\nRequires: " + BuildTagText(tactic.requiredTags) +
            "\nEffects: " + BuildEffectText(tactic.tacticEffects);
    }

    private string BuildTagText(List<TagData> tags)
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

    private string BuildEffectText(List<EffectData> effects)
    {
        if (effects == null || effects.Count == 0)
        {
            return "None";
        }

        string result = "";

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i] == null)
            {
                continue;
            }

            if (result != "")
            {
                result += "; ";
            }

            result += BuildSingleEffectText(effects[i]);
        }

        if (result == "")
        {
            return "None";
        }

        return result;
    }

    private string BuildSingleEffectText(EffectData effect)
    {
        StatModifierEffectData statEffect = effect as StatModifierEffectData;

        if (statEffect != null)
        {
            string sign = statEffect.value >= 0 ? "+" : "";

            return effect.effectName +
                " (" +
                effect.targetType +
                " " +
                sign +
                statEffect.value +
                " " +
                statEffect.statType +
                ", " +
                effect.durationType +
                ")";
        }

        if (!string.IsNullOrEmpty(effect.description))
        {
            return effect.effectName + " (" + effect.description + ")";
        }

        return effect.effectName + " (" + effect.targetType + ", " + effect.durationType + ")";
    }
}