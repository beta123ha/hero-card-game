public static class AIStyleController
{
    public static AIPlayStyle DecideCurrentStyle(
        AIPlayStyle baseStyle,
        AIPlayStyle currentStyle,
        AIDifficulty difficulty,
        AISituationSnapshot situation
    )
    {
        if (situation == null)
        {
            return baseStyle;
        }

        if (difficulty == AIDifficulty.Easy)
        {
            return DecideForEasy(baseStyle, currentStyle, situation);
        }

        if (difficulty == AIDifficulty.Normal)
        {
            return DecideForNormal(baseStyle, currentStyle, situation);
        }

        return DecideForHard(baseStyle, currentStyle, situation);
    }

    private static AIPlayStyle DecideForEasy(
        AIPlayStyle baseStyle,
        AIPlayStyle currentStyle,
        AISituationSnapshot situation
    )
    {
        return baseStyle;
    }

    private static AIPlayStyle DecideForNormal(
        AIPlayStyle baseStyle,
        AIPlayStyle currentStyle,
        AISituationSnapshot situation
    )
    {
        if (situation.botCanFinishPlayer)
        {
            return AIPlayStyle.Aggressive;
        }

        if (situation.botHealth <= 35 && situation.playerHealth > situation.botHealth)
        {
            return AIPlayStyle.Defensive;
        }

        if (situation.playerHealth <= 30)
        {
            return AIPlayStyle.Aggressive;
        }

        return baseStyle;
    }

    private static AIPlayStyle DecideForHard(
        AIPlayStyle baseStyle,
        AIPlayStyle currentStyle,
        AISituationSnapshot situation
    )
    {
        if (situation.botCanFinishPlayer)
        {
            return AIPlayStyle.Aggressive;
        }

        if (situation.playerCanThreatenDirectDamage && situation.botHealth <= 50)
        {
            return AIPlayStyle.Defensive;
        }

        if (situation.botHealth <= 30)
        {
            return AIPlayStyle.Defensive;
        }

        if (situation.playerHealth <= 40)
        {
            return AIPlayStyle.Aggressive;
        }

        if (situation.botHasGoodCombo)
        {
            return baseStyle;
        }

        return AIPlayStyle.Balanced;
    }
}