using System.Collections.Generic;

public class BattleHeroInstance
{
    public HeroCardData heroData;

    public int currentHealth;

    public bool canAttack;
    public bool wasDeployedOrMovedThisTurn;

    public List<EffectInstance> activeEffects = new List<EffectInstance>();

    public BattleHeroInstance(HeroCardData heroData)
    {
        this.heroData = heroData;

        if (heroData != null)
        {
            currentHealth = heroData.baseHealth;
        }
        else
        {
            currentHealth = 1;
        }

        canAttack = false;
        wasDeployedOrMovedThisTurn = true;
    }

    public int GetCurrentAttack()
    {
        if (heroData == null)
        {
            return 1;
        }

        return StatCalculator.CalculateAttack(heroData.baseAttack, activeEffects);
    }

    public int GetCurrentDefense()
    {
        if (heroData == null)
        {
            return 1;
        }

        return StatCalculator.CalculateDefense(heroData.baseDefense, activeEffects);
    }

    public int GetCurrentMaxHealth()
    {
        if (heroData == null)
        {
            return 1;
        }

        return StatCalculator.CalculateHealth(heroData.baseHealth, activeEffects);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}