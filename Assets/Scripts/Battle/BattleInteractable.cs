using UnityEngine;

public class BattleInteractable : Interactable
{
    public TextAsset battleJson;         // ลาก battle_monkey.json
    public BattleController controller;  // อ้าง UI บน Canvas

    public override void OnInteract(PlayerController player)
    {
        controller.OpenBattle(battleJson);
    }
}
