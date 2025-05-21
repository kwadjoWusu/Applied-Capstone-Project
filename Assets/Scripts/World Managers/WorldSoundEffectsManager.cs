using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundEffectsManager : MonoBehaviour
{
    public static WorldSoundEffectsManager instance; 

    [Header("Action Sounds")]
    public AudioClip rollSoundEffect;

    private void Awake(){
        if (instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start(){
        DontDestroyOnLoad(gameObject);
    }
}
