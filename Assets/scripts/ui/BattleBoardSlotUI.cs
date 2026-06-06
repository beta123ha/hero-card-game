using TMPro;
using UnityEngine;

public class BattleBoardSlotUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text slotIndexText;
    public TMP_Text terrainText;
    public TMP_Text heroNameText;
    public TMP_Text heroStatsText;
    public TMP_Text heroHealthText;

    public void ShowSlot(BattleBoardSlot slot)
    {
        if (slot == null)
        {
            Clear();
            return;
        }

        if (slotIndexText != null)
        {
            slotIndexText.text = "Slot " + (slot.slotIndex + 1);
        }

        if (terrainText != null)
        {
            if (slot.terrainData != null)
            {
                terrainText.text = slot.terrainData.name;
            }
            else
            {
                terrainText.text = "No Terrain";
            }
        }

        if (slot.heroInstance == null)
        {
            ShowEmptyHero();
            return;
        }

        ShowHero(slot.heroInstance);
    }

    private void ShowHero(BattleHeroInstance heroInstance)
    {
        if (heroInstance == null || heroInstance.heroData == null)
        {
            ShowEmptyHero();
            return;
        }

        if (heroNameText != null)
        {
            heroNameText.text = heroInstance.heroData.heroName;
        }

        if (heroStatsText != null)
        {
            heroStatsText.text =
                "ATK " + heroInstance.GetCurrentAttack()
                + " / DEF " + heroInstance.GetCurrentDefense();
        }

        if (heroHealthText != null)
        {
            heroHealthText.text =
                "HP " + heroInstance.currentHealth
                + " / " + heroInstance.GetCurrentMaxHealth();
        }
    }

    private void ShowEmptyHero()
    {
        if (heroNameText != null)
        {
            heroNameText.text = "Empty";
        }

        if (heroStatsText != null)
        {
            heroStatsText.text = "";
        }

        if (heroHealthText != null)
        {
            heroHealthText.text = "";
        }
    }

    private void Clear()
    {
        if (slotIndexText != null)
        {
            slotIndexText.text = "";
        }

        if (terrainText != null)
        {
            terrainText.text = "";
        }

        ShowEmptyHero();
    }
}