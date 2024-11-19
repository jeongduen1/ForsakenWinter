using Unity.VisualScripting;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    public static InteractController Instance;

    [Header("Interaction Parameters")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteratable;

    public bool canInteract = true;

    private Camera playerCamera;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (canInteract && !Cursor.visible)
        {
            HandleInteractionCheck();
            HandleInteractionInput();
        }
    }

    private void HandleInteractionCheck()
    {
        // ī�޶��� ������Ʈ���� ����ĳ��Ʈ�� �߻�
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            // ������Ʈ�� Interactable ������Ʈ�� �ִ��� Ȯ��
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                // ������ ������ ��ȣ�ۿ� ������Ʈ�� �ٸ� ��쿡�� ������Ʈ
                if (currentInteratable == null || hit.collider.gameObject.GetInstanceID() != currentInteratable.GetInstanceID())
                {
                    currentInteratable?.OnLoseFocus(); // ���� ��ȣ�ۿ� ������Ʈ�� ��Ŀ�� ����
                    currentInteratable = interactable;
                    currentInteratable.OnFucus();
                }
            }
            else
            {
                // ��ȣ�ۿ� ������ ������Ʈ�� �ƴϹǷ� ��Ŀ�� ����
                if (currentInteratable != null)
                {
                    currentInteratable.OnLoseFocus();
                    currentInteratable = null;
                }
            }
        }
        else if (currentInteratable != null)
        {
            // ����ĳ��Ʈ�� �ƹ� ������Ʈ�� ������ �ʾ��� ��
            currentInteratable.OnLoseFocus();
            currentInteratable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(Define.InteractKey)
            && currentInteratable != null
            && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteratable.OnInteract();
        }
    }
}
