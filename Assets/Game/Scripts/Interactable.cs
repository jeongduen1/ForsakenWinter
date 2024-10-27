using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Awake()
    {
        gameObject.layer = 9;
    }
    public abstract void OnInteract();
    public abstract void OnFucus();
    public abstract void OnLoseFocus();
}
