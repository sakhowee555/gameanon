using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager I { get; private set; }

    [Header("Demo XP")]
    public int currentXP = 0;
    public int maxXP = 100;

    [Header("Active Quest")]
    public string activeQuestTitle = "";

    [Header("Token Progress")]
    public int vocabTokens = 0;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        EventBus.OnVocabComplete += OnVocabPassed;
        EventBus.OnMonsterDefeated += OnMonsterDefeated;
    }
    void OnDisable()
    {
        EventBus.OnVocabComplete -= OnVocabPassed;
        EventBus.OnMonsterDefeated -= OnMonsterDefeated;
    }

    void Start()
    {
        EventBus.OnXPChanged?.Invoke(currentXP, maxXP);
        EventBus.OnActiveQuestChanged?.Invoke(activeQuestTitle);
    }

    // ✅ เมื่อมินิเกมคำศัพท์จบ
    void OnVocabPassed()
    {
        vocabTokens += 1;
        Debug.Log("✅ ได้ token จาก mob คำศัพท์ +1");
    }

    // ✅ เมื่อมอนสเตอร์ตาย
    void OnMonsterDefeated()
    {
        AddXP(25); // ให้ XP 25 ต่อมอน
        Debug.Log("✅ Monster defeated! XP added.");
    }

    public void AddXP(int amount)
    {
        currentXP = Mathf.Clamp(currentXP + amount, 0, maxXP);
        EventBus.OnXPChanged?.Invoke(currentXP, maxXP);
    }

    public void SetActiveQuest(string title)
    {
        activeQuestTitle = title;
        EventBus.OnActiveQuestChanged?.Invoke(activeQuestTitle);
    }
}
