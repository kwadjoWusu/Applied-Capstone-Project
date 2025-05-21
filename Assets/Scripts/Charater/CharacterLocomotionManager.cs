using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;
    [Header("Ground Check and Jumping")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius  = 1;
    [SerializeField] protected Vector3 yVelocity;
    [SerializeField] protected float groundedYVelocity = -20;
    [SerializeField] protected float fallStartYVelocity = -5;

    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;
   [SerializeField] protected float gravityForce = -5.55f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();
        if (character.isGrounded)
        {
            if(yVelocity.y <0){
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;

            }
        }
        else
        {
            if (!character.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
                
            }
            inAirTimer += Time.deltaTime;
            character.animator.SetFloat("inAirTimer", inAirTimer);

            yVelocity.y += gravityForce * Time.deltaTime;
             
        }
        character.characterController.Move(yVelocity * Time.deltaTime);
    }
    protected void HandleGroundCheck()
    {
        character.isGrounded = Physics.CheckSphere(character.transform.position,groundCheckSphereRadius,groundLayer);
    }

    // protected void OnDrawGizmosSelected() {
    //     Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);

        
    // }
    

}
