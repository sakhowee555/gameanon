using UnityEngine;
using UnityEngine.UI;

public class VocabMiniGameController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject vocabPanel;      // Canvas panel ของมินิเกม
    public Text questionText;          // ข้อความถาม
    public Button[] optionButtons;     // ปุ่มคำศัพท์ 4 ปุ่ม
    public AudioSource correctSound;   // เสียงถูก
    public AudioSource wrongSound;     // เสียงผิด

    private string correctAnswer = "banana";  // คำตอบถูก (เดโม)

    void Start()
    {
        vocabPanel.SetActive(false); // ซ่อนก่อน
    }

    public void StartMiniGame()
    {
        vocabPanel.SetActive(true);
        questionText.text = "Which one is a fruit?";

        string[] options = { "wound", "bandage", "banana", "slip" };

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i; // สำคัญ ต้อง copy index
            optionButtons[i].GetComponentInChildren<Text>().text = options[i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => CheckAnswer(options[idx]));
        }
    }

    void CheckAnswer(string selected)
    {
        if (selected == correctAnswer)
        {
            Debug.Log("✅ Correct: " + selected);
            correctSound?.Play();
            vocabPanel.SetActive(false);

            // แจ้งว่าเล่นมินิเกมจบแล้ว
            EventBus.OnVocabComplete?.Invoke();
        }
        else
        {
            Debug.Log("❌ Wrong: " + selected);
            wrongSound?.Play();
        }
    }
}
