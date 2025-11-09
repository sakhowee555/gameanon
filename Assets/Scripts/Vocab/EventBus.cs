using System;
using UnityEngine;

public static class EventBus
{
    // เรียกเมื่อมินิเกมคำศัพท์จบ (ผู้เล่นตอบถูก)
    public static Action OnVocabComplete;

    // เรียกเมื่อมอนสเตอร์ถูกกำจัด
    public static Action OnMonsterDefeated;

    public static Action<bool, int> OnBattleEnd;
    // เรียกเมื่อ XP เปลี่ยน
    public static Action<int, int> OnXPChanged;

    public static Action<string> OnQuestComplete;

    // เรียกเมื่อชื่อเควสเปลี่ยน
    public static Action<string> OnActiveQuestChanged;

    public static Action OnScrambleComplete;

    public static void EmitVocabComplete() => OnVocabComplete?.Invoke();
    public static void EmitScrambleComplete() => OnScrambleComplete?.Invoke();
    public static void EmitBattleEnd(bool win, int xp)
        => OnBattleEnd?.Invoke(win, xp);
    public static void EmitQuestComplete(string questId) => OnQuestComplete?.Invoke(questId);
}





