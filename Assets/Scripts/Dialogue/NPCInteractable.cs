using UnityEngine;

public class NPCInteractable : Interactable
{
    public TextAsset dialogueJson;
    public DialogueController controller;

    public override void OnInteract(PlayerController player)
    {
        controller.StartDialogue(dialogueJson, onEndCallback: () =>
        {
            // เมื่อจบบทสนทนา (node.end) → สามารถอัปเดตเควสต่อได้
            // เช่น: GameEvents.OnActiveQuestChanged?.Invoke("Monkey Trouble");
        });
    }
}
