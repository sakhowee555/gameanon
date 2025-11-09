using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPopup : MonoBehaviour
{
    public static ResultPopup I { get; private set; }

    [Header("Root")]
    public GameObject panel;

    [Header("Fields")]
    public TMP_Text titleText;      // "Quest Complete!" หรือ "Battle Clear!"
    public TMP_Text xpText;         // "+100 XP"
    public TMP_Text tokensText;     // "Grammar +1, Vocab +1" (มีได้/ไม่มีได้)
    public TMP_Text checklistText;  // สรุปขั้น: talk / mob / scramble / battle / report
    public Button okButton;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        panel.SetActive(false);
    }

    public void Show(string title, int gainedXP, int gainedGrammar, int gainedVocab)
    {
        panel.SetActive(true);
        Time.timeScale = 0f;

        titleText.text = title;
        xpText.text = gainedXP > 0 ? $"+{gainedXP} XP" : "XP +0";

        string t = "";
        if (gainedGrammar > 0) t += $"Grammar +{gainedGrammar}  ";
        if (gainedVocab > 0) t += $"Vocab +{gainedVocab}";
        tokensText.text = string.IsNullOrEmpty(t) ? "—" : t;

        // ทำ checklist แบบอ่านง่าย
        var qr = QuestRuntime.I;
        string check =
            $"- Talk to Yi Yi: {(qr.steps["talk_yiyi"] == QuestStepStatus.Done ? "✓" : "○")}\n" +
            $"- Mob vocab: {(qr.steps["mob_vocab"] == QuestStepStatus.Done ? "✓" : "○")}\n" +
            $"- Scramble: {(qr.steps["scramble"] == QuestStepStatus.Done ? "✓" : "○")}\n" +
            $"- Defeat Monkey: {(qr.steps["battle"] == QuestStepStatus.Done ? "✓" : "○")}\n" +
            $"- Report to Yi Yi: {(qr.steps["report"] == QuestStepStatus.Done ? "✓" : "○")}";
        checklistText.text = check;

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() => {
            Close();
        });
    }

    public void Close()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
