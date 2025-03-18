using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{

    PlayerLocomotionManager playerLocomotionManager;
    protected override void Awake()
    {
        base.Awake();

        // Do more stuff for the player
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }

    protected override void Update()
    {
        base.Update();
        // if we do not own this gameobject we do notcontrol or edit it 
        if (!IsOwner)
            return;
        // handles all the characters movement
        playerLocomotionManager.HandleAllMovement();
    }
}
