using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueRenderer : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler
{
    [Header("Refs")]
    public TMP_Text lineText;          // ข้อความ EN
    public TMP_Text lineTextTH;        // ข้อความ TH (แปลไทย)
    public TMP_Text speakerText;       // ชื่อผู้พูด
    public TooltipUI tooltip;          // กล่อง tooltip ลอย
    public AudioSource voiceSource;    // เล่นไฟล์เสียงประโยค

    // สีสำหรับแต่ละแท็ก
    const string COLOR_VAUX = "#3AA6FF";  // ฟ้าอ่อน
    const string COLOR_V = "#3AA6FF";  // ฟ้า
    const string COLOR_NPL = "#FF5A5A";  // แดง
    const string COLOR_QWORD = "#FFD24D";  // เหลือง

    // แปลงแท็กของเรา → rich text + link ของ TMP
    public string RenderTagged(string src)
    {
        if (string.IsNullOrEmpty(src)) return "";
        string s = src;
        s = s.Replace("<v-aux>", $"<link=vaux><color={COLOR_VAUX}>")
             .Replace("</v-aux>", "</color></link>");
        s = s.Replace("<v>", $"<link=v><color={COLOR_V}>")
             .Replace("</v>", "</color></link>");
        s = s.Replace("<n-pl>", $"<link=npl><color={COLOR_NPL}>")
             .Replace("</n-pl>", "</color></link>");
        s = s.Replace("<qword>", $"<link=qword><color={COLOR_QWORD}>")
             .Replace("</qword>", "</color></link>");
        return s;
    }

    public void SetSpeaker(string name) => speakerText.text = name;

    public void SetLines(string en, string th)
    {
        lineText.text = RenderTagged(en);
        lineTextTH.text = th ?? "";
    }

    public void Clear()
    {
        lineText.text = "";
        lineTextTH.text = "";
        speakerText.text = "";
        tooltip.Hide();
    }

    // Tooltip: เช็คลิงก์ใต้เมาส์
    public void OnPointerMove(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(lineText, Input.mousePosition, null);
        if (linkIndex != -1)
        {
            var linkInfo = lineText.textInfo.linkInfo[linkIndex];
            string type = linkInfo.GetLinkID(); // vaux, v, npl, qword
            string word = linkInfo.GetLinkText();

            string tip = type switch
            {
                "vaux" => $"{word}\n(กริยาช่วย / auxiliary verb)",
                "v" => $"{word}\n(กริยา / verb)",
                "npl" => $"{word}\n(คำนามพหูพจน์ / plural noun)",
                "qword" => $"{word}\n(คำถาม / question word)",
                _ => word
            };
            tooltip.Show(eventData.position, tip);
        }
        else tooltip.Hide();
    }

    public void OnPointerClick(PointerEventData eventData) { /* เผื่ออนาคตคลิกเพื่อฟีเจอร์เสริม */ }

    // เล่นเสียง (โยนไฟล์เข้ามาหรือใช้ playOneShot)
    public void PlayVoice(AudioClip clip)
    {
        if (!clip || !voiceSource) return;
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }
}
