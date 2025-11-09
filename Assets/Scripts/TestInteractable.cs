using UnityEngine;
public class TestInteractable : Interactable
{
    public override void OnInteract(PlayerController player)
    {
        Debug.Log("Hello from sign!");
    }
}
