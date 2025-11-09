using System;

public static class GameEvents
{
    public static Action<int, int> OnXPChanged;          // (current, max)
    public static Action<string> OnActiveQuestChanged;  // title
}
