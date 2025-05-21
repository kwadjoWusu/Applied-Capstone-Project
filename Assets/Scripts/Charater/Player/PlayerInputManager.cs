using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PlayerInputManager : MonoBehaviour
{
    // Think About Goals in Steps
    // 1. find a way to read the values of a joy stick
    // 2. move character based on those values

    public static PlayerInputManager instance;
    public PlayerManager player;
    PlayerControls playerControls;

    [Header("Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    [SerializeField] public float moveAmount;

    [Header("Camera Movement Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Player Actions")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;

    [Header("Attack Input Flags")]
    private bool lightAttackInput = false;
    private bool heavyAttackInput = false;




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // when the scene changes, this logic is run once
        SceneManager.activeSceneChanged += OnSceneChange;


        instance.enabled = false;

    }


    private void OnSceneChange(Scene arg0/*old scene*/ , Scene arg1/*new scene*/)
    {
        // if we are loading into our world scene, enable our player inputs
        if (arg1.buildIndex == WorldSaveGameManager.singletonInstance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        // otherwise we must be at the main menu, disable our players controls
        // this is so our player cant move around if we enter thing like a charater creation menu or other things
        else
        {
            instance.enabled = false;
        }

    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            player = GetComponent<PlayerManager>();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerAction.Dodge.performed += i => dodgeInput = true;

            playerControls.PlayerAction.Jump.performed += i => jumpInput = true;


            // Holding the input, set the bool to true
            playerControls.PlayerAction.Sprint.performed += i => sprintInput = true;
            // Releases the input, set the bool to false
            playerControls.PlayerAction.Sprint.canceled += i => sprintInput = false;
            // Hook up light attack (press)
            playerControls.PlayerAction.LightAttack.performed += i => lightAttackInput = true;

            // Hook up heavy attack (hold & release)
            playerControls.PlayerAction.HeavyAttack.performed += i => heavyAttackInput = true;







        }
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        // if we destroy this object unsubscribe to stop memory leaks
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        //Good for testing
        if (enabled)
        {
            if (hasFocus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }
    private void Update()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleAttackInputs();

    }
    // Movement
    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        // we clamp the values, so they ar 0,0.5, 1
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }
        // why do we pass 0 on the horizontal? it is beacause we only what non-strafting movement
        //we use the horizontal when we are strafing or locked on
        if (player == null)
        {
            return;
        }

        //if we are not locked on only use the move amount
        player.playerAnimatorManager.UpdateAnimatorMovement(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

        //if we are locked on pass the horizontal movement as well

    }


    private void HandleCameraMovementInput()
    {

        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    // Action
    private void HandleDodgeInput()
    {

        if (dodgeInput)
        {
            dodgeInput = false;

            //perform a dodge
            player.playerLocomotionManager.AttemptToPerformDodge();

        }
    }


    public void HandleSprintInput()
    {
        if (sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }

    public void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            // if you have a ui window open, dont allow jumping

            // Attempt to perform jump
            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }
    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.PlayerAction.LightAttack.performed -= ctx => lightAttackInput = true;
            playerControls.PlayerAction.HeavyAttack.performed -= ctx => heavyAttackInput = true;
            playerControls.Disable();
        }
    }



    private void HandleAttackInputs()
    {
        // only do anything if you pressed one of the attack buttons
        if (!lightAttackInput && !heavyAttackInput)
            return;

        var combat = player.playerCombatManager;
        bool armed = combat.HasSwordEquipped();

        // LIGHT ATTACK BUTTON
        if (lightAttackInput)
        {
            lightAttackInput = false;

            // if I don’t have a sword, bail immediately
            if (!armed)
                return;

            // aerial light-attack
            if (!player.isGrounded)
            {
                combat.QueueJumpAttack();
            }
            // otherwise normal light combo
            else
            {
                combat.QueueLightAttack();
            }
        }
        // HEAVY ATTACK BUTTON
        else if (heavyAttackInput)
        {
            heavyAttackInput = false;

            // heavy attacks also only when armed
            if (armed)
                combat.QueueHeavyAttack();
        }
    }


}





