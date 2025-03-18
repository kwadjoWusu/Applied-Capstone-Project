using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;


    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float rotationSpeed = 15;



    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void HandleAllMovement()
    {
        // Ground Movements
        HandleGroundedMovement();

        // Aerial Movements
            // Jumping Movements
            // Rotation
            HandleRotation();
            // Falling
    }

    private void GetVerticalAndHorizontal()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;

        //Clamp the movements
    }
    private void HandleGroundedMovement()
    {

        GetVerticalAndHorizontal();
        // Player movement is based on their camera's facing perspective & our movement inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {

            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation(){
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward *verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.y = 0;
        targetRotationDirection.Normalize();

        if (targetRotationDirection == Vector3.zero){
            targetRotationDirection = transform.forward;
        }
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation,rotationSpeed *Time.deltaTime);
        transform.rotation = targetRotation;

    }
}
