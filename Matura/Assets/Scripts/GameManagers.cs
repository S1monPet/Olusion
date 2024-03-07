using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    public static GameManagers Instance;
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
}
