public class AITerrainSwapDecision
{
    public bool shouldSwap;
    public int firstIndex;
    public int secondIndex;
    public string reason;

    public AITerrainSwapDecision(
        bool shouldSwap,
        int firstIndex,
        int secondIndex,
        string reason
    )
    {
        this.shouldSwap = shouldSwap;
        this.firstIndex = firstIndex;
        this.secondIndex = secondIndex;
        this.reason = reason;
    }

    public static AITerrainSwapDecision NoSwap(string reason)
    {
        return new AITerrainSwapDecision(false, -1, -1, reason);
    }
}