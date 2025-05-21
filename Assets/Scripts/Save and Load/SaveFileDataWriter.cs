using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFileDataWriter
{
    public string saveDataDirPath = "";
    public string saveFileName = "";

    //Before We create a new save file we must check to see if one of this character slot already exists (max 10 character slots)
    public bool CheckToSeeIfFileExists()
    {
        if (File.Exists(Path.Combine(saveDataDirPath, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    // Used to delete character save files 
    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirPath, saveFileName));
    }
    //Used to create a save file upon starting a new game
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    {
        // Make a path to the save file
        string savePath = Path.Combine(saveDataDirPath, saveFileName);

        try
        {
            // Create the directory the file will be written to, if it does not exist
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Create Save File, At Save Path" + savePath);

            // Serialize the C#Game data object into json file
            string dataToStore = JsonUtility.ToJson(characterData, true);

            //Write the file to our system
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Whilst trying to save character data, game not saved " + savePath + "\n" + e);

        }

    }

    // Used a load a save File upon Loading a Previous game

    public CharacterSaveData LoadSaveFile()
    {

        CharacterSaveData characterData = null;

        // Make a path to the load file
        string loadPath = Path.Combine(saveDataDirPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Deserialize the data from Json Back to Unity C#
                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Failure Whilst trying to load character data" + loadPath + "\n" + e);
            }
        }

        return characterData;
    }
}
