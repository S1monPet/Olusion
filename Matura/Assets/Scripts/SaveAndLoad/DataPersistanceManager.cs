using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("Debbugging")]
    [SerializeField] private bool _initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    private FileDataHandler dataHandler; 

    public static DataPersistanceManager Instance;

    private GameData gameData; 
    private List<IDataPersistance> dataPersitanceObjects;

    [SerializeField]
    private bool useEncryption;

    [Header("Application quit")]
    static bool CanQuit = false;

    [ContextMenu("Doesn't work on iOS or iPadOS")]

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    static bool WantsToQuit()
    {
        Instance.StartCoroutine(Instance.SaveAllData()); 
        return CanQuit;
    }

    private IEnumerator SaveAllData()
    {
        SaveGame(); 
        yield return new WaitForSeconds(2);

        CanQuit = true; 
        Application.Quit();
    }
    
    // WantsToQuit function did not work, we will Reset Game to the start
    private void OnApplicationQuit() 
    {
        ResetGame(); 
    }

    /*
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    */

    private void Start()
    {
        if (!DataPersistanceManager.Instance.HasGameData()) 
        {
            // button.interactable = false; // Disable continue button 
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the GameManager across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures that there are no duplicate GameManagers
            return; 
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption); //Operating system standard directory 
        //Debug.Log(Path.Combine(Application.persistentDataPath, fileName));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersitanceObjects = FindAllDataPersistanceObjects(); // Everytime the scene is loaded, we initialize it

        // Waiting for Initialization
        //LoadGame(); 
    }



    public void OnSceneUnloaded(Scene scene)
    {
        // SaveGame(); 
    }

    public void NewGame()
    {
        ResetGameData(); 
        GameManager.Instance.SetCurrentPlayerState(GameManager.PlayerState.Alive);
        // this.gameData = null; // Reseting it to null when creating new game, didn't save
    }

    public void ResetGameData() 
    {
        this.gameData = new GameData(); 
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load(); //If it's null we create a new game
        Debug.Log("this game" + this.gameData);

        if (this.gameData == null && _initializeDataIfNull) // If we wan't to create a new file
        {
            NewGame(); 
        }

        if (this.gameData == null) // If we don't want to create a new file
        {
            return; //NewGame(); 
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersitanceObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        }
        Debug.Log(gameData.Health);
    }

    public void SaveGame()
    {
        // Setting default GameData
        if (GameManager.Instance.CheckDeadState()) // If player was dead, we create NewGame and return
        {
            ResetGame();
            return;
        }

        if (this.gameData == null)
        {
            return; 
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersitanceObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }

        // Debug.Log(gameData.Health);
        dataHandler.Save(gameData);
    }

    public void ResetGame()
    {
        NewGame();
        dataHandler.Save(gameData); // Saving empty object
    }

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistenceObjects);       
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

}
