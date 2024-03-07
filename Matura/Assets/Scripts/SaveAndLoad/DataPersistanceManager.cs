using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    private FileDataHandler dataHandler; 

    public static DataPersistanceManager Instance;

    private GameData gameData; 
    private List<IDataPersistance> dataPersitanceObjects;

    [SerializeField]
    private bool useEncryption; 

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
        }
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption); //Operating system standard directory 
        //Debug.Log(Path.Combine(Application.persistentDataPath, fileName));

        this.dataPersitanceObjects = FindAllDataPersistanceObjects(); 
        LoadGame(); 
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load(); //If it's null we create a new game

        if (this.gameData == null)
        {
            NewGame(); 
        }

        foreach (IDataPersistance dataPersistanceObject in dataPersitanceObjects)
        {
            dataPersistanceObject.LoadData(gameData);
        }
        Debug.Log(gameData.Health);
    }

    public void SaveGame()
    {
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

}
