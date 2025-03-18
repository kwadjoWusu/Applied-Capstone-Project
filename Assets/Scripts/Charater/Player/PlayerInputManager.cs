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

    PlayerControls playerControls;

    [Header("Movement Input")]
    public static PlayerInputManager instance;
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    [SerializeField] public float moveAmount;

    [Header("Camera Movement Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;


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

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput  =  i.ReadValue<Vector2>(); 


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
    }

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
    }
    private void HandleCameraMovementInput(){

        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

}
