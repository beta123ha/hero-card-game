using UnityEngine;

public class BattleInitializer : MonoBehaviour
{
    public BattleUIController battleUIController;

    public BattleState BattleState { get; private set; }

    private void Start()
    {
        InitializeBattle();
    }

    private void InitializeBattle()
    {
        if (GameSession.Instance == null)
        {
            Debug.LogError("Missing GameSession. Please start battle from the normal scene flow.");
            return;
        }

        BattlePlayerState playerState = new BattlePlayerState(
            "Player",
            false,
            GameSession.Instance.selectedHeroes,
            GameSession.Instance.selectedTactics,
            GameSession.Instance.playerTerrainOrder
        );

        BattlePlayerState enemyState = new BattlePlayerState(
            "Enemy",
            true,
            GameSession.Instance.enemySelectedHeroes,
            GameSession.Instance.enemySelectedTactics,
            GameSession.Instance.enemyTerrainOrder
        );

        playerState.DrawCards(5);
        enemyState.DrawCards(5);

        BattleState = new BattleState(playerState, enemyState);

        Debug.Log("Battle initialized.");
        Debug.Log("Player deck: " + playerState.deck.Count);
        Debug.Log("Player hand: " + playerState.hand.Count);
        Debug.Log("Enemy deck: " + enemyState.deck.Count);
        Debug.Log("Enemy hand: " + enemyState.hand.Count);

        if (battleUIController != null)
        {
            battleUIController.ShowBattleState(BattleState);
        }
    }
}