using System;
using UnityEngine;

public static class EventBus
{
    // เรียกเมื่อมินิเกมคำศัพท์จบ (ผู้เล่นตอบถูก)
    public static Action OnVocabComplete;

    // เรียกเมื่อมอนสเตอร์ถูกกำจัด
    public static Action OnMonsterDefeated;

    // เรียกเมื่อ XP เปลี่ยน
    public static Action<int, int> OnXPChanged;

    // เรียกเมื่อชื่อเควสเปลี่ยน
    public static Action<string> OnActiveQuestChanged;
}
