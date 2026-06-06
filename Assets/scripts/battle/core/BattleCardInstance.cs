public class BattleCardInstance
{
    public BattleCardType cardType;

    public HeroCardData heroData;
    public TacticCardData tacticData;

    public BattleCardInstance(HeroCardData heroData)
    {
        cardType = BattleCardType.Hero;
        this.heroData = heroData;
        tacticData = null;
    }

    public BattleCardInstance(TacticCardData tacticData)
    {
        cardType = BattleCardType.Tactic;
        this.tacticData = tacticData;
        heroData = null;
    }

    public string GetCardName()
    {
        if (cardType == BattleCardType.Hero && heroData != null)
        {
            return heroData.heroName;
        }

        if (cardType == BattleCardType.Tactic && tacticData != null)
        {
            return tacticData.tacticName;
        }

        return "Missing Card";
    }
}