using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ScrambleController : MonoBehaviour
{
    [Header("Root")]
    public GameObject panel;

    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text instructionText;
    public RectTransform tilesContainer; // parent วางปุ่มคำ
    public TMP_Text outputText;
    public TMP_Text hintText;
    public TMP_Text timerText;           // อาจว่างได้ถ้าไม่ใช้
    public Button btnSubmit;
    public Button btnClear;
    public Button btnShuffle;
    public Button btnClose;

    [Header("Tile Prefab")]
    public Button tilePrefab;  // ปุ่ม TMP ที่มี Text ลูก

    private ScrambleDef def;
    private List<Button> tileButtons = new();
    private List<string> chosen = new();
    private bool passed;
    private int timeLeft;
    private bool running;

    void Awake()
    {
        panel.SetActive(false);
        btnClose.gameObject.SetActive(false);
    }

    public void OpenWithJson(TextAsset json)
    {
        def = JsonUtility.FromJson<ScrambleDef>(json.text);
        SetupUI();
        panel.SetActive(true);
        Time.timeScale = 0f; // หยุดโลกด้านหลัง
    }

    void SetupUI()
    {
        titleText.text = def.title;
        instructionText.text = def.instruction_en;
        hintText.text = "";
        outputText.text = "";
        btnClose.gameObject.SetActive(false);

        // เคลียร์ปุ่มเก่า
        foreach (var b in tileButtons) if (b) Destroy(b.gameObject);
        tileButtons.Clear();
        chosen.Clear();

        // สุ่มเรียง tiles เริ่มต้น
        var shuffled = def.tiles.OrderBy(_ => Random.value).ToList();
        foreach (var w in shuffled)
        {
            var btn = Instantiate(tilePrefab, tilesContainer);
            var t = btn.GetComponentInChildren<TMP_Text>();
            t.text = w;
            btn.onClick.AddListener(() => OnTileClicked(btn, w));
            tileButtons.Add(btn);
        }

        // ปุ่มควบคุม
        btnClear.onClick.RemoveAllListeners();
        btnClear.onClick.AddListener(ClearOutput);

        btnShuffle.onClick.RemoveAllListeners();
        btnShuffle.onClick.AddListener(ShuffleTiles);

        btnSubmit.onClick.RemoveAllListeners();
        btnSubmit.onClick.AddListener(Submit);

        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(ClosePanel);

        // Timer
        if (def.time_limit_sec > 0 && timerText != null)
        {
            timeLeft = def.time_limit_sec;
            running = true;
            timerText.gameObject.SetActive(true);
            StartCoroutine(TimerTick());
        }
        else if (timerText != null) timerText.gameObject.SetActive(false);
    }

    System.Collections.IEnumerator TimerTick()
    {
        while (running && timeLeft >= 0)
        {
            timerText.text = $"{timeLeft}s";
            yield return new WaitForSecondsRealtime(1f);
            timeLeft--;
        }
        if (running && timeLeft < 0) // หมดเวลา
        {
            running = false;
            hintText.text = "Time's up! Click Shuffle and try again.";
            btnShuffle.Select();
        }
    }

    void OnTileClicked(Button btn, string word)
    {
        if (passed) return;
        // เพิ่มคำไปที่ output
        chosen.Add(word);
        RefreshOutput();

        // disable ปุ่มที่เลือกแล้ว (ยกเลิกง่าย ๆ ด้วยการล้างทั้งหมด)
        btn.interactable = false;
    }

    void RefreshOutput()
    {
        // เว้นวรรคอัตโนมัติ แต่ถ้ามี "?" ให้ติดท้ายคำก่อนหน้า
        var text = string.Join(" ", chosen);
        text = text.Replace(" ?", "?"); // เก็บเครื่องหมาย ? ติดคำหน้า
        outputText.text = text;
    }

    void ClearOutput()
    {
        chosen.Clear();
        foreach (var b in tileButtons) b.interactable = true;
        outputText.text = "";
        hintText.text = "";
    }

    void ShuffleTiles()
    {
        // คืนสถานะก่อนแล้วสุ่มปุ่มใหม่
        ClearOutput();
        foreach (var b in tileButtons) b.transform.SetSiblingIndex(Random.Range(0, tilesContainer.childCount));
    }

    void Submit()
    {
        if (passed) return;

        string user = Normalize(outputText.text);
        string ans = Normalize(def.answer);

        if (string.IsNullOrWhiteSpace(user)) { hintText.text = "Pick words to form a sentence."; return; }

        if (user == ans)
        {
            passed = true;
            running = false;
            AudioManager.I?.PlayCorrect();
            hintText.text = "<color=#00C853>Correct!</color>";

            // ให้รางวัล grammar token
            if (def.reward != null && def.reward.grammar > 0)
            {
                // คุณจะไปเก็บที่ไหนก็ได้ เช่น QuestManager/PlayerState
                // ตัวอย่าง: เพิ่มอีเวนต์บอกว่าผ่านแล้ว
                EventBus.EmitScrambleComplete();
            }

            btnClose.gameObject.SetActive(true);
            btnClose.Select();
        }
        else
        {
            AudioManager.I?.PlayWrong();
            // โชว์ hint รอบแรก (ไม่ลบ progress)
            hintText.text = $"<color=#FFA000>Hint:</color> {def.hint}";
        }
    }

    string Normalize(string s)
    {
        s = s.Trim().ToLower();
        while (s.Contains("  ")) s = s.Replace("  ", " ");
        // จัดการเว้นวรรคหน้าเครื่องหมายคำถาม
        s = s.Replace(" ?", "?");
        return s;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
