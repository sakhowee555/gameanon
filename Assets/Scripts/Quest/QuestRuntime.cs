using UnityEngine;
using System.Collections.Generic;

public class QuestRuntime : MonoBehaviour
{
    public static QuestRuntime I { get; private set; }

    [Header("Quest: Monkey Trouble")]
    public QuestOverallStatus overall = QuestOverallStatus.NotAccepted;

    // ขั้นตามที่วางไว้: talk_yiyi → mob_vocab → scramble → battle → report
    public Dictionary<string, QuestStepStatus> steps = new Dictionary<string, QuestStepStatus> {
        {"talk_yiyi",  QuestStepStatus.Locked},
        {"mob_vocab",  QuestStepStatus.Locked},
        {"scramble",   QuestStepStatus.Locked},
        {"battle",     QuestStepStatus.Locked},
        {"report",     QuestStepStatus.Locked}
    };

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Accept()
    {
        overall = QuestOverallStatus.InProgress;
        SetActive("talk_yiyi");
        GameEvents.OnActiveQuestChanged?.Invoke("Monkey Trouble");
    }

    public void SetActive(string id)
    {
        if (!steps.ContainsKey(id)) return;
        foreach (var k in new List<string>(steps.Keys))
            if (steps[k] == QuestStepStatus.Active) steps[k] = QuestStepStatus.Locked;
        steps[id] = QuestStepStatus.Active;
        Debug.Log($"Quest step active: {id}");
    }

    public void SetDone(string id)
    {
        if (!steps.ContainsKey(id)) return;
        steps[id] = QuestStepStatus.Done;
        Debug.Log($"Quest step done: {id}");
    }

    public bool IsAllDoneExceptReport()
    {
        return steps["talk_yiyi"] == QuestStepStatus.Done
            && steps["mob_vocab"] == QuestStepStatus.Done
            && steps["scramble"] == QuestStepStatus.Done
            && steps["battle"] == QuestStepStatus.Done;
    }

    public void Complete()
    {
        overall = QuestOverallStatus.Completed;
        GameEvents.OnActiveQuestChanged?.Invoke(""); // ไม่มี Active quest แล้ว
    }
}
