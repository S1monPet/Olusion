using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        DataPersistanceManager.Instance.NewGame(); // Starting over

        AsyncLoader.Instance.LoadLevel("Main");
    } 

    public void OnContinueGameClicked()
    {
        AsyncLoader.Instance.LoadLevel("Main");
    }
}
