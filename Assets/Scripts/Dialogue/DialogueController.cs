using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;          // กล่องบทสนทนาหลัก เปิด/ปิดทั้งชุด
    public DialogueRenderer renderer; // ใช้แสดงบรรทัด
    public Button choiceA;
    public Button choiceB;
    public TMP_Text choiceAText;
    public TMP_Text choiceBText;
    public TMP_InputField inputField; // ช่องพิมพ์ประโยค (ซ่อนไว้ถ้าไม่ใช้)
    public Button submitBtn;          // ปุ่มยืนยันประโยค (ซ่อนไว้ถ้าไม่ใช้)
    public Button voiceBtn;           // ปุ่ม 🔊 เล่นเสียงบรรทัด

    [Header("Data")]
    public TextAsset dialogueJson;             // ไฟล์ JSON ของ Yi Yi
    public AudioClip[] voiceClips;             // เสียงประโยค (ถ้ามี)
    Dictionary<string, AudioClip> voiceMap;    // mapping id->clip (ถ้าตั้งชื่อไฟล์ตรง id)

    DialogueFile file;
    DialogueNode current;
    System.Action onEnd;      // callback เมื่อปิดบทสนทนา
    Dictionary<string, DialogueNode> index;

    void Awake()
    {
        panel.SetActive(false);
        inputField.gameObject.SetActive(false);
        submitBtn.gameObject.SetActive(false);
        MapVoices();
    }

    void MapVoices()
    {
        voiceMap = new Dictionary<string, AudioClip>();
        foreach (var c in voiceClips) voiceMap[c.name] = c; // ตั้งชื่อคลิป == id ของ node ก็ได้
    }

    public void StartDialogue(TextAsset json, System.Action onEndCallback = null)
    {
        file = JsonUtility.FromJson<DialogueFile>(json.text);
        index = file.lines.ToDictionary(n => n.id);
        onEnd = onEndCallback;

        ShowNode(file.lines[0]); // สมมติบรรทัดแรก คือ id แรก
        panel.SetActive(true);
        Time.timeScale = 0f; // หยุดเกมเบื้องหลังระหว่างคุย (ถ้าอยาก)
    }

    void ShowNode(DialogueNode node)
    {
        current = node;

        renderer.SetSpeaker(node.speaker);
        renderer.SetLines(node.text_en, node.text_th);

        // ปุ่มเสียง
        voiceBtn.onClick.RemoveAllListeners();
        voiceBtn.onClick.AddListener(() => {
            if (voiceMap.TryGetValue(node.id, out var clip)) renderer.PlayVoice(clip);
        });

        // ตัวเลือก
        if (node.choices != null && node.choices.Length > 0)
        {
            choiceA.gameObject.SetActive(true);
            choiceB.gameObject.SetActive(node.choices.Length > 1);

            choiceAText.text = node.choices[0].text_en;
            choiceA.onClick.RemoveAllListeners();
            choiceA.onClick.AddListener(() => Goto(node.choices[0].next));

            if (node.choices.Length > 1)
            {
                choiceBText.text = node.choices[1].text_en;
                choiceB.onClick.RemoveAllListeners();
                choiceB.onClick.AddListener(() => Goto(node.choices[1].next));
            }
        }
        else
        {
            choiceA.gameObject.SetActive(false);
            choiceB.gameObject.SetActive(false);
        }

        // จุดตรวจประโยค
        bool needInput = node.expect_input != null && node.expect_input.type == "sentence_check";
        inputField.gameObject.SetActive(needInput);
        submitBtn.gameObject.SetActive(needInput);

        if (needInput)
        {
            inputField.text = "";
            submitBtn.onClick.RemoveAllListeners();
            submitBtn.onClick.AddListener(() => {
                string user = inputField.text.Trim();
                string ans = node.expect_input.answer.Trim();
                if (IsSentenceMatch(user, ans))
                {
                    if (!string.IsNullOrEmpty(node.on_correct_next)) Goto(node.on_correct_next);
                }
                else
                {
                    // แสดง hint (แทนที่ข้อความไทยบรรทัดล่าง)
                    renderer.SetLines(node.text_en + $"\n\n<i><size=80%><color=#FFA500>Hint: {node.on_wrong_hint}</color></size></i>",
                                      "ลองเริ่มด้วย 'Do you know' + 'where + subject + be'");
                }
            });
        }

        if (node.end)
        {
            // node นี้คือจบแล้ว → ซ่อนปุ่มทั้งหมด, กด Space เพื่อปิด
            choiceA.gameObject.SetActive(false);
            choiceB.gameObject.SetActive(false);
        }
    }

    bool IsSentenceMatch(string a, string b)
    {
        // ตรวจแบบหยวน ๆ: ignore case + ช่องว่างซ้อน + จัด comma/จุดถาม
        string na = Normalize(a);
        string nb = Normalize(b);
        return na == nb;
    }

    string Normalize(string s)
    {
        s = s.ToLower().Trim();
        while (s.Contains("  ")) s = s.Replace("  ", " ");
        return s;
    }

    void Goto(string id)
    {
        if (!index.TryGetValue(id, out var next)) { EndDialogue(); return; }
        ShowNode(next);
    }

    public void EndDialogue()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        onEnd?.Invoke();
    }

    void Update()
    {
        if (panel.activeSelf && current != null && current.end && Input.GetKeyDown(KeyCode.Space))
        {
            EndDialogue();
        }
    }
}
