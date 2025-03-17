using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PlayerInputManager : MonoBehaviour
{
    // Think About Goals in Steps
    // 1. find a way to read the values of a joy stick
    // 2. move character based on those values

    public static PlayerInputManager instance;
    [SerializeField] Vector2 movementInput;
    PlayerControls playerControls;


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
        if (arg0.buildIndex == WorldSaveGameManager.singletonInstance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        // otherwise we must be at the main menu, disable our players controls
        // this is so our player cant move around if we enter thingd like a charater creation menu or other things
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

        }
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        // if we destroy this object unsubscribe to stop memory leaks
        SceneManager.activeSceneChanged -= OnSceneChange;
    }


}
