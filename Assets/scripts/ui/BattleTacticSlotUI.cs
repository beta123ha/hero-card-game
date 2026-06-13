using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleTacticSlotUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text slotIndexText;
    public TMP_Text tacticNameText;
    public TMP_Text tacticStateText;

    private int slotIndex;
    private bool isPlayerSlot;
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

    public void SetupClick(
        int slotIndex,
        bool isPlayerSlot,
        BattleUIController owner
    )
    {
        this.slotIndex = slotIndex;
        this.isPlayerSlot = isPlayerSlot;
        this.owner = owner;
    }

    public void ShowSlot(BattleTacticSlot slot, bool hideCardName)
    {
        if (slotIndexText != null)
        {
            slotIndexText.text = "Tactic " + (slotIndex + 1);
        }

        if (slot == null || slot.IsEmpty())
        {
            ShowEmpty();
            return;
        }

        if (tacticNameText != null)
        {
            if (slot.isFaceDown && hideCardName)
            {
                tacticNameText.text = "Face Down";
            }
            else
            {
                tacticNameText.text = slot.tacticCard.GetCardName();
            }
        }

        if (tacticStateText != null)
        {
            if (slot.isActive)
            {
                tacticStateText.text = "Active";
            }
            else if (slot.isFaceDown)
            {
                tacticStateText.text = "Face Down";
            }
            else
            {
                tacticStateText.text = "Ready";
            }
        }
    }

    private void ShowEmpty()
    {
        if (tacticNameText != null)
        {
            tacticNameText.text = "Empty";
        }

        if (tacticStateText != null)
        {
            tacticStateText.text = "";
        }
    }

    private void HandleClick()
    {
        if (owner != null)
        {
            owner.OnTacticSlotClicked(isPlayerSlot, slotIndex);
        }
    }
}