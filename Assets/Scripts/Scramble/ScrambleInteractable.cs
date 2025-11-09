using UnityEngine;

public class ScrambleInteractable : Interactable
{
    public TextAsset scrambleJson;         // ลากไฟล์ scramble_cave_gate.json
    public ScrambleController controller;  // อ้าง UI controller ใน Canvas

    public override void OnInteract(PlayerController player)
    {
        controller.OpenWithJson(scrambleJson);
    }
}
