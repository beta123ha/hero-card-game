public class BattleBoardSlot
{
    public int slotIndex;
    public TerrainData terrainData;
    public BattleHeroInstance heroInstance;

    public BattleBoardSlot(int slotIndex, TerrainData terrainData)
    {
        this.slotIndex = slotIndex;
        this.terrainData = terrainData;
        heroInstance = null;
    }

    public bool IsEmpty()
    {
        return heroInstance == null;
    }

    public void PlaceHero(BattleHeroInstance heroInstance)
    {
        this.heroInstance = heroInstance;
    }

    public BattleHeroInstance RemoveHero()
    {
        BattleHeroInstance removedHero = heroInstance;
        heroInstance = null;
        return removedHero;
    }
}