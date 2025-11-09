using UnityEngine;
public class InteractableScanner : MonoBehaviour
{
    public float radius = 0.5f;
    public LayerMask mask;
    public GameObject hintUI; // ไอคอน/ป้าย "E"

    void Update()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, mask);
        bool has = hit != null && hit.GetComponentInParent<Interactable>() != null;

        if (hintUI) hintUI.SetActive(has);
        if (has)
        {
            // วาง hint UI ทับตำแหน่ง object หรือบนหัว player ก็ได้
            // ในเดโม แบบง่าย: ติด UI ไว้ที่ player
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
