public class AISituationSnapshot
{
    public int botHealth;
    public int playerHealth;

    public bool botCanFinishPlayer;
    public bool playerCanThreatenDirectDamage;
    public bool botHasGoodCombo;

    public AISituationSnapshot(
        int botHealth,
        int playerHealth,
        bool botCanFinishPlayer,
        bool playerCanThreatenDirectDamage,
        bool botHasGoodCombo
    )
    {
        this.botHealth = botHealth;
        this.playerHealth = playerHealth;
        this.botCanFinishPlayer = botCanFinishPlayer;
        this.playerCanThreatenDirectDamage = playerCanThreatenDirectDamage;
        this.botHasGoodCombo = botHasGoodCombo;
    }
}