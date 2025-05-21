using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }
    public void UpdateAnimatorMovement(float horizontalValue, float verticalValue, bool isSprinting)
    {
        float horizontalAmount = horizontalValue;
        float verticalAmount = verticalValue;



        if (isSprinting)
        { // if isSprinting 
            verticalAmount = 2;
        }
        character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);




    }

    public virtual void PlayTargetAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {

        character.animator.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        // used to stop character form attempting another action
        // if you get damaged , and begin performing a damage animation
        //this flag will stop you from doing anything again(turn true if you are stunned)
        // can check for this before attemption new actions
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;
        // tell the server/host we played an anmation, and to play that animation for everybody else present
        character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }


}