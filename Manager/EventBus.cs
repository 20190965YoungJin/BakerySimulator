using System;

public static class EventBus
{
    public static event Action OnMoneyThresholdReached;
    private static bool hasTriggeredMoneyEvent = false;

    public static void RaiseMoneyThresholdEvent()
    {
        if (hasTriggeredMoneyEvent) return;

        hasTriggeredMoneyEvent = true;
        OnMoneyThresholdReached?.Invoke();
    }



    public static event Action OnUnlockTable;
    private static bool hasUnlockTableEvent = false;

    public static void UnlockTableEvent()
    {
        if (hasUnlockTableEvent) return;

        hasUnlockTableEvent = true;
        OnUnlockTable?.Invoke();
    }
}
