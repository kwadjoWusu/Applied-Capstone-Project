using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera cameraObject;
    [SerializeField] Transform cameraPivotTransform;
    public PlayerManager player;

    // We can change these settings to tweak camera performance
    [Header("Camera Settings")]
    private float cameraSmoothSpeed = 1; // the bigger the number, the longer for the camera to reach its position durning movement 
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float leftAndRightRotationSpeed = 220;

    [SerializeField] float minimumPivot = -30; // the lowest point you are able to look down 
    [SerializeField] float maximumPivot = 60; // the highest point you are abel to look up
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;





    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition; // values used for the camera's collision
    private float targetCameraZPosition; // values used for camera collisions
    private Vector3 cameraObjectPosition; //uses for collisions(moves the camera obejct to this position)
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
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            // Follow the player
            HandleFollowTarget();
            //Rotate around the player
            HandleRotations();
            // collide with object
            HandleCollisions();


        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;

    }

    private void HandleRotations()
    {
        // if locked on, force rotation towards target
        // else rotate regularly 

        //normal rotations
        //rotation left and right based on the horizontal movement on the right joystick or mouse
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        //rotation up and down based on the vertical movement on the right joystick or mouse
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        // Clamp the up and down look angle between a minimum and a maximum value
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        // rotate this gameobject left and right
        Vector3 cameraRotation = Vector3.zero;
        cameraRotation.y = leftAndRightLookAngle;
        Quaternion targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        // rotation this gameobject up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;

    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        // DIRECTION FOR COLLISION
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // WE CHECK IF THERE IS AN OBJECT IN FRONT OF OUR DESIRED DIRECTION ^ (SEE ABOVE)
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), 0))
        {
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;

        }
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, collideWithLayers); 
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}
