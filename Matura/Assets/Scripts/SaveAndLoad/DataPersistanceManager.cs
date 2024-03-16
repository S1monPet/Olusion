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
        if (this.gameData == null)
        {
            return; 
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersitanceObjects)
        {
            dataPersistanceObject.SaveData(ref gameData);
        }

        Debug.Log(gameData.Health);
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame(); 
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
