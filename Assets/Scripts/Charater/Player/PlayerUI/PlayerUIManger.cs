using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;

    [HideInInspector] public PlayerUIHudManager playerUIHudManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;

    private void Awake(){
        // Singleton pattern - ensure only one instance exists
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize UI components
            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>(true);
            if (playerUIHudManager == null) {
                Debug.LogError("[PlayerUIManager] Failed to find PlayerUIHudManager component!", this);
            }
            
            playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>(true);
            if (playerUIPopUpManager == null) {
                Debug.LogError("[PlayerUIManager] Failed to find PlayerUIPopUpManager component!", this);
            }
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (startGameAsClient){
            startGameAsClient = false; 
            //We must shutdown the network as a host to connect as a client, during the title screen
            NetworkManager.Singleton.Shutdown();
            // We start the network as a client
            NetworkManager.Singleton.StartClient();
        }
    }
    
    /// <summary>
    /// Safely get the PopUpManager, logging an error if it's not found
    /// </summary>
    /// <returns>The PopUpManager or null if not found</returns>
    public PlayerUIPopUpManager GetPopUpManager() {
        if (playerUIPopUpManager == null) {
            // Try to find it if it's null for some reason
            playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>(true);
            
            if (playerUIPopUpManager == null) {
                Debug.LogError("[PlayerUIManager] PopUpManager is null and could not be found!", this);
                return null;
            }
        }
        return playerUIPopUpManager;
    }
}