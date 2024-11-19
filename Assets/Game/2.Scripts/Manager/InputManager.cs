using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool GetSprintInput() => Input.GetKey(Define.SprintKey);
    public bool GetJumpInput() => Input.GetKeyDown(Define.JumpKey);
    public bool GetCrouchInput() => Input.GetKeyDown(Define.CrouchKey);
    public bool GetInteractInput() => Input.GetKeyDown(Define.InteractKey);
    public bool GetGameMenuInput() => Input.GetKeyDown(Define.EscapeKey);
    public bool GetInventoryInput() => Input.GetKeyDown(Define.InventoryKey);
}
