using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canInteract = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float crouchSpeed = 1.2f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float sensitivity = 1.0f;
    [SerializeField, Range(1, 100)] private float upperLookLimite = 80.0f;
    [SerializeField, Range(1, 100)] private float lowerLookLimite = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouching Parameters")]
    [SerializeField] private float crouchingHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0,0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteratable;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
                HandleJump();
            if (canCrouch)
                HandleCrouch();
            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            ApplyMovement();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxisRaw("Vertical"),
            (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxisRaw("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) 
        + (transform.transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxisRaw("Mouse Y") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimite, lowerLookLimite);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * sensitivity, 0);
    }

    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
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
        if (Input.GetKeyDown(interactKey) 
            && currentInteratable != null 
            && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteratable.OnInteract();
        }
    }

    private void ApplyMovement()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        float timeElapse = 0;
        float targetHeight = isCrouching ? standingHeight : crouchingHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapse < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapse/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapse / timeToCrouch);
            timeElapse += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }
}