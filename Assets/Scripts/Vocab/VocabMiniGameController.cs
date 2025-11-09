using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class VocabMiniGameController : MonoBehaviour
{
    [Header("Root")]
    public GameObject panel;              // ทั้งหน้ามินิเกม (inactive ตอนเริ่ม)

    [Header("Study Mode")]
    public GameObject studyRow;           // แถวการ์ดเรียน
    public VocabCardUI[] studyCards;      // 4 ใบ

    [Header("Test Mode")]
    public GameObject testRow;            // แถวทดสอบ
    public Image[] imageSlots;            // 4 รูป (สุ่มตำแหน่ง)
    public Button[] wordButtons;          // 4 ปุ่มคำ (สุ่มตำแหน่ง)
    public TMP_Text timerText;
    public TMP_Text hintText;

    [Header("Controls")]
    public Button btnClose;
    public Button btnRetry;

    [Header("Config")]
    public float studyTimeSec = 6f;       // เวลาโหมดเรียน (แนะนำ 5-8 วิ)

    private VocabSet currentSet;
    private int timeLeft;
    private int correctPairs;
    private bool running;
    private string selectedWordId = null; // คำที่ถูกเลือกไว้
    private Dictionary<string, Sprite> spriteCache = new();
    private Dictionary<string, AudioClip> audioCache = new();

    void Awake()
    {
        panel.SetActive(false);
        btnRetry.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
    }

    // โหลด JSON + เปิดมินิเกม
    public void OpenWithJson(TextAsset json)
    {
        currentSet = JsonUtility.FromJson<VocabSet>(json.text);
        correctPairs = 0;
        selectedWordId = null;
        hintText.text = "";
        PreloadAssets();
        panel.SetActive(true);
        Time.timeScale = 0f; // หยุดโลกข้างหลัง

        // เริ่มโหมดเรียน
        StartCoroutine(StudyThenTest());
    }

    void PreloadAssets()
    {
        spriteCache.Clear();
        audioCache.Clear();
        foreach (var c in currentSet.cards)
        {
            if (!spriteCache.ContainsKey(c.id))
                spriteCache[c.id] = Resources.Load<Sprite>(c.imagePath);
            if (!audioCache.ContainsKey(c.id))
                audioCache[c.id] = Resources.Load<AudioClip>(c.audioPath);
        }
    }

    System.Collections.IEnumerator StudyThenTest()
    {
        // ---- Study ----
        studyRow.SetActive(true);
        testRow.SetActive(false);
        btnRetry.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);

        for (int i = 0; i < currentSet.cards.Count && i < studyCards.Length; i++)
        {
            var card = currentSet.cards[i];
            studyCards[i].gameObject.SetActive(true);
            studyCards[i].BindStudy(spriteCache[card.id], card.word, card.th, audioCache[card.id]);
        }

        yield return new WaitForSecondsRealtime(studyTimeSec);

        // ---- Test ----
        SetupTestUI();
        StartTest();
    }

    void SetupTestUI()
    {
        studyRow.SetActive(false);
        testRow.SetActive(true);

        // สุ่มเรียงรูป/คำ
        var cards = currentSet.cards.OrderBy(_ => Random.value).ToList();
        for (int i = 0; i < imageSlots.Length; i++)
        {
            var c = cards[i];
            imageSlots[i].sprite = spriteCache[c.id];
            imageSlots[i].name = "img_" + c.id;
        }

        var wordOrder = currentSet.cards.OrderBy(_ => Random.value).ToList();
        for (int i = 0; i < wordButtons.Length; i++)
        {
            var c = wordOrder[i];
            var t = wordButtons[i].GetComponentInChildren<TMP_Text>();
            t.text = c.word;
            string id = c.id;

            // เคลียร์ listener เก่า
            wordButtons[i].onClick.RemoveAllListeners();
            wordButtons[i].onClick.AddListener(() => OnWordClicked(id));
            wordButtons[i].interactable = true;
        }

        // คลิกที่รูปเพื่อตอบคู่
        foreach (var img in imageSlots)
        {
            var btn = img.GetComponent<Button>();
            if (!btn) img.gameObject.AddComponent<Button>();
            btn = img.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            string imgId = img.name.Substring(4);
            btn.onClick.AddListener(() => OnImageClicked(imgId));
        }
    }

    void StartTest()
    {
        correctPairs = 0;
        timeLeft = currentSet.time_limit_sec;
        running = true;
        btnRetry.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        StartCoroutine(TimerTick());
        hintText.text = "Select a word, then click its picture.";
    }

    System.Collections.IEnumerator TimerTick()
    {
        while (running && timeLeft >= 0)
        {
            timerText.text = $"{timeLeft}s";
            yield return new WaitForSecondsRealtime(1f);
            timeLeft--;
            if (timeLeft < 0) break;
        }
        if (running) OnTimeUp();
    }

    void OnTimeUp()
    {
        running = false;
        hintText.text = "Time's up! Try again.";
        btnRetry.gameObject.SetActive(true);
        btnRetry.onClick.RemoveAllListeners();
        btnRetry.onClick.AddListener(() => { SetupTestUI(); StartTest(); });
    }

    void OnWordClicked(string id)
    {
        selectedWordId = id;
        hintText.text = $"Selected: {id}. Now click the matching picture.";
        var ac = audioCache.ContainsKey(id) ? audioCache[id] : null;
        if (ac) AudioManager.I?.PlaySfx(ac);
    }

    void OnImageClicked(string imgId)
    {
        if (!running) return;
        if (string.IsNullOrEmpty(selectedWordId))
        {
            hintText.text = "Pick a word first.";
            return;
        }
        bool ok = (imgId == selectedWordId);
        if (ok)
        {
            correctPairs++;
            // ปิดปุ่มของคู่นี้
            DisableWordButton(selectedWordId);
            GreyOutImage(imgId);
            AudioManager.I?.PlayCorrect();
            hintText.text = $"Great! {correctPairs}/{currentSet.win_requirement}";

            if (correctPairs >= currentSet.win_requirement)
            {
                Win();
            }
        }
        else
        {
            AudioManager.I?.PlayWrong();
            hintText.text = "Not matched. Try again!";
        }
    }

    void DisableWordButton(string id)
    {
        foreach (var b in wordButtons)
        {
            var t = b.GetComponentInChildren<TMP_Text>();
            if (t != null && t.text.ToLower() == id.ToLower())
            {
                b.interactable = false;
                break;
            }
        }
    }

    void GreyOutImage(string id)
    {
        foreach (var img in imageSlots)
        {
            if (img.name == "img_" + id)
            {
                var c = img.color; c.a = 0.4f; img.color = c;
                break;
            }
        }
    }

    void Win()
    {
        running = false;
        hintText.text = "Completed!";
        btnClose.gameObject.SetActive(true);
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() => {
            // ให้รางวัล token
            if (currentSet.reward != null && currentSet.reward.vocab > 0)
            {
                // คุณจะจัดเก็บ token ที่ไหนก็ได้
                // เช่น เพิ่มค่าใน QuestManager หรือ PlayerState ก็ว่าไป
                // ส่งอีเวนต์บอกว่าผ่านมินิเกมแล้ว
                EventBus.EmitVocabComplete();
            }
            ClosePanel();
        });
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
