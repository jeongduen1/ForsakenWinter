using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    private GameObject currentActiveUI = null; // 현재 활성화된 UI 이름
    private bool canCloseWithEscape = true; // ESC로 닫기 가능 여부

    public GameObject CurrentActiveUI => currentActiveUI; // 현재 활성화된 UI 반환

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

    // "canCloseWithEsc"ESC로 닫을 수 있는지 여부
    public void OpenUI(GameObject UI, bool canCloseWithEsc, bool canMove)
    {
        if (currentActiveUI != null)
        {
            Debug.LogWarning($"이미 UI({currentActiveUI})가 열려 있습니다.");
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
            Debug.LogWarning("닫을 UI가 없습니다.");
            return;
        }

        currentActiveUI.SetActive(false);
        currentActiveUI = null;
        canCloseWithEscape = false;
        SetCursor(false);
        SetMovement(true);
    }

    // ESC 키가 눌렸을 때 처리
    public void HandleEscapeKey()
    {
        if (currentActiveUI != null)
        {
            if (canCloseWithEscape)
                CloseUI(); // 현재 열린 UI 닫기
        }
        else
        {
            OpenUI(menu, true, false); // 메뉴 UI 열기
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
