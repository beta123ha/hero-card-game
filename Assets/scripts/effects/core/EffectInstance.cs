public class EffectInstance
{
    public EffectData effectData;
    public string sourceName;
    public int remainingTurns;
    public int stackCount;

    public EffectInstance(EffectData effectData, string sourceName)
    {
        this.effectData = effectData;
        this.sourceName = sourceName;

        stackCount = 1;

        if (effectData == null)
        {
            remainingTurns = 0;
            return;
        }

        remainingTurns = effectData.durationTurns;
    }

    public bool IsExpired()
    {
        if (effectData == null)
        {
            return true;
        }

        if (effectData.durationType == EffectDurationType.Permanent)
        {
            return false;
        }

        if (effectData.durationType == EffectDurationType.WhileConditionTrue)
        {
            return false;
        }

        if (effectData.durationType == EffectDurationType.Instant)
        {
            return true;
        }

        return remainingTurns <= 0;
    }

    public void TickTurn()
    {
        if (effectData == null)
        {
            return;
        }

        if (effectData.durationType == EffectDurationType.UntilEndOfTurn)
        {
            remainingTurns--;
        }
    }

    public void RefreshDuration()
    {
        if (effectData == null)
        {
            return;
        }

        remainingTurns = effectData.durationTurns;
    }

    public void AddStack()
    {
        if (effectData == null)
        {
            return;
        }

        if (effectData.stackingType == EffectStackingType.Stackable)
        {
            stackCount++;
            RefreshDuration();
            return;
        }

        if (effectData.stackingType == EffectStackingType.StackableWithLimit)
        {
            if (stackCount < effectData.maxStacks)
            {
                stackCount++;
            }

            RefreshDuration();
        }
    }
}