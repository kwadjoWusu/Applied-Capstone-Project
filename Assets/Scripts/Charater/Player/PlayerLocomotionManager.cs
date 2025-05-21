using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [Header("Movement")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [SerializeField] float walkingSpeed = 5.5f;
    [SerializeField] float runningSpeed = 7.5f;
    [SerializeField] float sprintingSpeed = 10;
    [SerializeField] float rotationSpeed = 15;
    [SerializeField] int sprintingStaminaCost = 2;

    [Header("Jump")]
    [SerializeField] float jumpStamainaCost = 25;
    [SerializeField] float jumpHeight = 4f;
    private Vector3 jumpDirection;
    [SerializeField] float jumpForwardSpeed = 5;
    [SerializeField] float freeFallSpeed = 2;

    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStamainaCost = 25;

    // PlayerLocomotionManager.cs
    public bool isDodging = false;





    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    protected override void Update()
    {
        base.Update();
        if (player.IsOwner)
        {
            player.characterNetworkManager.verticalMovement.Value = verticalMovement;
            player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.moveAmount.Value = moveAmount;


        }
        else
        {
            moveAmount = player.characterNetworkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            //if not locked on, pass move amount
            player.playerAnimatorManager.UpdateAnimatorMovement(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

            //if locked on, pass vertical and horizontal values
        }
    }

    public void HandleAllMovement()
    {
        // Ground Movements
        HandleGroundedMovement();

        // Aerial Movements
        // Jumping Movements
        HandleJumpingMovement();
        // Free Fall Movement
        HandleFreeFallMovement();
        // Rotation
        HandleRotation();
        // Falling
    }

    private void GetMovementInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;

        //Clamp the movements
    }

    private void HandleGroundedMovement()
    {
        if (player.playerLocomotionManager.isDodging) return;
        if (!player.canMove) return;

        GetMovementInputs();
        // Player movement is based on their camera's facing perspective & our movement inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (player.playerNetworkManager.isSprinting.Value)
        {

            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);

        }
        else
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {

                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
    }


    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }

    }

    private void HandleFreeFallMovement()
    {
        if (!player.isGrounded)
        {
            Vector3 freeFallDirection;
            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection = freeFallDirection + PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
            freeFallDirection.y = 0;

            player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);

        }

    }

    private void HandleRotation()
    {

        if (player.playerLocomotionManager.isDodging || !player.canRotate) return;
        if (!player.canRotate)
            return;


        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.y = 0;
        targetRotationDirection.Normalize();

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;

    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            // Set Sprinting to false
            player.playerNetworkManager.isSprinting.Value = false;
        }

        // If we are out of stamina, set sprinting to false
        if (player.playerNetworkManager.currStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        // If we are moving set sprinting to true
        if (moveAmount >= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = true;

        }
        // If we are stationary/moving set sprinting to false
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.playerNetworkManager.currStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }

    public void AttemptToPerformDodge()
    {

        if (player.isPerformingAction)
            return;
        if (player.playerNetworkManager.currStamina.Value <= 0)
        {
            return;
        }
        // if we are moving and we attempt to perform dodge you will perform
        if (moveAmount > 0)
        {

            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;

            rollDirection.y = 0;
            rollDirection.Normalize();
            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;
            isDodging = true;
            // Perform a roll animation
            player.playerAnimatorManager.PlayTargetAnimation("Roll_Forward_01", true, true);
        }
        // if we are stationary, we perform a backstep
        else
        {
            // Perform a backstep animation
            player.playerAnimatorManager.PlayTargetAnimation("Back_Step_01", true, true);
        }
        player.playerNetworkManager.currStamina.Value -= dodgeStamainaCost;
    }


    public void AttemptToPerformJump()
    {
        //if we are performing a general action, we do not want to allow a jump (will change with combat)


        if (player.isPerformingAction)
            return;
        if (player.playerNetworkManager.currStamina.Value <= 0)
        {
            return;
        }
        if (player.isJumping)
            return;
        if (!player.isGrounded)
            return;

        // if we are two handing our weapon, play the two handed jumping animation otherwise play the one handed jumping animation
        player.playerAnimatorManager.PlayTargetAnimation("Main_Jump_01", false);
        player.isJumping = true;

        player.playerNetworkManager.currStamina.Value -= jumpStamainaCost;
        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            if (player.playerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 1;
            }
            else if (PlayerInputManager.instance.moveAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.25f;
            }

        }

    }

    public void ApplyJumpingVelocity()
    {
        //Apply an upward velocity depending on forces on our game
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);


    }
}
