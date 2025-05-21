using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;
    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        
    }

    protected override void Start()
    {
        base.Start();
        CalculateHealthBasedOnLifeLevel(player.playerNetworkManager.life.Value);
        CalculateStaminaBasedOnFortitudeLevel(player.playerNetworkManager.fortitude.Value);
    }
}
