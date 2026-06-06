public class BattleState
{
    public BattlePlayerState playerState;
    public BattlePlayerState enemyState;

    public bool isPlayerTurn;
    public int turnNumber;

    public BattleState(BattlePlayerState playerState, BattlePlayerState enemyState)
    {
        this.playerState = playerState;
        this.enemyState = enemyState;

        turnNumber = 1;
        isPlayerTurn = UnityEngine.Random.Range(0, 2) == 0;
    }

    public BattlePlayerState GetCurrentPlayer()
    {
        if (isPlayerTurn)
        {
            return playerState;
        }

        return enemyState;
    }

    public BattlePlayerState GetOpponentPlayer()
    {
        if (isPlayerTurn)
        {
            return enemyState;
        }

        return playerState;
    }

    public void SwitchTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        turnNumber++;
    }
}