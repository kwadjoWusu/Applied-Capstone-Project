using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerUIManger : MonoBehaviour
{
    public static PlayerUIManger instance;

    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;



    private void Awake(){
        if(instance == null){
            instance =this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (startGameAsClient){
            startGameAsClient = false; 
            //We must shutdown the network as a host to connect as a client, durning the title screen
            NetworkManager.Singleton.Shutdown();
            // We start the network as a client
            NetworkManager.Singleton.StartClient();
        }
    }
}
