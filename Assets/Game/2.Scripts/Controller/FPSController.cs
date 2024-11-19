using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public static FPSController Instance;

    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.C;

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
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

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
        Instance = this;
    }

    private void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            if (!Cursor.visible)
                HandleMouseLook();

            if (canJump)
                HandleJump();
            if (canCrouch)
                HandleCrouch();
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

    private void ApplyMovement()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        // 플레이어가 앉은 상태에서 일어설 때, 위쪽에 충분한 공간이 있는지 확인
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, standingHeight - crouchingHeight))
            yield break;

        duringCrouchAnimation = true;

        float timeElapse = 0;
        float targetHeight = isCrouching ? standingHeight : crouchingHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapse < timeToCrouch)
        {
            // 캐릭터 컨트롤러의 높이와 중심을 천천히 변화시킴
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapse / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapse / timeToCrouch);

            timeElapse += Time.deltaTime;
            yield return null;
        }

        // 최종적으로 높이와 중심을 설정
        characterController.height = targetHeight;
        characterController.center = targetCenter;

        // 앉기 상태를 토글하고 애니메이션 중 상태를 종료
        isCrouching = !isCrouching;
        duringCrouchAnimation = false;
    }

    public void HandleCanMove(bool value)
    {
        CanMove = value;
    }
}