using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI GameObjects")]
    public GameObject inventory;

    private void Update()
    {
        InventoryInput();
        if (Input.GetKeyUp(Define.EscapeKey))
            UIManager.Instance.HandleEscapeKey();
    }

    private void InventoryInput()
    {
        if (Input.GetKeyDown(Define.InventoryKey))
        {
            if (UIManager.Instance.CurrentActiveUI == null)
            {
                UIManager.Instance.OpenUI(inventory, true, true);
                InventoryController.Instance.ListItems();
            }
            else if (UIManager.Instance.CurrentActiveUI == inventory)
                UIManager.Instance.CloseUI();
        }
    }
}