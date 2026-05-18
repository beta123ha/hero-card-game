using UnityEngine;

public static class AIStyleSelector
{
    public static AIPlayStyle PickRandomStyle()
    {
        int randomValue = Random.Range(0, 3);

        if (randomValue == 0)
        {
            return AIPlayStyle.Aggressive;
        }

        if (randomValue == 1)
        {
            return AIPlayStyle.Defensive;
        }

        return AIPlayStyle.Balanced;
    }
}