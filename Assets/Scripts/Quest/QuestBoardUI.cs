using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBoardUI : MonoBehaviour
{
    public Image monkeyQuestIcon;   // ไอคอนเควสลิง (เช่น วงกลมหรือรูปกล่อง)
    public TMP_Text monkeyQuestLabel;
    public Sprite iconPending;      // ไอคอนปกติ
    public Sprite iconDone;         // ไอคอน ✓

    void OnEnable()
    {
        Refresh();
        EventBus.OnQuestComplete += OnQuestComplete;
    }
    void OnDisable()
    {
        EventBus.OnQuestComplete -= OnQuestComplete;
    }

    public void Refresh()
    {
        bool done = QuestRuntime.I.overall == QuestOverallStatus.Completed;
        monkeyQuestIcon.sprite = done ? iconDone : iconPending;
        monkeyQuestLabel.text = done ? "Monkey Trouble  ✓" : "Monkey Trouble";
    }

    void OnQuestComplete(string questId)
    {
        if (questId == "q02_monkey_trouble")
        {
            Refresh();
        }
    }
}
