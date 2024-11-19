using UnityEngine;

public abstract class ObjectToggle : Interactable
{
    protected bool isActive = false;

    public override void OnInteract()
    {
        isActive = !isActive;
    }

    private string message = "switch";
    public override void OnFucus()
    {
        UIManager.Instance.EnableInteractionText(message);
    }

    public override void OnLoseFocus()
    {
        UIManager.Instance.DisableInteractionText();
    }
}
