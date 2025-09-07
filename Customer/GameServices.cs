using System.Collections.Generic;
using UnityEngine;

public static class GameServices
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void SilenceAssertions()
    {
        Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
    }


    public static PaymentManager PaymentManager { get; private set; }
    public static Stall Stall { get; private set; }
    public static StackBread StackBread { get; private set; }
    public static TableSeatController tableSeatController { get; private set; }
    public static Transform shelfPosition { get; private set; }
    public static Transform takeoutPosition { get; private set; }
    public static Transform counterPosition { get; private set; }

    public static void RegisterServices(PaymentManager payment, Stall stall, StackBread stackBread, TableSeatController tableSeat, Transform shelfPos, Transform takeoutPos, Transform counterPos)
    {
        PaymentManager = payment;
        Stall = stall;
        StackBread = stackBread;
        tableSeatController = tableSeat;
        shelfPosition = shelfPos;
        takeoutPosition = takeoutPos;
        counterPosition = counterPos;   
    }
}
