using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public void OnExitButton()
    {
        AsyncLoader.Instance.LoadLevel("Menu");
    }

    public void OnMenuButton()
    {
        AsyncLoader.Instance.LoadLevel("Menu");
    }

    public void OnRespawnButton()
    {
        DataPersistanceManager.Instance.ResetGame(); 
        AsyncLoader.Instance.LoadLevel("Main");
    }
}
