using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    private GameObject currentActiveUI = null; // ���� Ȱ��ȭ�� UI �̸�
    private bool canCloseWithEscape = true; // ESC�� �ݱ� ���� ����

    public GameObject CurrentActiveUI => currentActiveUI; // ���� Ȱ��ȭ�� UI ��ȯ

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject HUD;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField] private TMP_Text interactionText;

    public void EnableInteractionText(string text)
    {
        interactionText.text = text + " (F)";
        interactionText.gameObject.SetActive(true);
    }

    public void DisableInteractionText()
    {
        interactionText.gameObject.SetActive(false);
    }

    // "canCloseWithEsc"ESC�� ���� �� �ִ��� ����
    public void OpenUI(GameObject UI, bool canCloseWithEsc, bool canMove)
    {
        if (currentActiveUI != null)
        {
            Debug.LogWarning($"�̹� UI({currentActiveUI})�� ���� �ֽ��ϴ�.");
            return;
        }

        UI.SetActive(true);
        currentActiveUI = UI;
        canCloseWithEscape = canCloseWithEsc;
        SetCursor(true);
        SetMovement(canMove);
    }

    public void CloseUI()
    {
        if (currentActiveUI == null)
        {
            Debug.LogWarning("���� UI�� �����ϴ�.");
            return;
        }

        currentActiveUI.SetActive(false);
        currentActiveUI = null;
        canCloseWithEscape = false;
        SetCursor(false);
        SetMovement(true);
    }

    // ESC Ű�� ������ �� ó��
    public void HandleEscapeKey()
    {
        if (currentActiveUI != null)
        {
            if (canCloseWithEscape)
                CloseUI(); // ���� ���� UI �ݱ�
        }
        else
        {
            OpenUI(menu, true, false); // �޴� UI ����
        }
    }

    private void SetCursor(bool visible)
    {
        HUD.SetActive(!visible);
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void SetMovement(bool canMove)
    {
        InteractController.Instance.canInteract = !InteractController.Instance.canInteract;
        FPSController.Instance.HandleCanMove(canMove);
    }
}
