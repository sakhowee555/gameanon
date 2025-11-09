using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [TextArea] public string interactHint = "Press [E]";

    // เรียกเมื่อผู้เล่นกด E และโดน collider ของเรา
    public virtual void OnInteract(PlayerController player)
    {
        Debug.Log($"Interact with: {name}");
    }
}
