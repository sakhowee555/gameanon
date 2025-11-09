using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{
    [Header("Root")]
    public GameObject panel;

    [Header("Enemy/Player UI")]
    public Image enemySprite;
    public Transform enemyHPContainer; // วางหัวใจ/แถบ 5 ชิ้น (Image หรือ Slider ก็ได้)
    public Transform playerHPContainer; // วางหัวใจ 3 ชิ้น
    public Sprite hpOn, hpOff; // ถ้าใช้ Image heart

    [Header("Common UI")]
    public TMP_Text questionText;
    public TMP_Text timerText;
    public TMP_Text hintText;

    [Header("Scramble")]
    public GameObject subScramble;
    public RectTransform scrambleTilesParent;
    public Button scrambleTilePrefab;

    [Header("Choice")]
    public GameObject subChoice;
    public Button[] choiceButtons;
    public TMP_Text[] choiceTexts;

    [Header("Fill")]
    public GameObject subFill;
    public TMP_Text templateFillText;
    public TMP_InputField fillInput;
    public Button fillSubmit;

    [Header("DragFill (ใช้แบบเลือกคำใส่ช่อง)")]
    public GameObject subDragFill;
    public TMP_Text templateDragText;
    public Transform bankButtonsParent;
    public Button bankButtonPrefab;
    public Transform chosenSlotsParent; // สร้าง Slot เป็น Button/Label ให้ผู้เล่นกดใส่ตามลำดับ
    public TMP_Text[] chosenSlotsTexts;
    public Button dragSubmit;
    public Button dragClear;

    [Header("Controls")]
    public Button btnRetry;
    public Button btnClose;

    [Header("Data")]
    public TextAsset battleJson;
    public Sprite monkeySprite; // รูปมอน

    BattleDef def;
    int enemyHP, playerHP;
    int turnIndex;
    int timeLeft;
    bool running;

    // temp for scramble
    List<Button> scrambleButtons = new();
    List<string> chosenScramble = new();

    // temp for drag fill
    List<string> chosenDrag = new();

    void Awake()
    {
        panel.SetActive(false);
        btnRetry.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
    }

    public void OpenBattle(TextAsset json)
    {
        def = JsonUtility.FromJson<BattleDef>(json.text);
        enemyHP = def.enemyHP;
        playerHP = def.playerHP;
        turnIndex = 0;

        // UI init
        enemySprite.sprite = monkeySprite;
        RefreshHPUI();
        questionText.text = "";
        hintText.text = "";
        timerText.text = "";

        panel.SetActive(true);
        Time.timeScale = 0f;

        NextTurn();
    }

    void RefreshHPUI()
    {
        // ถ้าใช้ Image hearts:
        for (int i = 0; i < enemyHPContainer.childCount; i++)
        {
            var img = enemyHPContainer.GetChild(i).GetComponent<Image>();
            img.sprite = i < enemyHP ? hpOn : hpOff;
        }
        for (int i = 0; i < playerHPContainer.childCount; i++)
        {
            var img = playerHPContainer.GetChild(i).GetComponent<Image>();
            img.sprite = i < playerHP ? hpOn : hpOff;
        }
    }

    void NextTurn()
    {
        if (enemyHP <= 0) { Win(); return; }
        if (playerHP <= 0) { Lose(); return; }
        if (turnIndex >= def.turns.Count) { Win(); return; }

        // reset panels
        subScramble.SetActive(false);
        subChoice.SetActive(false);
        subFill.SetActive(false);
        subDragFill.SetActive(false);
        btnRetry.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        hintText.text = "";

        var t = def.turns[turnIndex];
        questionText.text = t.question ?? "";
        SetupTurn(t);

        // timer
        timeLeft = def.timePerTurn;
        running = true;
        if (timeLeft > 0) StartCoroutine(TimerTick());
    }

    System.Collections.IEnumerator TimerTick()
    {
        while (running && timeLeft >= 0)
        {
            timerText.text = $"{timeLeft}s";
            yield return new WaitForSecondsRealtime(1f);
            timeLeft--;
        }
        if (running && timeLeft < 0) // time up
        {
            running = false;
            PlayerHurt();
            hintText.text = "<color=#FFA000>Time's up.</color>";
            InvokeNext();
        }
    }

    void SetupTurn(BattleTurn t)
    {
        switch (t.type)
        {
            case "scramble":
                SetupScramble(t);
                break;
            case "choice":
                SetupChoice(t);
                break;
            case "fill":
                SetupFill(t);
                break;
            case "drag_fill":
                SetupDragFill(t);
                break;
            default:
                Debug.LogWarning("Unknown turn type: " + t.type);
                break;
        }
    }

    // --- SCRAMBLE ---
    void SetupScramble(BattleTurn t)
    {
        subScramble.SetActive(true);
        // clear old
        foreach (var b in scrambleButtons) if (b) Destroy(b.gameObject);
        scrambleButtons.Clear();
        chosenScramble.Clear();

        var shuffled = t.tiles.OrderBy(_ => Random.value).ToList();
        foreach (var w in shuffled)
        {
            var b = Instantiate(scrambleTilePrefab, scrambleTilesParent);
            b.GetComponentInChildren<TMP_Text>().text = w;
            b.onClick.AddListener(() => {
                chosenScramble.Add(w);
                b.interactable = false;
            });
            scrambleButtons.Add(b);
        }

        // ใช้ปุ่ม Submit/Hint ผ่าน Choice? ง่ายสุด: เพิ่มปุ่มยืนยันร่วมกันก็ได้
        // ที่นี่ เราให้ผู้เล่นคลิกครบแล้วกด Submit (ใช้ fillSubmit แทนชั่วคราว)
        fillSubmit.gameObject.SetActive(true);
        fillSubmit.onClick.RemoveAllListeners();
        fillSubmit.onClick.AddListener(() => {
            string user = Normalize(string.Join(" ", chosenScramble)).Replace(" ?", "?");
            string ans = Normalize(t.answer);
            Judge(user == ans, t.hint);
        });
    }

    // --- CHOICE ---
    void SetupChoice(BattleTurn t)
    {
        subChoice.SetActive(true);
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < t.options.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = t.options[i];
                int idx = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => {
                    bool ok = (idx == t.answerIndex);
                    Judge(ok, t.hint);
                });
            }
            else choiceButtons[i].gameObject.SetActive(false);
        }
    }

    // --- FILL ---
    void SetupFill(BattleTurn t)
    {
        subFill.SetActive(true);
        templateFillText.text = t.template;
        fillInput.text = "";
        fillSubmit.gameObject.SetActive(true);
        fillSubmit.onClick.RemoveAllListeners();
        fillSubmit.onClick.AddListener(() => {
            string user = Normalize(fillInput.text).Replace("  ", " ");
            string ans = Normalize(FillTemplateCheck(t.template, t.answers));
            Judge(user == ans, t.hint);
        });
    }
    string FillTemplateCheck(string template, List<string> answers)
    {
        // รวมเป็นสตริงที่คาดหวัง: แทน __ ด้วยแต่ละคำตามลำดับ
        string s = template;
        foreach (var a in answers)
        {
            int p = s.IndexOf("__");
            if (p >= 0) s = s.Remove(p, 2).Insert(p, a);
        }
        return s;
    }

    // --- DRAG_FILL (ใช้แบบกดคำจาก bank ใส่ลง slot ทีละช่อง) ---
    void SetupDragFill(BattleTurn t)
    {
        subDragFill.SetActive(true);
        templateDragText.text = t.template;
        chosenDrag.Clear();

        // สร้างปุ่ม bank
        foreach (Transform c in bankButtonsParent) Destroy(c.gameObject);
        foreach (var w in t.bank)
        {
            var b = Instantiate(bankButtonPrefab, bankButtonsParent);
            b.GetComponentInChildren<TMP_Text>().text = w;
            b.onClick.AddListener(() => {
                // หาช่องแรกที่ยังว่าง
                int slot = chosenDrag.Count;
                if (slot < t.answers.Count)
                {
                    chosenDrag.Add(w);
                    chosenSlotsTexts[slot].text = w;
                    b.interactable = false;
                }
            });
        }

        // ปุ่มเคลียร์
        dragClear.onClick.RemoveAllListeners();
        dragClear.onClick.AddListener(() => {
            chosenDrag.Clear();
            foreach (var txt in chosenSlotsTexts) txt.text = "__";
            foreach (Transform c in bankButtonsParent) c.GetComponent<Button>().interactable = true;
        });

        // ปุ่มส่ง
        dragSubmit.onClick.RemoveAllListeners();
        dragSubmit.onClick.AddListener(() => {
            bool ok = chosenDrag.Count == t.answers.Count;
            if (ok)
            {
                for (int i = 0; i < t.answers.Count; i++)
                    if (Normalize(chosenDrag[i]) != Normalize(t.answers[i])) { ok = false; break; }
            }
            Judge(ok, t.hint);
        });
    }

    void Judge(bool correct, string hint)
    {
        running = false;
        if (correct)
        {
            AudioManager.I?.PlayCorrect();
            EnemyHurt();
        }
        else
        {
            AudioManager.I?.PlayWrong();
            hintText.text = $"<color=#FFA000>Hint:</color> {hint}";
            PlayerHurt();
        }
        InvokeNext();
    }

    void EnemyHurt()
    {
        enemyHP = Mathf.Max(0, enemyHP - 1);
        RefreshHPUI();
    }
    void PlayerHurt()
    {
        playerHP = Mathf.Max(0, playerHP - 1);
        RefreshHPUI();
    }

    void InvokeNext()
    {
        // หน่วงเล็กน้อยให้เห็นผล HP
        LeanTween.delayedCall(0.6f, () => {
            turnIndex++;
            NextTurn();
        });
    }

    void Win()
    {
        running = false;
        hintText.text = "<color=#00C853>Victory!</color>";
        btnClose.gameObject.SetActive(true);
        btnRetry.gameObject.SetActive(false);

        // มอบรางวัล XP
        if (def.reward != null && def.reward.xp > 0)
            EventBus.EmitBattleEnd(true, def.reward.xp);

        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(ClosePanel);
    }

    void Lose()
    {
        running = false;
        hintText.text = "<color=#FF5252>Defeated. Try again.</color>";
        btnRetry.gameObject.SetActive(true);
        btnClose.gameObject.SetActive(false);

        btnRetry.onClick.RemoveAllListeners();
        btnRetry.onClick.AddListener(() => {
            // รีสตาร์ทการต่อสู้
            OpenBattle(battleJson);
        });

        EventBus.EmitBattleEnd(false, 0);
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    string Normalize(string s)
    {
        s = (s ?? "").Trim().ToLower();
        while (s.Contains("  ")) s = s.Replace("  ", " ");
        s = s.Replace(" ?", "?");
        return s;
    }
}
