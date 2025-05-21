using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileDataWriter saveFileWriter;

    [Header("Game Slot")]
    public CharacterSlots characterSlot;

    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timedPlayer;

    public void OnEnable()
    {
        LoadSaveSlot();
    }

    private void LoadSaveSlot()
    {
        saveFileWriter = new SaveFileDataWriter();
        saveFileWriter.saveDataDirPath = Application.persistentDataPath;

        //Save Slot 01
        if (characterSlot == CharacterSlots.CharacterSlot_01)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.singletonInstance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            // If the file exist, get information from it 
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.singletonInstance.characterSlot01.characterName;
            }
            // If it does not, disable this gameobject
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlots.CharacterSlot_02)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.singletonInstance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            // If the file exist, get information from it 
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.singletonInstance.characterSlot02.characterName;
            }
            // If it does not, disable this gameobject
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlots.CharacterSlot_03)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.singletonInstance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            // If the file exist, get information from it 
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.singletonInstance.characterSlot03.characterName;
            }
            // If it does not, disable this gameobject
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlots.CharacterSlot_04)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.singletonInstance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            // If the file exist, get information from it 
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.singletonInstance.characterSlot04.characterName;
            }
            // If it does not, disable this gameobject
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (characterSlot == CharacterSlots.CharacterSlot_05)
        {
            saveFileWriter.saveFileName = WorldSaveGameManager.singletonInstance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

            // If the file exist, get information from it 
            if (saveFileWriter.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.singletonInstance.characterSlot05.characterName;
            }
            // If it does not, disable this gameobject
            else
            {
                gameObject.SetActive(false);
            }
        }



    }

    public void LoadGameFromCharacterSlot(){
        WorldSaveGameManager.singletonInstance.currentCharacterSlotBeingUsed = characterSlot;
        WorldSaveGameManager.singletonInstance.LoadGame();
    }

    public void SelectCurrentSlot(){
        TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
    }
}
