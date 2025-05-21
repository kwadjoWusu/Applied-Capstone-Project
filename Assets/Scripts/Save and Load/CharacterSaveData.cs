using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// since we want to reference this data for every save file, this script is not monobehaviour and is instead serializable 
public class CharacterSaveData
{
    [Header("Scene Index")] // load into multiple scenes
    public int sceneIndex = 1;
    

    [Header("Character Name")]
    public string characterName = "Character";

    [Header("Time Played")]
    public float secondsPlayed;

    // you can only save data of basic data types
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;


    [Header("Resources")]
    public int currentHealth;
    public float currentStamina;


    [Header("Stats")]
    public int life;
    public int fortitude;



}
