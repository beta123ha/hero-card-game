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

    [Header("Tactic Slots")]
    public BattleTacticSlotUI[] playerTacticSlotUis;
    public BattleTacticSlotUI[] enemyTacticSlotUis;

    [Header("Player Hand")]
    public Transform playerHandContent;
    public BattleHandCardUI playerHandCardPrefab;

    private BattleState currentBattleState;
    private int selectedHandCardIndex = -1;

    private void Start()
    {
        SetupBoardSlotClicks();
        SetupTacticSlotClicks();
    }

    public void ShowBattleState(BattleState battleState)
    {
        if (battleState == null)
        {
            return;
        }

        currentBattleState = battleState;

        SetupBoardSlotClicks();
        SetupTacticSlotClicks();

        ShowPlayerInfo(battleState);
        ShowBoard(battleState);
        ShowTacticSlots(battleState);
        ShowPlayerHand(battleState.playerState);
    }

    private void SetupBoardSlotClicks()
    {
        SetupBoardSlotClicks(playerBoardSlotUis, true);
        SetupBoardSlotClicks(enemyBoardSlotUis, false);
    }

    private void SetupBoardSlotClicks(
        BattleBoardSlotUI[] slotUis,
        bool isPlayerBoard
    )
    {
        if (slotUis == null)
        {
            return;
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (slotUis[i] != null)
            {
                slotUis[i].SetupClick(i, isPlayerBoard, this);
            }
        }
    }

    private void SetupTacticSlotClicks()
    {
        SetupTacticSlotClicks(playerTacticSlotUis, true);
        SetupTacticSlotClicks(enemyTacticSlotUis, false);
    }

    private void SetupTacticSlotClicks(
        BattleTacticSlotUI[] slotUis,
        bool isPlayerSlot
    )
    {
        if (slotUis == null)
        {
            return;
        }

        for (int i = 0; i < slotUis.Length; i++)
        {
            if (slotUis[i] != null)
            {
                slotUis[i].SetupClick(i, isPlayerSlot, this);
            }
        }
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

    private void ShowBoardSlots(
        BattleBoardSlotUI[] slotUis,
        BattlePlayerState playerState
    )
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

    private void ShowTacticSlots(BattleState battleState)
    {
        ShowTacticSlots(playerTacticSlotUis, battleState.playerState, false);
        ShowTacticSlots(enemyTacticSlotUis, battleState.enemyState, true);
    }

    private void ShowTacticSlots(
        BattleTacticSlotUI[] slotUis,
        BattlePlayerState playerState,
        bool hideCardName
    )
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

            if (i < playerState.tacticSlots.Count)
            {
                slotUis[i].ShowSlot(playerState.tacticSlots[i], hideCardName);
            }
            else
            {
                slotUis[i].ShowSlot(null, hideCardName);
            }
        }
    }

    private void ShowPlayerHand(BattlePlayerState playerState)
    {
        if (playerHandContent == null || playerHandCardPrefab == null)
        {
            return;
        }

        for (int i = playerHandContent.childCount - 1; i >= 0; i--)
        {
            Destroy(playerHandContent.GetChild(i).gameObject);
        }

        if (playerState == null)
        {
            return;
        }

        for (int i = 0; i < playerState.hand.Count; i++)
        {
            BattleHandCardUI cardUI = Instantiate(
                playerHandCardPrefab,
                playerHandContent
            );

            cardUI.Setup(playerState.hand[i], i, this);
        }
    }

    public void OnHandCardClicked(int handIndex)
    {
        if (currentBattleState == null)
        {
            return;
        }

        if (!currentBattleState.isPlayerTurn)
        {
            Debug.Log("It is not player turn.");
            return;
        }

        BattlePlayerState player = currentBattleState.playerState;

        if (handIndex < 0 || handIndex >= player.hand.Count)
        {
            return;
        }

        selectedHandCardIndex = handIndex;

        BattleCardInstance selectedCard = player.hand[handIndex];
        Debug.Log("Selected card: " + selectedCard.GetCardName());
    }

    public void OnBoardSlotClicked(bool isPlayerBoard, int slotIndex)
    {
        if (currentBattleState == null)
        {
            return;
        }

        if (!isPlayerBoard)
        {
            Debug.Log("Cannot deploy hero to enemy board.");
            return;
        }

        if (selectedHandCardIndex < 0)
        {
            Debug.Log("Select a card first.");
            return;
        }

        DeploySelectedHeroToSlot(slotIndex);
    }

    public void OnTacticSlotClicked(bool isPlayerSlot, int slotIndex)
    {
        if (currentBattleState == null)
        {
            return;
        }

        if (!isPlayerSlot)
        {
            Debug.Log("Cannot place tactic to enemy tactic slot.");
            return;
        }

        if (selectedHandCardIndex < 0)
        {
            Debug.Log("Select a tactic card first.");
            return;
        }

        PlaceSelectedTacticToSlot(slotIndex);
    }

    private void DeploySelectedHeroToSlot(int slotIndex)
    {
        if (currentBattleState == null)
        {
            return;
        }

        if (!currentBattleState.isPlayerTurn)
        {
            Debug.Log("It is not player turn.");
            return;
        }

        BattlePlayerState player = currentBattleState.playerState;

        if (player.hasUsedHeroActionThisTurn)
        {
            Debug.Log("Hero action already used this turn.");
            return;
        }

        if (slotIndex < 0 || slotIndex >= player.boardSlots.Count)
        {
            return;
        }

        BattleBoardSlot targetSlot = player.boardSlots[slotIndex];

        if (!targetSlot.IsEmpty())
        {
            Debug.Log("Target slot is not empty.");
            return;
        }

        if (selectedHandCardIndex < 0 || selectedHandCardIndex >= player.hand.Count)
        {
            selectedHandCardIndex = -1;
            return;
        }

        BattleCardInstance selectedCard = player.hand[selectedHandCardIndex];

        if (selectedCard.cardType != BattleCardType.Hero || selectedCard.heroData == null)
        {
            Debug.Log("Selected card is not a valid hero.");
            return;
        }

        BattleHeroInstance heroInstance = new BattleHeroInstance(selectedCard.heroData);

        targetSlot.PlaceHero(heroInstance);
        player.hand.RemoveAt(selectedHandCardIndex);

        player.hasUsedHeroActionThisTurn = true;
        selectedHandCardIndex = -1;

        Debug.Log("Deployed hero: " + selectedCard.GetCardName());

        ShowBattleState(currentBattleState);
    }

    private void PlaceSelectedTacticToSlot(int slotIndex)
    {
        if (currentBattleState == null)
        {
            return;
        }

        if (!currentBattleState.isPlayerTurn)
        {
            Debug.Log("It is not player turn.");
            return;
        }

        BattlePlayerState player = currentBattleState.playerState;

        if (player.hasUsedTacticActionThisTurn)
        {
            Debug.Log("Tactic action already used this turn.");
            return;
        }

        if (slotIndex < 0 || slotIndex >= player.tacticSlots.Count)
        {
            return;
        }

        BattleTacticSlot targetSlot = player.tacticSlots[slotIndex];

        if (!targetSlot.IsEmpty())
        {
            Debug.Log("Target tactic slot is not empty.");
            return;
        }

        if (selectedHandCardIndex < 0 || selectedHandCardIndex >= player.hand.Count)
        {
            selectedHandCardIndex = -1;
            return;
        }

        BattleCardInstance selectedCard = player.hand[selectedHandCardIndex];

        if (selectedCard.cardType != BattleCardType.Tactic || selectedCard.tacticData == null)
        {
            Debug.Log("Selected card is not a valid tactic.");
            return;
        }

        targetSlot.PlaceTactic(selectedCard);
        player.hand.RemoveAt(selectedHandCardIndex);

        player.hasUsedTacticActionThisTurn = true;
        selectedHandCardIndex = -1;

        Debug.Log("Placed tactic face down: " + selectedCard.GetCardName());

        ShowBattleState(currentBattleState);
    }

    public void OnEndTurnClicked()
    {
        if (currentBattleState == null)
        {
            return;
        }

        BattlePlayerState oldPlayer = currentBattleState.GetCurrentPlayer();
        MarkTacticsAsOld(oldPlayer);

        currentBattleState.SwitchTurn();

        BattlePlayerState currentPlayer = currentBattleState.GetCurrentPlayer();

        currentPlayer.hasUsedHeroActionThisTurn = false;
        currentPlayer.hasUsedTacticActionThisTurn = false;

        currentPlayer.DrawCard();

        selectedHandCardIndex = -1;

        ShowBattleState(currentBattleState);
    }

    private void MarkTacticsAsOld(BattlePlayerState player)
    {
        if (player == null || player.tacticSlots == null)
        {
            return;
        }

        foreach (BattleTacticSlot slot in player.tacticSlots)
        {
            if (slot != null)
            {
                slot.wasPlacedThisTurn = false;
            }
        }
    }

    public void BackToEnemySetup()
    {
        SceneManager.LoadScene("enemy_setup");
    }

    public void backToMenu()
    {
        BackToEnemySetup();
    }
}