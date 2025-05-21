using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    public CharacterAnimatorManager characterAnimatorManager;
    public CharacterEffectsManager characterEffectsManager;


    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool isPerformingAttack = false;


    public bool isJumping = false;
    public bool isGrounded = true;

    public bool canRotate = true;
    public bool canMove = true;





    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }

    protected virtual void Update()
    {
        // Only do network‐sync if we actually have a CharacterNetworkManager
        if (characterNetworkManager == null) return;
        animator.SetBool("isGrounded", isGrounded);

        // If this character is being controlled from our side, then assign its network position to the position of out trsnsform
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        // if this character is being controlled from the other side
        {
            // Position
            transform.position = Vector3.SmoothDamp
                (transform.position,
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.networkPositionSmoothTime);

            // Rotation
            transform.rotation = Quaternion.Slerp
                (transform.rotation,
                characterNetworkManager.networkRotation.Value,
                characterNetworkManager.networkRotationSmoothTime);
        }

    }

    protected virtual void LateUpdate()
    {

    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currHealth.Value = 0;
            isDead.Value = true;
            //RESET ANY FLAGS HERE THAT NEED TO BE RESET
            //NOTHING YET
            //IF WE ARE NOT GROUNDED, PLAY AN AERIAL DEATH ANIMATION


            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetAnimation("Dead_01", true);
            }
        }

        //
        //PLAY SOME DEATH SFX
        yield return new WaitForSeconds(5);
        //AWARD PLAYERS WITH RUNES
        //DISABLE CHARACTER

    }
    public virtual void ReviveCharacter()
    {

    }


}
