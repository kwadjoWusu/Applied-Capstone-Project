using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager singletonInstance;
    [SerializeField] int worldSceneIndex = 1;

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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator LoadNewGame()
    {

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }


}
