using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUIController : MonoBehaviour
{
    [Header("Main Info")]
    public TMP_Text playerHpText;
    public TMP_Text enemyHpText;
    public TMP_Text turnText;

    [Header("Card Counts")]
    public TMP_Text playerDeckCountText;
    public TMP_Text playerHandCountText;
    public TMP_Text playerGraveyardCountText;

    public TMP_Text enemyDeckCountText;
    public TMP_Text enemyHandCountText;
    public TMP_Text enemyGraveyardCountText;

    [Header("Board Slots")]
    public BattleBoardSlotUI[] playerBoardSlotUis;
    public BattleBoardSlotUI[] enemyBoardSlotUis;

    public void ShowBattleState(BattleState battleState)
    {
        if (battleState == null)
        {
            return;
        }

        ShowPlayerInfo(battleState);
        ShowBoard(battleState);
    }

    private void ShowPlayerInfo(BattleState battleState)
    {
        BattlePlayerState player = battleState.playerState;
        BattlePlayerState enemy = battleState.enemyState;

        if (playerHpText != null)
        {
            playerHpText.text = "Player HP: " + player.currentHealth;
        }

        if (enemyHpText != null)
        {
            enemyHpText.text = "Enemy HP: " + enemy.currentHealth;
        }

        if (turnText != null)
        {
            string currentTurnText = battleState.isPlayerTurn ? "Player" : "Enemy";
            turnText.text = "Turn " + battleState.turnNumber + ": " + currentTurnText;
        }

        if (playerDeckCountText != null)
        {
            playerDeckCountText.text = "Deck: " + player.deck.Count;
        }

        if (playerHandCountText != null)
        {
            playerHandCountText.text = "Hand: " + player.hand.Count;
        }

        if (playerGraveyardCountText != null)
        {
            playerGraveyardCountText.text = "Graveyard: " + player.graveyard.Count;
        }

        if (enemyDeckCountText != null)
        {
            enemyDeckCountText.text = "Deck: " + enemy.deck.Count;
        }

        if (enemyHandCountText != null)
        {
            enemyHandCountText.text = "Hand: " + enemy.hand.Count;
        }

        if (enemyGraveyardCountText != null)
        {
            enemyGraveyardCountText.text = "Graveyard: " + enemy.graveyard.Count;
        }
    }

    private void ShowBoard(BattleState battleState)
    {
        ShowBoardSlots(playerBoardSlotUis, battleState.playerState);
        ShowBoardSlots(enemyBoardSlotUis, battleState.enemyState);
    }

    private void ShowBoardSlots(BattleBoardSlotUI[] slotUis, BattlePlayerState playerState)
    {
        if (slotUis == null || playerState == null)
        {
            return;
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (slotUis[i] == null)
            {
                continue;
            }

            if (i < playerState.boardSlots.Count)
            {
                slotUis[i].ShowSlot(playerState.boardSlots[i]);
            }
            else
            {
                slotUis[i].ShowSlot(null);
            }
        }
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }
}