using UnityEngine;

public class DroppedItem : Interactable
{
    [SerializeField] private ItemData item;

    public override void OnInteract()
    {
        InventoryController.Instance.AddItem(item);
        OnLoseFocus();
        Destroy(gameObject);
    }

    private string message = "PickUp";
    public override void OnFucus()
    {
        UIManager.Instance.EnableInteractionText(message);
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.DisableInteractionText();
    }
}
