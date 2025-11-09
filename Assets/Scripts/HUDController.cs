using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    public Slider xpBar;
    public TMP_Text questTitle;

    private void OnEnable()
    {
        GameEvents.OnXPChanged += SetXP;
        GameEvents.OnActiveQuestChanged += SetQuestTitle;
    }
    private void OnDisable()
    {
        GameEvents.OnXPChanged -= SetXP;
        GameEvents.OnActiveQuestChanged -= SetQuestTitle;
    }

    public void SetXP(int current, int max)
    {
        if (xpBar)
        {
            xpBar.maxValue = max;
            xpBar.value = current;
        }
    }

    public void SetQuestTitle(string title)
    {
        if (questTitle) questTitle.text = string.IsNullOrEmpty(title) ? "No Active Quest" : title;
    }
}
