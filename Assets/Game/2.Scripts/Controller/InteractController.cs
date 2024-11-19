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
        // 카메라의 뷰포인트에서 레이캐스트를 발사
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            // 오브젝트에 Interactable 컴포넌트가 있는지 확인
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                // 이전에 감지한 상호작용 오브젝트와 다른 경우에만 업데이트
                if (currentInteratable == null || hit.collider.gameObject.GetInstanceID() != currentInteratable.GetInstanceID())
                {
                    currentInteratable?.OnLoseFocus(); // 기존 상호작용 오브젝트의 포커스 해제
                    currentInteratable = interactable;
                    currentInteratable.OnFucus();
                }
            }
            else
            {
                // 상호작용 가능한 오브젝트가 아니므로 포커스 해제
                if (currentInteratable != null)
                {
                    currentInteratable.OnLoseFocus();
                    currentInteratable = null;
                }
            }
        }
        else if (currentInteratable != null)
        {
            // 레이캐스트가 아무 오브젝트도 맞추지 않았을 때
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
