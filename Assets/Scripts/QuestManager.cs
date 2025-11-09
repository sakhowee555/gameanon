using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager I { get; private set; }

    public int currentXP = 0;
    public int maxXP = 100;
    public int grammarTokens = 0;
    public int vocabTokens = 0;

    void Awake() { if (I != null && I != this) { Destroy(gameObject); return; } I = this; DontDestroyOnLoad(gameObject); }

    void OnEnable()
    {
        EventBus.OnVocabComplete += OnVocabDone;
        EventBus.OnScrambleComplete += OnScrambleDone;
        EventBus.OnBattleEnd += OnBattleEnd;
    }
    void OnDisable()
    {
        EventBus.OnVocabComplete -= OnVocabDone;
        EventBus.OnScrambleComplete -= OnScrambleDone;
        EventBus.OnBattleEnd -= OnBattleEnd;
    }

    void Start()
    {
        GameEvents.OnXPChanged?.Invoke(currentXP, maxXP);
    }

    public void AddXP(int amount)
    {
        currentXP = Mathf.Clamp(currentXP + amount, 0, maxXP);
        GameEvents.OnXPChanged?.Invoke(currentXP, maxXP);
    }

    // --- Quest step updates ---
    void OnVocabDone()
    {
        vocabTokens += 1;
        QuestRuntime.I.SetDone("mob_vocab");
        QuestRuntime.I.SetActive("scramble");
        Debug.Log("Vocab token +1");
    }

    void OnScrambleDone()
    {
        grammarTokens += 1;
        QuestRuntime.I.SetDone("scramble");
        QuestRuntime.I.SetActive("battle");
        Debug.Log("Grammar token +1");
    }

    void OnBattleEnd(bool win, int xpReward)
    {
        if (!win)
        {
            Debug.Log("Battle lost. Retry available.");
            return;
        }

        // ให้ XP
        if (xpReward > 0) AddXP(xpReward);

        // อัปเดตเควส
        QuestRuntime.I.SetDone("battle");

        // โชว์ Result หลังชนะบอส (สรุปเฉพาะที่เกี่ยวกับ battle + token ที่ได้มาก่อนหน้า)
        ResultPopup.I.Show(
            title: "Battle Clear!",
            gainedXP: xpReward,
            gainedGrammar: 0,    // token grammar ได้มาจาก Scramble แล้ว
            gainedVocab: 0       // token vocab ได้จาก mob แล้ว
        );

        // เปิด step report ต่อ (จะทำหรือข้ามก็ได้)
        QuestRuntime.I.SetActive("report");
    }

    // เรียกตอนคอนเฟิร์มรายงานกับ Yi Yi (หลังกลับไปคุย)
    public void ReportToYiYiAndComplete()
    {
        if (!QuestRuntime.I.IsAllDoneExceptReport()) return;

        QuestRuntime.I.SetDone("report");
        QuestRuntime.I.Complete();

        // แสดง Result ว่าเควสจบจริง พร้อม checklist
        ResultPopup.I.Show("Quest Complete!", 0, 0, 0);

        // แจ้งบอร์ดให้รีเฟรชเป็น ✓
        EventBus.EmitQuestComplete("q02_monkey_trouble");
    }
}
