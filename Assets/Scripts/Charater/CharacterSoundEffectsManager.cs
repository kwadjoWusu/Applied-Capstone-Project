using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundEffectsManager : MonoBehaviour
{
    private AudioSource audioSource;


    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();

    }

    public void PlayRollingSoundFX(){
        audioSource.PlayOneShot(WorldSoundEffectsManager.instance.rollSoundEffect);
    }
}
