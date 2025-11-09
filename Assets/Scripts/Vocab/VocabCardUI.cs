using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VocabCardUI : MonoBehaviour
{
    [Header("Study Card")]
    public Image icon;
    public TMP_Text wordEN;
    public TMP_Text wordTH;
    public Button soundBtn;

    private AudioClip clip;

    public void BindStudy(Sprite sprite, string en, string th, AudioClip audioClip)
    {
        if (icon) icon.sprite = sprite;
        if (wordEN) wordEN.text = en;
        if (wordTH) wordTH.text = th;
        clip = audioClip;

        if (soundBtn)
        {
            soundBtn.onClick.RemoveAllListeners();
            soundBtn.onClick.AddListener(() => {
                if (clip) AudioManager.I?.PlaySfx(clip); // ใช้ AudioSource sfx เล่นเสียงคำ
            });
        }
    }
}
