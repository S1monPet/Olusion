using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State; 

    [Header("FPS")]
    private float fps;
    public TextMeshProUGUI FPSText;

    [Header("Player")]
    public PlayerState CurrentPlayerState = PlayerState.Alive; 
    public enum PlayerState {
        Alive, Dead
    };

    public void SetCurrentPlayerState(PlayerState currentPlayerState) 
    {
        CurrentPlayerState = currentPlayerState; 
    }

    public bool CheckDeadState() 
    {
        return CurrentPlayerState == PlayerState.Dead;
    }

    private void Awake()
    {
        // Disable vSync, mobile devices ignore it
        QualitySettings.vSyncCount = 0; 

        Application.targetFrameRate = 100;

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
        // InvokeRepeating(nameof(UpdateFPSDisplay), 1, 1);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Menu:
                break;
            case GameState.Main:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newState), newState, null); 
        }
    }

    public enum GameState {
        Menu, 
        Main
    }
    

    private void UpdateFPSDisplay() //InvokeRepeating requires
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPSText.text = fps.ToString();
    }

}
