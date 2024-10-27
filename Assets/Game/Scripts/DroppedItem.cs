using UnityEditor;
using UnityEngine;

public class DroppedItem : Interactable
{
    private string message = "PickUp";
    public override void OnFucus()
    {
        HUDController.instance.EnableInteractionText(message);
    }

    public override void OnInteract()
    {
        
    }

    public override void OnLoseFocus()
    {
        HUDController.instance.DisableInteractionText();
    }
}