using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerrainSlotButtonUI : MonoBehaviour
{
    public TMP_Text labelText;

    private int slotIndex;
    private bool isPlayerSlot;
    private TerrainSetupController controller;

    private void Awake()
    {
        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void Setup(
        int index,
        TerrainData terrain,
        TerrainSetupController terrainController,
        bool belongsToPlayer
    )
    {
        slotIndex = index;
        controller = terrainController;
        isPlayerSlot = belongsToPlayer;

        Refresh(index, terrain, false, belongsToPlayer);
    }

    public void Refresh(
        int index,
        TerrainData terrain,
        bool isSelected,
        bool canClick
    )
    {
        slotIndex = index;

        string terrainName = "Empty";

        if (terrain != null)
        {
            terrainName = terrain.terrainName;
        }

        string text = "Slot " + (slotIndex + 1) + "\n" + terrainName;

        if (isSelected)
        {
            text += "\n[selected]";
        }

        if (!canClick)
        {
            text += "\n(enemy)";
        }

        labelText.text = text;

        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.interactable = canClick;
        }
    }

    private void OnClick()
    {
        if (!isPlayerSlot)
        {
            return;
        }

        if (controller == null)
        {
            return;
        }

        controller.SelectSlot(slotIndex);
    }
}