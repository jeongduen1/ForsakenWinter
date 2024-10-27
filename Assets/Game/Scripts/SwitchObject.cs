using UnityEngine;

public class SwitchObject : Interactable
{
    private string offMessage = "On";
    private string onMessage = "Off";
    public bool OnOff = false;
    [SerializeField] public GameObject someThing;

    public override void OnFucus()
    {
        if (OnOff) HUDController.instance.EnableInteractionText(onMessage);
        else HUDController.instance.EnableInteractionText(offMessage);
    }

    public override void OnInteract()
    {
        OnOff = !OnOff;
        someThing.SetActive(OnOff);
    }

    public override void OnLoseFocus()
    {
        HUDController.instance.DisableInteractionText();
    }
}
