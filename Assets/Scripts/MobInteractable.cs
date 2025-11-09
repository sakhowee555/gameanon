using UnityEngine;

public class MobInteractable : Interactable
{
    public TextAsset vocabJson;                  // ลากไฟล์ vocab_injured_cat.json
    public VocabMiniGameController controller;   // อ้างถึง UI Controller ใน Canvas

    public override void OnInteract(PlayerController player)
    {
        controller.OpenWithJson(vocabJson);
    }
}
