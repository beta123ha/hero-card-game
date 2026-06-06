public class BattleTacticSlot
{
    public int slotIndex;
    public BattleCardInstance tacticCard;

    public bool isFaceDown;
    public bool wasPlacedThisTurn;
    public bool isActive;

    public BattleTacticSlot(int slotIndex)
    {
        this.slotIndex = slotIndex;
        tacticCard = null;

        isFaceDown = false;
        wasPlacedThisTurn = false;
        isActive = false;
    }

    public bool IsEmpty()
    {
        return tacticCard == null;
    }

    public void PlaceTactic(BattleCardInstance card)
    {
        tacticCard = card;
        isFaceDown = true;
        wasPlacedThisTurn = true;
        isActive = false;
    }

    public void Activate()
    {
        isFaceDown = false;
        isActive = true;
    }

    public void Clear()
    {
        tacticCard = null;
        isFaceDown = false;
        wasPlacedThisTurn = false;
        isActive = false;
    }
}