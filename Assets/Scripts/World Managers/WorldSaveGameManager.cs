using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager singletonInstance;

    public PlayerManager player;

    [Header("Save/Load")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;




    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;


    [Header("Current Character Data")]
    public CharacterSlots currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string savefileName;

    [Header("Characters Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;
    public CharacterSaveData characterSlot05;





    private void Awake()
    {
        //There can only be one instance of this script at one time, if another exist,  then destroy it
        if (singletonInstance == null)
        {
            singletonInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }
        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }
    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots characterSlots)
    {
        string filename = "";
        switch (characterSlots)
        {
            case CharacterSlots.CharacterSlot_01:
                filename = "CharacterSlot_01";
                break;
            case CharacterSlots.CharacterSlot_02:
                filename = "CharacterSlot_02";
                break;
            case CharacterSlots.CharacterSlot_03:
                filename = "CharacterSlot_03";
                break;
            case CharacterSlots.CharacterSlot_04:
                filename = "CharacterSlot_04";
                break;
            case CharacterSlots.CharacterSlot_05:
                filename = "CharacterSlot_05";
                break;
            default:
                break;

        }
        return filename;
    }

    public void AttemptToCreateNewGame()
    {


        saveFileDataWriter = new SaveFileDataWriter();

        saveFileDataWriter.saveDataDirPath = Application.persistentDataPath;

        // Check to see if we can create a new save file (check for other existing files first)
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_01);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot_01;
            currentCharacterData = new CharacterSaveData();
            NewGame();

            return;
        }
        // Check to see if we can create a new save file (check for other existing files first)
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_02);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot_02;
            currentCharacterData = new CharacterSaveData();
            NewGame();

            return;
        }
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_03);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot_03;
            currentCharacterData = new CharacterSaveData();
            NewGame();

            return;
        }
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_04);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot_04;
            currentCharacterData = new CharacterSaveData();
            NewGame();

            return;
        }
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_05);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot_05;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        // if there are no free slots, notify the player
        TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
    }

    private void NewGame()
    {
        player.playerNetworkManager.life.Value = 15;
        player.playerNetworkManager.fortitude.Value = 10;

        SaveGame();
        StartCoroutine(LoadWorldScene());

    }

    public void LoadGame()
    {
        // Load a previous file with a file name depending on which slot we are using
        savefileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);
        saveFileDataWriter = new SaveFileDataWriter();
        //Generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = savefileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());

    }

    public void SaveGame()
    {
        // save the current file under a file name depending on which slot we are using 
        savefileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();

        //Generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = savefileName;
        // Pass the players info from Game, to their save file
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        // write that info onto a json file, saved to this machine
        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);

    }

    public void DeleteGame(CharacterSlots characterSlots)
    {
        // Choose file to delete based on the name
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlots);

        saveFileDataWriter.DeleteSaveFile();
    }

    // load all character profiles on device when starting game
    private void LoadAllCharacterProfiles()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_01);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_02);
        characterSlot02 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_03);
        characterSlot03 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_04);
        characterSlot04 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot_05);
        characterSlot05 = saveFileDataWriter.LoadSaveFile();
    }

    public IEnumerator LoadWorldScene()
    {

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

        yield return null;
    }

    // public IEnumerator LoadWorldScene()
    // {
    //     AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

    //     // Currently you only wait one frame:
    //     // yield return null;

    //     // —> Change that to wait for the actual scene load:
    //     yield return loadOperation;

    //     // Then apply the saved data onto your player
    //     player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
    // }




    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }


}
