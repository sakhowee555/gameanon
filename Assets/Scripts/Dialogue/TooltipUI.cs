using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public RectTransform root;
    public TMP_Text text;

    public void Show(Vector2 screenPos, string msg)
    {
        text.text = msg;
        root.gameObject.SetActive(true);
        root.position = screenPos + new Vector2(12, -12);
    }

    public void Hide()
    {
        root.gameObject.SetActive(false);
    }
}
